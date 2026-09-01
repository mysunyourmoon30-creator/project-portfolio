# Retrospective

## Decisions made that deviate from a literal reproduction

**Consolidated 16 tables into one SQLite database.** The real system splits
these across ~5 SQL Server databases and pays for it with the cross-database
transaction bug documented in Backend ROADMAP §7b.3 (one commit left
commented out, no distributed transaction). Rather than reproduce that
specific bug and then "fix" it with a comment, this clone applies README
§6.2's own recommendation directly: one database, one transaction boundary.
`SaveTotalWeightAsync` withdrawing `RM_BAL` in the same `SaveChanges()` call
as writing the `Weighting`/`TotalWeight` rows (see
`KanbanControllerTests.SaveTotalWeight_HappyPath_PersistsAndWithdrawsRmBal_InOneTransaction`)
is the concrete, testable proof this decision pays off.

**Shared DTOs between API and Desktop instead of parallel Dto/VM classes.**
The original duplicates ~10 records per screen because the ten real desktop
apps are physically separate solutions with no shared library. This clone's
API and Desktop live in one solution, so `Innovation.Services.Contracts`'
DTOs are referenced directly from both sides. This is the one convention
README explicitly did *not* ask to change that we changed anyway, purely for
build-time simplicity - worth knowing if a reviewer expects VM duplication
as a checklist item.

**Trimmed screen count.** Only the four screens needed for the T1/T2 happy
path and the four "must not close" scenarios were built (login, select-kanban,
main weighing, auto-feed), not all seven originally scoped in the plan
(`frmPLCTest` and a standalone `frmSaveTotal` were folded into
`Presenter_TotalWeight`/`Presenter_ShowAutoFeed` directly). The
architecturally interesting parts - constructor injection, event-based
view/presenter wiring, the NotFound-bug fix - don't need a dedicated PLC-test
screen to demonstrate.

## A real bug this process caught

Phase 4's first draft injected `IPresenter_UserLogin` directly into
`frmUserLogin`'s constructor (so the view could call presenter methods from
button click handlers) while `Presenter_UserLogin` also took
`IView_UserLogin` in its constructor. That's an unresolvable DI cycle -
`dotnet run` failed immediately with `A circular dependency was detected`.
This is the same shape of problem the original system's `_view.Presenter =
this` back-wiring existed to route around (Frontend ROADMAP §5b.5), just
caught by the DI container instead of silently working via a settable
property. The fix: views expose events (`LoginRequested`,
`BarcodeScanned`, `StepWeightEntered`, `SaveRequested`, `AcceptRequested`);
presenters subscribe to them in their constructor. The dependency stays
strictly one-directional (presenter → view), which is what `IView<T>`
dropping its `Presenter` setter was trying to guarantee in the first place -
it just needed the event-based wiring to actually make that guarantee
buildable.

**Lesson:** removing a bad pattern's *symptom* (the settable back-reference)
without providing the *replacement mechanism* for the legitimate thing it was
doing (view notifying presenter of user actions) just moves the cycle from
"silently works via a mutable property" to "throws at startup." Actually
running the app - not just running unit tests with mocked views - is what
surfaced this.

## Two more bugs a real run caught, that unit tests missed

After the DI-cycle fix, the app started but pressing Enter in the barcode
field, clicking Save, and clicking Accept all did *nothing* - no error, no
API call, no log line. The cause: `Program.cs`'s composition root resolved
`IView_TotalWeight` from the container to get the main form, but never
resolved `IPresenter_TotalWeight`. Since `Presenter_TotalWeight`'s
constructor is where it subscribes to the view's events
(`BarcodeScanned`, `SaveRequested`, etc.), and nothing ever asked the
container for that presenter, it was simply never constructed - the view sat
there with zero subscribers on every event. Every `Presenter_TotalWeightTests`
unit test passed because those tests construct the presenter directly with
`new Presenter_TotalWeight(...)`, which always wires the events; none of
them exercise `Program.cs`'s own resolution path. Fixed by resolving the
presenter (not the view) and reading `presenter.View` back out.

Separately, the save confirmation's message box showed a warning triangle
instead of an info icon. `ShowMessage`'s icon mapping (`type == Error ?
Error : Warning`) only special-cased `Error` and silently treated
`Information` as `Warning` too. Fixed by extracting a proper three-way
switch into `MessageBoxIconMapper`, shared across all four forms (it had
been copy-pasted identically into each one), with a test locking in all
three cases.

**Lesson, reinforced:** both bugs were invisible to a comprehensive unit
test suite (79 tests, all green) because unit tests exercise the classes
you construct directly, not the composition root that wires them together
in the shipped app, and not the exact enum-to-icon mapping a human eye
would catch in half a second. There is no substitute for actually running
the thing and looking at it - which is exactly what this session did,
end to end, against a real running API, using UI Automation to drive the
real WinForms controls.

## A fourth bug: a rejected value silently surviving into Save

A later re-run of the demo script (deliberately re-verifying the full
checklist end to end, not just spot-checking) entered an out-of-range
weight (12.00, tolerance [9.5, 10.5]), got the expected warning, then
clicked **Save** *without correcting the value*. The API log showed the
save going through anyway - `RM_BAL` withdrawn, `TotalWeight` inserted -
using the rejected 12.00.

The cause: WinForms' `DataGridView` commits an edited cell into its bound
object **before** raising `CellEndEdit` - by the time
`Presenter_TotalWeight.SubmitStepWeightAsync` runs its tolerance check,
`row.Actual` already equals the candidate weight, rejected or not. The
existing unit test (`SubmitStepWeight_OutsideTolerance_ShowsWarning_DoesNotSetActual`)
never caught this because it calls the presenter method directly with
`row.Actual` starting at `null` - a mental model of the interaction that
doesn't match how the real `DataGridView` event actually fires. Fixed by
explicitly setting `row.Actual = null` in the rejection branch, and added a
second test (`SubmitStepWeight_OutsideTolerance_RevertsActualThatGridAlreadyCommitted`)
that starts `row.Actual` already set to the rejected value, matching reality.

**Lesson, reinforced again:** a unit test can be internally consistent and
still encode the wrong assumption about *when* a UI framework mutates
shared state relative to the event it raises. The fix came from re-running
the exact same demo script a second time end to end rather than assuming
"already verified once" was enough - the first run happened to always
correct the value before saving, so the gap never surfaced until a
verification pass tried the "reject then save without fixing it" path.

## What I'd do differently with more time

- Build the WinForms designer surfaces properly (this clone hand-writes
  `.Designer.cs` files; a real Visual Studio session with the visual
  designer would produce cleaner control layouts).
- Cover all 16 entities with dedicated repository contract tests rather than
  the representative subset + schema-smoke-test approach `RepositoryImplTests`
  takes, if the goal shifts from "demonstrate the pattern" to "guarantee
  every table's mapping."
- Audit every other `DataGridView`-bound field for the same "grid commits
  before the handler runs" assumption - `Accepted` is a checkbox column
  bound the same way and was never stress-tested with a rejected/reverted
  value the way `Actual` was here.
