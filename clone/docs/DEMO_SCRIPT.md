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
3. In **ยิงบาร์โค้ดคัมบัง**, type `KB0000001` and press **Enter**.
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

## 4. Exercise the "must not close" scenarios

The main screen has a **ทดสอบ Auto-feed** panel with Barcode / Line ID / Plan
ID fields and a button - use it to drive `Presenter_ShowAutoFeed` directly:

| Barcode | Line ID | Plan ID | Expected result |
|---|---|---|---|
| `RM001` | `1` | `1` | Success - dialog closes itself, shows "ป้อนวัตถุดิบสำเร็จ" |
| any unknown value, e.g. `NOPE` | `1` | `1` | Warning "ไม่พบบาร์โค้ด...ใน RM_BAL" - **dialog stays open**, click ปิด [Esc] to dismiss |
| `RM001` | `999` (unconfigured) | `1` | Warning "ไม่ได้ตั้งค่า Feeddoor Step..." - **dialog stays open** |
| `RM001` | `1` | `999` (no MixTemp row) | **Not an error** - proceeds to success and closes itself, same as the happy path |

Every one of these was run for real (screenshots below), not just asserted
in a unit test. The same four scenarios are also covered by
`Presenter_ShowAutoFeedTests` and `Innovation.Hardware.Tests` if you'd rather
run them headlessly:

```bash
dotnet test tests/Innovation.TotalWeight_PLC.Tests/Innovation.TotalWeight_PLC.Tests.csproj --filter Presenter_ShowAutoFeedTests
```

![Auto-feed panel](screenshots/04b-autofeed-panel.png)

**Barcode not found** - warning shown, dialog still open behind it:

![Barcode not found warning](screenshots/05-autofeed-barcode-not-found-warning.png)
![Dialog still open](screenshots/05b-autofeed-dialog-still-open.png)

**Feeddoor Step not configured** - warning shown, dialog still open:

![Feeddoor missing warning](screenshots/06-autofeed-feeddoor-missing-warning.png)
![Dialog still open](screenshots/06b-autofeed-dialog-still-open.png)

**Missing MixTemp (not an error) → success, closes itself:**

![Auto-feed success](screenshots/07-autofeed-success.png)

This is the direct fix for the original `NotFound()` bug (Frontend ROADMAP
§5b.2), which conflated "report a problem" with "close the form."

## 5. Screenshots

This walkthrough was run for real on Windows against the live API, driven
via UI Automation instead of by hand - each step below is an actual
screenshot, not a mockup.

![Login](screenshots/01-login.png)

![Kanban loaded](screenshots/02-kanban-loaded.png)

![Weight entered](screenshots/03-weight-entered.png)

![Save confirmation](screenshots/04-save-confirmation.png)

Running this walkthrough for real caught two bugs that no unit test had
caught (both fixed, both now covered by regression tests) - see
[`RETROSPECTIVE.md`](RETROSPECTIVE.md) for details:

1. `Program.cs` only resolved `IView_TotalWeight` from the container, never
   `IPresenter_TotalWeight` - so the presenter (and its event subscriptions)
   was never constructed, and the barcode/save/accept actions silently did
   nothing.
2. `ShowMessage`'s icon mapping only special-cased `Error`, so `Information`
   messages showed a warning triangle instead of the info icon.
