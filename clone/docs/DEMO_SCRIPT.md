# Demo script

## Prerequisites

- .NET 8 SDK installed
- Windows (required for the WinForms desktop app)

## 1. Start the API

```bash
cd clone/src/Backend/Innovation.Api
dotnet run --urls http://localhost:5299
```

On first run this creates `totalweight.db` (SQLite), applies the
`InitialCreate` migration, and seeds one demo operator + one weighable
kanban via `DemoDataSeeder`.

## 2. Start the desktop app

In a second terminal:

```bash
cd clone/src/Desktop/Innovation.TotalWeight_PLC
dotnet run
```

## 3. Walk through the happy path

1. **Login dialog** appears first. Enter:
   - Username: `operator1`
   - Password: `Password123!`
2. Click **เข้าสู่ระบบ [Enter]**. On success the main weighing screen opens.
   Entering a wrong password instead shows "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง"
   and keeps the login dialog open - verified live against the real API's
   401 response.
3. Either type `KB0000001` into **ยิงบาร์โค้ดคัมบัง** and press **Enter**, or
   click **เลือกคัมบัง...** to open a dialog listing every kanban with
   `Status = "Pending"` (fetched live from `GET /api/kanbans`), pick
   `KB0000001` from the grid, and click **ตกลง [F5]** - either path loads
   the same kanban. Clicking **ยกเลิก [Esc]** in that dialog returns to the
   main screen with nothing loaded, no API side effect.
   - The steps grid populates with one row: step 1, raw material `RM001`,
     target 10.00, range [9.50, 10.50].
4. Click into the **Actual** cell of step 1, type `10.05`, and press Tab/Enter
   to commit the edit.
   - A weight inside the tolerance range is accepted silently; a weight
     outside it (try `12.00`) pops a warning and the cell reverts.
5. Click **บันทึก [F5]** (or press F5). A confirmation message appears
   ("บันทึกน้ำหนักเรียบร้อยแล้ว"), and the API withdraws 10.05 from `RM001`'s
   `RM_BAL` balance in the same database transaction.
6. Select the step row and click **ยืนยัน (Accept) [F2]** (or press F2). The
   row's Accepted checkbox flips to true.

This full flow was run for real on Windows against the live API (not just
asserted in a test). Doing so caught two bugs that no unit test had caught
(both fixed, both now covered by regression tests) - see
[`RETROSPECTIVE.md`](RETROSPECTIVE.md) for details:

1. `Program.cs` only resolved `IView_TotalWeight` from the container, never
   `IPresenter_TotalWeight` - so the presenter (and its event subscriptions)
   was never constructed, and the barcode/save/accept actions silently did
   nothing.
2. `ShowMessage`'s icon mapping only special-cased `Error`, so `Information`
   messages showed a warning triangle instead of the info icon.

## 4. Exercise the "must not close" scenarios

The main screen has a **ทดสอบ Auto-feed** panel with Barcode / Line ID / Plan
ID fields and a button - use it to drive `Presenter_ShowAutoFeed` directly:

| Barcode | Line ID | Plan ID | Expected result |
|---|---|---|---|
| `RM001` | `1` | `1` | Success - dialog closes itself, shows "ป้อนวัตถุดิบสำเร็จ" |
| any unknown value, e.g. `NOPE` | `1` | `1` | Warning "ไม่พบบาร์โค้ด...ใน RM_BAL" - **dialog stays open**, click ปิด [Esc] to dismiss |
| `RM001` | `999` (unconfigured) | `1` | Warning "ไม่ได้ตั้งค่า Feeddoor Step..." - **dialog stays open** |
| `RM001` | `1` | `999` (no MixTemp row) | **Not an error** - proceeds to success and closes itself, same as the happy path |
| `RM001` | `1` | `1`, with PLC unreachable | Warning "ติดต่อ PLC ไม่ได้ กรุณาตรวจสอบการเชื่อมต่อ" - **dialog stays open** |

Every one of these five was run for real against the live API, not just
asserted in a unit test - each failure case left the dialog genuinely open
(confirmed via its window handle still existing) with the "ปิด [Esc]" button
available, and the API log confirmed which lookup failed in each case. The
PLC-unreachable row required a temporary `Program.cs` swap to a
`SimulatedPlcDevice` configured to fail connecting, reverted immediately
after (`git diff` showed zero changes once reverted). The same scenarios are
also covered by `Presenter_ShowAutoFeedTests` and
`Innovation.Hardware.Tests` if you'd rather run them headlessly:

```bash
dotnet test tests/Innovation.TotalWeight_PLC.Tests/Innovation.TotalWeight_PLC.Tests.csproj --filter Presenter_ShowAutoFeedTests
```

This is the direct fix for the original `NotFound()` bug (Frontend ROADMAP
§5b.2), which conflated "report a problem" with "close the form."

### Scenario 7 - DB write fails during withdrawal

RM_BAL/Feeddoor/MixTemp all succeed; only the write fails. This needs a
genuine write failure, which isn't reachable through normal UI input alone.
It was verified once, live, by temporarily adding an environment-variable-
gated fault injection (`DEMO_FORCE_WITHDRAW_FAILURE=1`) to
`ExecuteRmBalWithdraw`, running the same RM001/1/1 happy-path input against
it, and reverting the change immediately after (`git diff` showed zero
changes once reverted - it never shipped). The API log confirmed the
RM_BAL, Feeddoor, and MixTemp lookups all succeeded before the simulated
write failure, isolating the exact code path: the warning appeared
("เขียนฐานข้อมูลไม่สำเร็จระหว่าง auto-feed") and the dialog stayed open,
requiring a manual close.

This is the same catch-and-warn branch `Presenter_ShowAutoFeedTests
.DbWriteFailsDuringAutoFeed_ShowsWarning_DoesNotCloseDialog` already covers
with a mocked `IApiClient` - this run just proved it also holds with a real
HTTP round trip and a real (if artificially triggered) server-side failure.
