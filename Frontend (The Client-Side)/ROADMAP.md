\# Frontend (The Client-Side) — Architecture Blueprint



> เอกสารนี้บรรยาย\*\*ระบบจริงตามที่เป็นอยู่\*\* รวมข้อบกพร่องและคำที่สะกดผิด — ไม่ได้แก้ให้สวยขึ้น

> แผน "สร้างใหม่ให้สะอาด" อยู่ใน \[../README.md](../README.md)

>

> ค่าที่เป็นความลับถูกแทนด้วย placeholder ทั้งหมด



\---



\## 1. ภาพรวม



แอปเดสก์ท็อป \*\*WinForms 10 ตัวที่เป็นอิสระต่อกัน\*\* ไม่ได้อยู่ในโซลูชันเดียวกัน ไม่ได้ build พร้อมกัน

และ deploy แยกกันไปยังเครื่องหน้าโรงงานใน 6 ไซต์ ทุกตัวคุยกับ backend ผ่าน REST API



| ตัวเลขรวม | ค่า |

|---|---:|

| แอป | 10 |

| ไฟล์ `.cs` (ไม่รวม `bin`/`obj`) | \~1,500 |

| โฟลเดอร์ | 397 |

| ฟอร์ม (มี `ClientSize`) | \*\*165\*\* |

| เทมเพลตรายงาน `XtraReport` | 21 |

| ไฟล์ `.Designer.cs` ที่เป็น UI รวม | 186 |



\### ข้อจำกัดที่ต้องคงไว้



\- \*\*.NET Framework\*\* — v4.6.2 หรือ v4.7.2 ไม่ใช่ .NET Core/5+

\- `.csproj` เป็นรูปแบบเก่า (ไม่ใช่ SDK-style) ระบุไฟล์ทีละไฟล์ด้วย `<Compile Include=...>`

\- \*\*DevExpress v21.2\*\* — ไลบรารีเชิงพาณิชย์ ต้องมี license จึงจะ build ได้

\- \*\*`InnoControlLibrary`\*\* อ้างด้วย absolute path `D:\\Library\\InnoControlLibrary.dll`

\- `Innovation.TotalWeight\_PLC` บังคับ \*\*C# 7.3 เท่านั้น\*\* (ดู `Prompt/system\_prompt\_FULL.md` ของโปรเจกต์)

\- อ้าง DLL ของ backend ผ่าน `HintPath` ที่ชี้ไปยังโฟลเดอร์ \*\*`bin\\Debug\\`\*\* ของโซลูชันข้างเคียง



\---



\## 2. โครงสร้าง



ทุกโฟลเดอร์ปรากฏครบ \*\*397 โฟลเดอร์\*\* (ไม่รวม `bin/`, `obj/`, `.vs/`, `packages/`)

`\[n cs]` = จำนวนไฟล์ `.cs` ในโฟลเดอร์นั้นโดยตรง, `<...>` = ไฟล์สำคัญประจำโฟลเดอร์



```text

Frontend (The Client-Side)/

|-- Innovation.BomReplace/  <Innovation.BomReplace.sln>

|   `-- Innovation.BomReplace/  \[1 cs]  <App.config, Innovation.BomReplace.csproj, Program.cs>

|       |-- Forms/  \[4 cs]

|       |-- Interfaces/

|       |   |-- Presenters/  \[5 cs]

|       |   |-- Services/  \[2 cs]

|       |   `-- Views/  \[3 cs]

|       |-- Presenters/  \[3 cs]

|       |-- Properties/  \[3 cs]

|       |-- Services/  \[2 cs]

|       `-- ViewModels/

|-- Innovation.DrawingReturn/  \[1 cs]  <App.config, Innovation.DrawingReturn.csproj, Program.cs>

|   |-- AppController/

|   |   |-- Implements/  \[1 cs]

|   |   `-- Interfaces/  \[1 cs]

|   |-- AutoUpdate\_Setting\_By\_Site/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Presenters/

|   |   |-- Implements/  \[3 cs]

|   |   `-- Interfaces/  \[6 cs]

|   |-- Program\_Config\_By\_Site/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Properties/  \[3 cs]

|   |   `-- DataSources/

|   |-- Resources/

|   |-- Services/  \[1 cs]

|   |   |-- Implements/  \[2 cs]

|   |   `-- Interfaces/  \[2 cs]

|   |-- UI/

|   |   |-- Implements/  \[6 cs]

|   |   `-- Interfaces/  \[5 cs]

|   `-- ViewModel/

|       |-- AppController/  \[6 cs]

|       |-- DBMaster/  \[3 cs]

|       `-- ReturnRm/  \[2 cs]

|-- Innovation.InventoryManagement/  \[3 cs]  <App.config, Innovation.InventoryManagement.csproj, Program.cs>

|   |-- ApplicationService/  \[1 cs]

|   |-- AutoUpdate\_Setting\_By\_Site/

|   |   |-- BKK/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Const/  \[2 cs]

|   |   `-- ProgramModule/  \[1 cs]

|   |-- Global/  \[1 cs]

|   |-- Helper/  \[3 cs]

|   |-- InventoryMainMenu/

|   |   |-- AppController/  \[1 cs]

|   |   |-- Presenter/  \[2 cs]

|   |   |-- Service/  \[3 cs]

|   |   `-- UI/  \[4 cs]

|   |-- InventoryTransaction/

|   |   |-- AppController/  \[1 cs]

|   |   |-- Presenter/  \[16 cs]

|   |   |-- Service/  \[1 cs]

|   |   `-- UI/  \[32 cs]

|   |-- MaterialPlanTransfer/

|   |   |-- AppController/  \[1 cs]

|   |   |-- Presenter/  \[6 cs]

|   |   |-- Service/  \[2 cs]

|   |   `-- UI/  \[12 cs]

|   |-- MaterialRequest/

|   |   |-- AppController/  \[1 cs]

|   |   |-- BarCodeReport/  \[4 cs]

|   |   |-- Presenter/  \[18 cs]

|   |   |-- Service/  \[2 cs]

|   |   `-- UI/  \[32 cs]

|   |-- PackageReposrt/

|   |   |-- AppController/  \[1 cs]

|   |   |-- Presenter/  \[5 cs]

|   |   |-- Service/  \[1 cs]

|   |   `-- UI/  \[10 cs]

|   |-- Program\_Config\_By\_Site/

|   |   |-- BKK/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Properties/  \[3 cs]

|   |   `-- DataSources/

|   |-- QCLab/

|   |   |-- AppController/  \[1 cs]

|   |   |-- Presenter/  \[4 cs]

|   |   |-- Service/  \[2 cs]

|   |   `-- UI/  \[8 cs]

|   |-- ReportDevExpress/

|   |   |-- NewLabel/

|   |   |   |-- Presenter/  \[1 cs]

|   |   |   `-- UI/  \[2 cs]

|   |   |-- PackageReport/

|   |   |   |-- Presenter/  \[5 cs]

|   |   |   `-- UI/  \[10 cs]

|   |   `-- TransferReport/

|   |       |-- Presenter/  \[3 cs]

|   |       |-- Service/  \[1 cs]

|   |       `-- UI/  \[6 cs]

|   `-- Resources/

|-- Innovation.LossPreventionManagement/  <Innovation.LossPreventionManagement.sln>

|   |-- Innovation.Application/  <Innovation.LossPreventionManagement.sln>

|   |   `-- Innovation.ProductionManagement/

|   `-- Innovation.LossPreventionManagement/  \[1 cs]  <App.config, Innovation.LossPreventionManagement.csproj, Program.cs>

|       |-- AppController/

|       |   |-- Implements/  \[1 cs]

|       |   `-- Interfaces/  \[1 cs]

|       |-- Presenters/

|       |   |-- Implements/  \[3 cs]

|       |   `-- Interfaces/  \[6 cs]

|       |-- Properties/  \[3 cs]

|       |-- Resources/

|       |-- Services/  \[1 cs]

|       |   |-- Implements/  \[2 cs]

|       |   `-- Interfaces/  \[2 cs]

|       |-- UI/

|       |   |-- Implements/  \[6 cs]

|       |   `-- Interfaces/  \[5 cs]

|       `-- ViewModel/

|           |-- AppController/  \[6 cs]

|           `-- DBMaster/  \[3 cs]

|-- Innovation.MasterData/  \[2 cs]  <App.config, Innovation.MasterData.csproj, Program.cs>

|   |-- AppController/

|   |   |-- Implements/  \[1 cs]

|   |   `-- Interface/  \[1 cs]

|   |-- AutoUpdate\_Setting\_By\_Site/

|   |   |-- BKK/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Const/  \[4 cs]

|   |-- Global/  \[1 cs]

|   |-- Helper/  \[4 cs]

|   |-- Presenter/

|   |   |-- Implements/  \[36 cs]

|   |   `-- Interface/  \[40 cs]

|   |-- Program\_Config\_By\_Site/

|   |   |-- BKK/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Properties/  \[3 cs]

|   |   `-- DataSources/

|   |-- Resources/

|   |-- Service/  \[1 cs]

|   |   |-- Implements/  \[11 cs]

|   |   |   `-- ServiceValidation/  \[1 cs]

|   |   `-- Interface/  \[11 cs]

|   |-- UI/

|   |   |-- Implements/  \[77 cs]

|   |   `-- Interface/  \[38 cs]

|   |-- Validator/  \[14 cs]

|   `-- ViewModel/  \[208 cs]

|       |-- TESTAPI/  \[7 cs]

|       `-- UserPremission/  \[2 cs]

|-- Innovation.ProductionManagement/  \[1 cs]  <App.config, Innovation.ProductionManagement.csproj, Program.cs>

|   |-- AppController/

|   |   |-- Implements/  \[1 cs]

|   |   `-- Interface/  \[1 cs]

|   |-- ApplicationInterface/

|   |   `-- UI/  \[2 cs]

|   |       `-- Presenter/  \[3 cs]

|   |-- AutoUpdate\_Setting\_By\_Site/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Const/  \[1 cs]

|   |-- Global/  \[1 cs]

|   |-- Helper/  \[4 cs]

|   |-- ProductionMainMenu/

|   |   |-- Presenter/

|   |   |   |-- Implements/  \[2 cs]

|   |   |   `-- Interface/  \[2 cs]

|   |   |-- Service/  \[1 cs]

|   |   |   |-- Implements/  \[2 cs]

|   |   |   `-- Interface/  \[2 cs]

|   |   `-- UI/

|   |       |-- Implements/  \[6 cs]

|   |       `-- Interface/  \[2 cs]

|   |-- ProductionManagement/

|   |   |-- Approve/

|   |   |   `-- ApproveInterface/

|   |   |       |-- Presenter/

|   |   |       |   |-- Implements/  \[2 cs]

|   |   |       |   `-- Interface/  \[2 cs]

|   |   |       |-- Service/

|   |   |       |   |-- Implements/  \[1 cs]

|   |   |       |   `-- Interface/  \[1 cs]

|   |   |       `-- UI/

|   |   |           |-- Implements/  \[4 cs]

|   |   |           `-- Interface/  \[2 cs]

|   |   |-- CycleTime/

|   |   |   |-- Presenter/

|   |   |   |   |-- Implements/  \[2 cs]

|   |   |   |   `-- Interface/  \[2 cs]

|   |   |   |-- Service/

|   |   |   |   |-- Implements/  \[1 cs]

|   |   |   |   `-- Interface/  \[1 cs]

|   |   |   `-- UI/

|   |   |       |-- Implements/  \[4 cs]

|   |   |       `-- Interface/  \[2 cs]

|   |   |-- DailyProduction/

|   |   |   |-- Presenter/

|   |   |   |   |-- Implements/  \[5 cs]

|   |   |   |   `-- Interface/  \[5 cs]

|   |   |   |-- Service/

|   |   |   |   |-- Implements/  \[5 cs]

|   |   |   |   `-- Interface/  \[5 cs]

|   |   |   `-- UI/

|   |   |       |-- Implements/  \[10 cs]

|   |   |       `-- Interface/  \[5 cs]

|   |   |-- PackingCheckSheet/

|   |   |   |-- Presenter/

|   |   |   |   |-- Implements/  \[3 cs]

|   |   |   |   `-- Interface/  \[3 cs]

|   |   |   |-- ReportPackingCheckSheet/

|   |   |   |   |-- Implements/  \[2 cs]

|   |   |   |   `-- Interface/  \[1 cs]

|   |   |   |-- Service/

|   |   |   |   |-- Implements/  \[2 cs]

|   |   |   |   `-- Interface/  \[2 cs]

|   |   |   `-- UI/

|   |   |       |-- Implements/  \[4 cs]

|   |   |       `-- Interface/  \[2 cs]

|   |   |-- Report/

|   |   |   |-- Presenter/

|   |   |   |   |-- Implements/  \[1 cs]

|   |   |   |   `-- Interface/  \[1 cs]

|   |   |   |-- ReportDevExpress/

|   |   |   |   |-- Presenter/

|   |   |   |   |   |-- Implements/  \[9 cs]

|   |   |   |   |   `-- Interface/  \[9 cs]

|   |   |   |   |-- Service/

|   |   |   |   |   |-- Implements/  \[9 cs]

|   |   |   |   |   `-- Interface/  \[9 cs]

|   |   |   |   `-- UI/

|   |   |   |       |-- Implements/  \[18 cs]

|   |   |   |       `-- Interface/  \[9 cs]

|   |   |   |-- Service/

|   |   |   |   |-- Implements/  \[1 cs]

|   |   |   |   `-- Interface/  \[1 cs]

|   |   |   `-- UI/

|   |   |       |-- Implements/  \[2 cs]

|   |   |       `-- Interface/  \[1 cs]

|   |   `-- Supplement/

|   |       |-- Presenter/

|   |       |   |-- Implements/  \[4 cs]

|   |       |   `-- Interface/  \[4 cs]

|   |       |-- Service/

|   |       |   |-- Implements/  \[4 cs]

|   |       |   `-- Interface/  \[4 cs]

|   |       `-- UI/

|   |           |-- Implements/  \[8 cs]

|   |           `-- Interface/  \[4 cs]

|   |-- Program\_Config\_By\_Site/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Properties/  \[3 cs]

|   |   `-- DataSources/

|   |-- Resources/

|   `-- ViewModel/  \[29 cs]

|       |-- CycleTime/  \[3 cs]

|       |-- DailyInputData/  \[12 cs]

|       |-- PackingCheckSheet/  \[6 cs]

|       |-- PickingListRouteCard/  \[14 cs]

|       |-- Report/  \[21 cs]

|       `-- Route\_Card/  \[14 cs]

|-- Innovation.RM\_Confirm/  \[3 cs]  <App.config, Innovation.RM\_Confirm.csproj, Program.cs>

|   |-- AppController/  \[2 cs]

|   |-- Config/

|   |   |-- CPL/

|   |   |-- PI1/

|   |   |-- PI2/

|   |   |-- PI3/

|   |   `-- PI4/

|   |-- Presenter/

|   |   |-- Implements/  \[6 cs]

|   |   `-- Interface/  \[9 cs]

|   |-- Properties/  \[3 cs]

|   |   `-- DataSources/

|   |-- Resources/

|   |-- Service/

|   |   |-- Implements/  \[1 cs]

|   |   `-- Interface/  \[1 cs]

|   |-- UI/

|   |   |-- Implements/  \[12 cs]

|   |   `-- Interface/  \[8 cs]

|   `-- ViewModel/  \[8 cs]

|-- Innovation.SCADAReportViewer/

|   |-- .nuget/

|   |-- AutoUpdateFile/

|   |-- Innovation.SCADAReportViewer.Win.Ui/  \[3 cs]  <App.config, Innovation.SCADAReportViewer.Win.Ui.csproj, Program.cs>

|   `-- Innovation.SCADAReportViewer.Win.Ui.Setup/

|       |-- AppController/

|       |   |-- Implement/  \[1 cs]

|       |   `-- Interface/  \[5 cs]

|       |-- AutoUpdate\_Setting\_By\_Site/

|       |   |-- BKK/

|       |   |-- CPL/

|       |   |-- PI1/

|       |   |-- PI2/

|       |   |-- PI3/

|       |   `-- PI4/

|       |-- Helpers/  \[1 cs]

|       |-- Presenter/

|       |   |-- Implement/  \[5 cs]

|       |   `-- Interface/  \[5 cs]

|       |-- Program\_Config\_By\_Site/

|       |   |-- BKK/

|       |   |-- CPL/

|       |   |-- PI1/

|       |   |-- PI2/

|       |   |-- PI3/

|       |   `-- PI4/

|       |-- Properties/  \[3 cs]

|       |   `-- DataSources/

|       |-- Resouce/

|       |-- Resouces/

|       |-- Resources/

|       |-- Service/

|       |   |-- Implement/  \[6 cs]

|       |   `-- Interface/  \[6 cs]

|       |-- UI/  \[12 cs]

|       |   |-- Implement/  \[10 cs]

|       |   `-- Interface/  \[6 cs]

|       `-- ViewModel/  \[22 cs]

|-- Innovation.TotalWeight\_PLC/  \[1 cs]  <Innovation.TotalWeight\_PLC.csproj, Innovation.TotalWeight\_PLC.sln, Program.cs, app.config>

|   |-- .claude/

|   |-- Common/  \[5 cs]

|   |   |-- Enums/  \[4 cs]

|   |   `-- Extensions/

|   |-- Controllers/  \[2 cs]

|   |-- Helpers/  \[6 cs]

|   |-- Interfaces/

|   |   |-- Presenters/  \[3 cs]

|   |   `-- Views/  \[2 cs]

|   |-- Presenter/  \[1 cs]

|   |   |-- Implementations/  \[22 cs]

|   |   `-- Interfaces/  \[25 cs]

|   |-- Prompt/

|   |-- Properties/  \[3 cs]

|   |   `-- DataSources/

|   |-- Resources/

|   |-- Service/  \[3 cs]

|   |   |-- Implementations/  \[5 cs]

|   |   `-- Interfaces/  \[2 cs]

|   |-- Tracing/  \[2 cs]

|   |-- UI/

|   |   |-- Implementations/  \[56 cs]

|   |   `-- Interfaces/  \[26 cs]

|   `-- ViewModel/  \[66 cs]

|       |-- InputBarcode/  \[1 cs]

|       |   |-- Request/

|       |   `-- Response/

|       |-- Request/  \[11 cs]

|       `-- Response/  \[10 cs]

`-- KB\_PLC\_Control/  \[18 cs]  <KB\_PLC\_Control.csproj, Program.cs, app.config>

&#x20;   |-- AutoUpdate\_Setting\_By\_Site/

&#x20;   |   |-- CPL/

&#x20;   |   |-- PI1/

&#x20;   |   |-- PI2/

&#x20;   |   |-- PI3/

&#x20;   |   `-- PI4/

&#x20;   |-- Program\_Config\_By\_Site/

&#x20;   |   |-- CPL/

&#x20;   |   |-- PI1/

&#x20;   |   |-- PI2/

&#x20;   |   |-- PI3/

&#x20;   |   `-- PI4/

&#x20;   |-- Properties/  \[3 cs]

&#x20;   |   `-- DataSources/

&#x20;   |-- Resources/

&#x20;   |-- Setting AutoUpdate/

&#x20;   |   |-- CPL/

&#x20;   |   |-- PAT2/

&#x20;   |   |-- PI/

&#x20;   |   |-- PI2/

&#x20;   |   `-- PI4/

&#x20;   `-- SqlServerTypes/  \[1 cs]

```



\### สิ่งที่อ่านออกจากทรี



\- ทุกแอปมีชั้น `UI` / `Presenter` / `Service` / `AppController` / `ViewModel` ของตัวเอง — \*\*คัดลอกกันมา

&#x20; ไม่ได้แชร์ไลบรารีกลาง\*\* จึงมีสำเนา 10 ชุด

\- `Program\_Config\_By\_Site/<SITE>/` และ `AutoUpdate\_Setting\_By\_Site/<SITE>/` เป็น payload สำหรับ deploy

\- `Innovation.ProductionManagement` และ `Innovation.InventoryManagement` แบ่งตาม\*\*ฟีเจอร์\*\*

&#x20; (`<Feature>/{UI,Presenter,Service}`) ต่างจากตัวอื่นที่แบ่งตาม\*\*ชั้น\*\*

\- `KB\_PLC\_Control` มีฟอร์มวางแบนอยู่ที่ราก ไม่มีชั้นอะไรเลย



\---



\## 3. สัญญาหลักของ MVP



interface ห้าตัวนี้ \*\*เหมือนกันทุกตัวอักษรในทุกแอป\*\* ต่างกันแค่ namespace

และ\*\*แยกอยู่คนละไฟล์ ไฟล์ละหนึ่ง interface\*\* (ตัวอย่างจาก `Innovation.TotalWeight\_PLC`):



```

Interfaces/Views/IView.cs                   → IView<TPresenter>

Interfaces/Views/IChildView.cs              → IChildView<TPresenter, TParentPresenter>

Interfaces/Presenters/IPresenter.cs         → IPresenter<TView>

Interfaces/Presenters/IGeneralViewPresenter.cs → IGeneralViewPresenter<TView>

Interfaces/Presenters/IChildViewPresenter.cs   → IChildViewPresenter<TView, TParentPresenter>

```



```csharp

public interface IView<TPresenter>

{

&#x20;   TPresenter Presenter { set; }     // setter อย่างเดียว

&#x20;   void Run();

}



public interface IChildView<TPresenter, TParentPresenter> : IView<TPresenter>

{

&#x20;   IView<TParentPresenter> ParentView { set; }

}



public interface IPresenter<TView>

{

&#x20;   TView View { get; }               // getter อย่างเดียว

}



public interface IGeneralViewPresenter<TView> : IPresenter<TView>

{

&#x20;   void Run();

}



public interface IChildViewPresenter<TView, TParentPresenter> : IPresenter<TView>

{

&#x20;   void Run(IView<TParentPresenter> parentView);

}

```



interface ต่อหน้าจอสืบทอดจากคู่นี้:



```csharp

public interface IView\_Login : IView<IPresenter\_Login>

{

&#x20;   string Username { get; set; }

&#x20;   string Password { get; set; }

}



public interface IPresenter\_Login : IGeneralViewPresenter<IView\_Login>

{

&#x20;   LoginVM GetProgramAndProgramVersion();

&#x20;   void GetCompany();

&#x20;   bool CheckUsernamePassword();

&#x20;   void GetUserPermission();

}

```



\### `IApplicationController` — จุดเดียวที่ presenter ถูก resolve



เป็น\*\*ลิสต์แบนของเมธอด `RunXxx()`\*\* ไม่มีโครงสร้างอื่น ขนาดโตตามแอป

(`Innovation.MasterData` มีประมาณ 40 สมาชิก)



\### `IViewBase` — มีเฉพาะใน `Innovation.TotalWeight\_PLC`



```csharp

public interface IViewBase

{

&#x20;   void ShowMessage(string message, AppMessageType type = AppMessageType.Warning);

&#x20;   bool ShowConfirm(string message);

&#x20;   void ShowConfirm(string message, Action onConfirm, Action onCancel = null);

&#x20;   void CloseDialog(DialogResult result);

}

```



View ประกอบสอง interface เข้าด้วยกัน:

`public interface IView\_TotalWeight : IViewBase, IView<IPresenter\_TotalWeight>`



แอปอื่นไม่มีตัวนี้ — presenter เรียก `MessageBox.Show(...)` ตรงๆ จากใน presenter



\---



\## 4. ตารางความต่าง — รูปแบบเดียวกัน สะกดโฟลเดอร์คนละอย่าง



| หน้าที่ | `TotalWeight\_PLC` | `DrawingReturn` | `MasterData` |

|---|---|---|---|

| base view interface | `Interfaces/Views/` | `UI/Interfaces/` | `UI/Interface/` |

| view interface ต่อหน้าจอ | `UI/Interfaces/` | `UI/Interfaces/` | `UI/Interface/` |

| ฟอร์ม | `UI/Implementations/` | `UI/Implements/` | `UI/Implements/` |

| base presenter interface | `Interfaces/Presenters/` | `Presenters/Interfaces/` | `Presenter/Interface/` |

| presenter | `Presenter/Implementations/` | `Presenters/Implements/` | `Presenter/Implements/` |

| service | `Service/Interfaces` + `Implementations` | `Services/Interfaces` + `Implements` | `Service/Interface` + `Implements` |

| controller | `Controllers/` | `AppController/Interfaces` + `Implements` | `AppController/Interface` + `Implements` |

| ของเพิ่ม | `Common/` `Helpers/` `Tracing/` `Prompt/` | — | `Const/` `Global/` `Helper/` `Validator/` |



แกนความต่าง: `Presenter` ↔ `Presenters` · `Service` ↔ `Services` · `Interface` ↔ `Interfaces` ·

`Implements` ↔ `Implementations` (SCADAReportViewer ใช้เอกพจน์ `Implement`/`Interface`) ·

`Controllers` ↔ `AppController` · `UI/` ↔ `Forms/` (BomReplace)



\*\*ทั้งหมดนี้คือรูปแบบเดียวกัน สะกดต่างกัน\*\* — เป็นผลของการคัดลอกโปรเจกต์ต่อๆ กันมาโดยไม่มีเทมเพลตกลาง



\---



\## 5. ตัวอย่างฟีเจอร์ครบวงจร — Login ใน `Innovation.DrawingReturn`



เลือกแอปนี้เพราะเล็กและสะอาดที่สุด (42 ไฟล์ 3 ฟอร์ม)



```

Program.Main()

&#x20; └─ ConfigureServices()                       สร้าง ServiceProvider

&#x20; └─ app.RunLogin()                            IApplicationController

&#x20;      └─ Presenter\_Login (ctor)               \_iView = iView; \_iView.Presenter = this;

&#x20;           └─ frmLogin.Run() => ShowDialog()  ← message loop เกิดที่นี่

&#x20;                └─ CheckApplicationVersion()

&#x20;                     └─ Service.GetProgramAndProgramVersion()   HTTP

&#x20;                └─ btnOk\_Click

&#x20;                     └─ Presenter.CheckUsernamePassword()

&#x20;                          └─ Service\_UserAuthentication          ตรวจ AD ที่เครื่อง

&#x20;                     └─ Presenter.GetUserPermission()            HTTP

&#x20;                └─ app.RunMain(ApplicationUserObj)

```



การตรวจรหัสผ่านเกิดที่เครื่อง client ไม่ได้ส่งรหัสผ่านไป API:



```csharp

using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "company.local"))

{

&#x20;   isValid = pc.ValidateCredentials(username, password);

}

```



API ถูกเรียกเพื่อขอ\*\*สิทธิ์\*\*เท่านั้น (ดูรายละเอียดฝั่งเซิร์ฟเวอร์ที่ Backend ROADMAP §9)



\---



\## 5b. รูปแบบข้ามชั้นของฝั่ง client — ส่วนที่สำคัญที่สุดของเอกสารนี้



กลไกสี่อย่างต่อไปนี้อธิบายพฤติกรรมเกือบทั้งหมดที่บันทึกไว้ใน `RUNTIME\_TEST\_CHECKLIST.md`



\### 5b.1 `BaseForm` และ `RunSafeAsync`



`Innovation.TotalWeight\_PLC/UI/Implementations/BaseForm.cs` (257 บรรทัด)



```csharp

public class BaseForm : DevExpress.XtraEditors.XtraForm, IViewBase

{

&#x20;   public void ShowMessage(string message, AppMessageType type = AppMessageType.Warning)

&#x20;   {

&#x20;       switch (type)

&#x20;       {

&#x20;           case AppMessageType.Information: MessageBoxHelper.ShowInformation(message, "Information"); break;

&#x20;           case AppMessageType.Error:       MessageBoxHelper.ShowError(message, "Error"); break;

&#x20;           default:                         MessageBoxHelper.ShowWarning(message, "Warning"); break;

&#x20;       }

&#x20;   }



&#x20;   public void CloseDialog(DialogResult result) { this.DialogResult = result; this.Close(); }



&#x20;   protected virtual void ShowWarning(string message)

&#x20;   {

&#x20;       CallTracer.ScheduleScreenshot();          // เก็บภาพหน้าจอก่อนขึ้น dialog เสมอ

&#x20;       ShowMessage(message, AppMessageType.Warning);

&#x20;   }

}

```



ตัวห่อ async ที่ event handler ทุกตัวต้องผ่าน:



```csharp

private static bool \_caseActive;    // ← static บน form base class



protected async Task RunSafeAsync(string context, Func<Task> action, Action onFinally = null, bool showProgress = true)

{

&#x20;   bool ownsCase = !\_caseActive;

&#x20;   if (ownsCase) { \_caseActive = true; CallTracer.StartCase(context); }

&#x20;   try

&#x20;   {

&#x20;       if (showProgress) ProgressWaitingFormHelper.ShowForm();

&#x20;       await action();

&#x20;       if (ownsCase) CallTracer.EndCase(true);

&#x20;   }

&#x20;   catch (WebException ex) when (ex.Response is HttpWebResponse r \&\& r.StatusCode == HttpStatusCode.InternalServerError)

&#x20;   {

&#x20;       log4net.LogManager.GetLogger(GetType()).Error(context, ex);

&#x20;       CallTracer.Record("UI", context, $"InternalServerError: {ex.Message}");

&#x20;       CallTracer.ScheduleScreenshot();

&#x20;       CustomDetailMessageBox.ShowCannotLoadFormBecauseInternalServerErrorMessage($"{context} : ", ex.Message);

&#x20;       if (ownsCase) CallTracer.EndCase(false, ex.Message);

&#x20;   }

&#x20;   catch (WebException ex) { /\* ...ShowCannotLoadFormBecauseCannotConnectApiErrorMessage... \*/ }

&#x20;   catch (Exception ex)

&#x20;   {

&#x20;       log4net.LogManager.GetLogger(GetType()).Error(context, ex);

&#x20;       CallTracer.ScheduleScreenshot();

&#x20;       CustomDetailMessageBox.ShowCannotLoadFormErrorMessage($"{context} : ", ex.Message);

&#x20;       if (ownsCase) CallTracer.SavePartialCase("Unhandled | " + ex.Message);

&#x20;   }

&#x20;   finally

&#x20;   {

&#x20;       if (ownsCase) \_caseActive = false;

&#x20;       onFinally?.Invoke();

&#x20;       if (showProgress) ProgressWaitingFormHelper.CloseForm();

&#x20;   }

}

```



\*\*สามข้อสังเกต\*\*



1\. `\_caseActive` เป็น \*\*static mutable state บนคลาสฐานของฟอร์ม\*\* จำเป็นเพราะ `CallTracer` ไม่มี stack

&#x20;  จึงต้องให้เฉพาะตัวนอกสุดเป็นเจ้าของ `StartCase`/`EndCase` ใช้ได้เพราะแอปนี้เป็นสถานีเดียว

&#x20;  เธรดเดียว แต่ผิดทันทีถ้ามีหลายหน้าต่างทำงานพร้อมกัน

2\. `BaseForm` \*\*รู้เรื่อง PLC\*\* — มี `TryGetStep(int id)` และ `TryGetStepParam(...)` สำหรับหาค่า

&#x20;  `SendStepParameter` ทั้งที่เป็นคลาสฐานของ UI ไม่ควรรู้เรื่องฮาร์ดแวร์

3\. \*\*handler แบบ synchronous ที่ไม่ผ่าน `RunSafeAsync` คือแหล่งบั๊กที่รู้กันอยู่\*\* —

&#x20;  `RUNTIME\_TEST\_CHECKLIST.md` §K ระบุว่า `Label48\_Click` → `RunPLCTest()` ไม่ได้ถูกห่อ

&#x20;  จึงสงสัยว่า exception ถูกกลืนหายไปเงียบๆ



\### 5b.2 `HandleData` / `NotFound` / `Found` — ต้นเหตุของบั๊กส่วนใหญ่



presenter รายงานผลผ่าน sink ตัวเดียวบน view:



```csharp

private void NotFound(string funcName, string message = "ข้อมูล")

&#x20;   => \_view.HandleData("Presenter", GetFormName(), funcName, message, true, false);



private void Found(string funcName, string message = "")

&#x20;   => \_view.HandleData("Presenter", GetFormName(), funcName, message, false, false);

```



`HandleData(layer, formName, funcName, message, found, close)`



> \*\*ปัญหาเชิงออกแบบ\*\*: เดิม `NotFound` ทำ\*\*สองอย่างพร้อมกัน\*\* — ขึ้นข้อความ

> "ไม่พบ … กรุณาติดต่อ ICT" \*\*และปิดฟอร์ม\*\* การผูก "รายงาน" ไว้กับ "ปิด" คือสาเหตุของรายการแก้บั๊ก

> เกือบทั้งหมดใน `RUNTIME\_TEST\_CHECKLIST.md` §H, §I, §J



ผลที่ตามมาคือโค้ดปัจจุบันเต็มไปด้วยคอมเมนต์เตือนไม่ให้เรียก กระจายอยู่ใน presenter อย่างน้อย 6 ตัว:



```csharp

// Presenter\_PassReset.cs

// outcome. Do NOT call HandleData/NotFound (that pops "…ติดต่อ ICT" which Delphi never …



// Presenter\_SelectKB.cs

// NotFound (wrong "ไม่พบ … ติดต่อ ICT" + close) or CloseDialog here; just return.



// Presenter\_CheckTTW.cs

// empty); do NOT NotFound-close on zero rows (the View shows blank fields on empty).

```



รายการที่ได้รับผลกระทบ: `Presenter\_LoginLine`, `Presenter\_PassReset`, `Presenter\_PasswordMaxMin`,

`Presenter\_PasswordSUP1`, `Presenter\_SaveTotal`, `Presenter\_CheckTTW`, `Presenter\_SelectKB`



\### 5b.3 ธง boolean แทน exception



service ไม่โยน exception สำหรับกรณี "หาไม่เจอตามคาด" แต่คืน response VM ที่มีธง `bool` แยกรายกรณี

ประมาณ 30 ตัว แล้ว presenter ไล่ `if` ทีละตัว:



```csharp

public async Task GetKanban()

{

&#x20;   var context = GetContext();

&#x20;   CallTracer.Record("Presenter", nameof(GetKanban), $"IsNext\_Plan\_Manual={\_view.IsNext\_Plan\_Manual}");

&#x20;   bool isValid = \_view.IsNext\_Plan\_Manual ? ValidateManual(context) : ValidateNormal(context);

&#x20;   if (!isValid) return;



&#x20;   var res = await \_service.GetKanban(\_view.req);

&#x20;   if (res == null) return;                        // ← ผลลัพธ์ที่สาม แบบเงียบ



&#x20;   if (res.HasNoSetting)

&#x20;   {

&#x20;       NotFound(context, $"Setting StationId : {\_view.req.KBNormal.StationId}");

&#x20;       \_view.ShowMessage($"ไม่พบข้อมูล Setting สำหรับ Station ID: {\_view.req.KBNormal.StationId}");

&#x20;       \_view.CloseDialog(DialogResult.OK);

&#x20;       return;

&#x20;   }

&#x20;   if (res.HasNoModuleId)

&#x20;   {

&#x20;       NotFound(context, $"KBFilterRules ModuleId : {\_view.req.KBNormal.ModuleId}");

&#x20;       \_view.ShowMessage($"ไม่พบข้อมูล KBFilterRules สำหรับ Module ID: {\_view.req.KBNormal.ModuleId}");

&#x20;       \_view.CloseDialog(DialogResult.OK);

&#x20;       return;

&#x20;   }



&#x20;   Found(context, $"IsNext\_Plan\_Manual : {\_view.IsNext\_Plan\_Manual}");

&#x20;   \_view.LstKanbanDatas = res.LstKanbanDatas;

}

```



ธงที่พบ:



```

HasNoSetting          HasNoModuleId          HasRmAutomation        IsAlreadyInTotalWeight

IsEmptyRmBal          IsEmptySiloApprove     IsEmptyKbTogether      IsEmptyWeighting

IsEmptyForId          IsEmptyKbBarcode       IsLockedOrExpired      IsBalWtLessThanWeightH

IsNotEqualSiloId      IsNotEqualLineName     IsNotEqualKbBarcode    IsNotEqualOwnerSiteId

IsTotalWeightExists   IsChkManualNextPlan    IsNext\_Plan\_Manual     IsMixingCheck

IsKbTogether          IsRmConfirm            IsFirstOrder           IsStartMixingTemp

IsManual              IsInsert               IsOil                  IsSave (ปรากฏ 8 ครั้ง)

```



> \*\*สำคัญ\*\*: ฝั่ง backend ประกาศธง\*\*ชุดเดียวกันเป๊ะ\*\*ใน `Innovation.Core/DtoModel/TotalWeightPlc/\*Dto`

> นี่จึงไม่ใช่นิสัยของฝั่ง client แต่เป็น \*\*contract ข้ามฝั่งที่ตั้งใจออกแบบ และดูแลด้วยมือทั้งสองข้าง\*\*

> การเพิ่มกรณีความล้มเหลวใหม่หนึ่งกรณีต้องแก้สามที่: Dto, VM, และ presenter ที่ไล่ `if`

> (ดู Backend ROADMAP §7b.1)



\### 5b.4 Controller ใช้ view เป็นช่องส่งพารามิเตอร์



```csharp

public KanbanDatasVM RunSelectKB(KanbanRequestVM req)

{

&#x20;   var presenter = \_provider.GetRequiredService<IPresenter\_SelectKB>();

&#x20;   presenter.View.LineId      = req.LineId;        // เขียนพารามิเตอร์ลง property ของ view

&#x20;   presenter.View.SHOWKBONLINE = req.SHOWKBONLINE;

&#x20;   presenter.View.SiteId      = req.SiteId;

&#x20;   presenter.View.req         = req;

&#x20;   presenter.Run();                                 // บล็อกที่ ShowDialog

&#x20;   var result = presenter.View.objKb;               // อ่านผลลัพธ์กลับจาก view

&#x20;   return result;

}

```



นี่คือเหตุผลที่ view มี public mutable property จำนวนมาก และเป็นเหตุผลที่ทดสอบ view แยกไม่ได้



\### 5b.5 รูปร่างของ presenter



`Presenter\_TotalWeight` (625 บรรทัด) เป็นตัวแทนที่ดี:



```csharp

public class Presenter\_TotalWeight : IPresenter\_TotalWeight

{

&#x20;   private readonly IService\_TotalWeightPlc \_service;

&#x20;   private readonly IView\_TotalWeight \_view;

&#x20;   public IView\_TotalWeight View => \_view;



&#x20;   public Presenter\_TotalWeight(IView\_TotalWeight view, IService\_TotalWeightPlc service)

&#x20;   {

&#x20;       \_view = view ?? throw new ArgumentNullException(nameof(view));

&#x20;       \_service = service ?? throw new ArgumentNullException(nameof(service));

&#x20;       \_view.Presenter = this;                     // ← ผูกกลับ เกิดเป็นวง

&#x20;   }

&#x20;   public void Run() => \_view.Run();



&#x20;   private string GetFormName() => this.GetType().Name;

&#x20;   private string GetContext(\[CallerMemberName] string methodName = "") => methodName;



&#x20;   private bool HasSiteId(string funcName)

&#x20;   {

&#x20;       if (\_view.SiteId <= 0)

&#x20;       {

&#x20;           \_view.HandleData("Presenter", GetFormName(), funcName, "Site ID");

&#x20;           return false;

&#x20;       }

&#x20;       return true;

&#x20;   }

}

```



รูปแบบที่ใช้ซ้ำ: guard ต่อเงื่อนไขที่คืน `bool` และรายงานผ่าน `HandleData`,

`\[CallerMemberName]` เพื่อติดป้ายชื่อเมธอดอัตโนมัติ, และ `\_view.Presenter = this` ที่สร้างวงอ้างอิง



\---



\## 6. Composition root



ทุกแอปที่เป็น MVP ใช้รูปแบบเดียวกัน — `Microsoft.Extensions.DependencyInjection`



```csharp

public static IServiceProvider ServiceProvider { get; private set; }



\[STAThread]

static void Main()

{

&#x20;   Application.EnableVisualStyles();

&#x20;   Application.SetCompatibleTextRenderingDefault(false);

&#x20;   ConfigureServices();

&#x20;   var app = ServiceProvider.GetService<IApplicationController>();

&#x20;   app.RunLogin();

}



private static void ConfigureServices()

{

&#x20;   var services = new ServiceCollection();

&#x20;   // --- Service ---

&#x20;   services.AddTransient<IService\_DrawingReturn, Service\_DrawingReturn>();

&#x20;   // --- Presenter ---

&#x20;   services.AddTransient<IPresenter\_Login, Presenter\_Login>();

&#x20;   // --- UI ---

&#x20;   services.AddTransient<IView\_Login, frmLogin>();

&#x20;   services.AddSingleton<IApplicationController, ApplicationController>();

&#x20;   ServiceProvider = services.BuildServiceProvider();

}

```



> \*\*`Application.Run()` ไม่เคยถูกเรียก\*\* — message loop คือ `ShowDialog()` ของฟอร์มล็อกอิน

> และหน้าจอถัดๆ ไปคือ `ShowDialog` ซ้อนอยู่ข้างในอีกที



\### ความต่างของ `Innovation.TotalWeight\_PLC`



1\. ตั้งค่า log4net และ hook exception ระดับ AppDomain (ตัว hook มีตัวว่างเปล่า)

&#x20;  ```csharp

&#x20;  var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));

&#x20;  log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);

&#x20;  ```

2\. ลงทะเบียนฟอร์มที่อยู่ยาวเป็น \*\*singleton\*\* (`IView\_TotalWeight`, `IView\_PLCTest`, `IView\_CheckTTW`)

3\. \*\*ผูกฟอร์มสอง singleton เข้าหากันด้วยมือ\*\* เพราะกราฟเป็นวง

&#x20;  ```csharp

&#x20;  var plc = ServiceProvider.GetRequiredService<IView\_PLCTest>();

&#x20;  var tw  = ServiceProvider.GetRequiredService<IView\_TotalWeight>();

&#x20;  plc.ViewTotalWeight = tw;

&#x20;  tw.plc = plc;

&#x20;  CallTracer.RegisterMainForm(tw as Form);

&#x20;  ```

4\. มี factory registration สำหรับชนิดรูปธรรม เพราะ `frmShowAutoFeed` ต้องการฟอร์มตัวจริง ไม่ใช่ interface

&#x20;  ```csharp

&#x20;  services.AddSingleton(sp => (frmTotalWeight)sp.GetRequiredService<IView\_TotalWeight>());

&#x20;  services.AddSingleton(sp => (frmPLCTest)sp.GetRequiredService<IView\_PLCTest>());

&#x20;  ```



\### วิธี resolve presenter — สองสำนัก



```csharp

// DrawingReturn / MasterData — service locator แบบ static ผ่าน Program

var presenter = Program.ServiceProvider.GetService(typeof(IPresenter\_CustomerMain))

&#x20;               as IChildViewPresenter<IView\_CustomerMain, IPresenter\_MasterDataMain>;

presenter.Run(parentView);



// TotalWeight\_PLC — inject IServiceProvider ทาง constructor

private readonly IServiceProvider \_provider;

public ApplicationController(IServiceProvider provider) { \_provider = provider; }

var presenter = \_provider.GetRequiredService<IPresenter\_TotalWeight>();

```



`TotalWeight\_PLC` ยังมีกลไก resolve ตัวที่สามซ้อนอยู่อีก — `Helpers/PresenterLocator.cs`

เป็น `static Dictionary<Type, object>` พร้อม `Register<T>`/`Resolve<T>` ทำงานคู่ขนานกับ DI container



\### ประตูล็อกอินตอนเปิดโปรแกรม



| แอป | เมธอดที่เรียก | มีล็อกอินก่อนไหม |

|---|---|---|

| BomReplace, DrawingReturn, InventoryManagement, LossPrevention, MasterData, ProductionManagement, SCADAReportViewer | `app.RunLogin()` | มี |

| TotalWeight\_PLC | `app.RunTotalWeight()` | \*\*ไม่มี\*\* — เข้าหน้าทำงานเลย ยืนยันตัวตนเมื่อจำเป็น |

| RM\_Confirm | `app.RunMain()` | \*\*ไม่มี\*\* |

| KB\_PLC\_Control | `Application.Run(new Main())` | ไม่มีใน `Program.cs` |



\---



\## 6b. การเรียก API — มีสองรุ่น



\### รุ่นที่ 1 — `WebClient` + `NameValueCollection` (แอปส่วนใหญ่)



```csharp

public class Service\_DrawingReturn : IService\_DrawingReturn

{

&#x20;   WebClient client;

&#x20;   NameValueCollection param;



&#x20;   private void GenerateWebclient()

&#x20;   {

&#x20;       client = new ExtendedWebClient();

&#x20;       client.BaseAddress = Properties.Settings.Default.ApiAddress;

&#x20;       client.Headers\["Accept"] = "application/json";

&#x20;       client.Headers\["Content-Type"] = "application/json";

&#x20;       client.Encoding = Encoding.UTF8;

&#x20;   }



&#x20;   public ChangeStatusResultVM SaveReturnRm(List<KbBarcodeVM> ItemsReturnLst, int OperationSiteId, int ReviseBy)

&#x20;   {

&#x20;       GenerateWebclient();

&#x20;       param = new NameValueCollection();

&#x20;       param.Add("OperationSiteId", OperationSiteId.ToString());

&#x20;       param.Add("ReviseBy", ReviseBy.ToString());

&#x20;       client.QueryString = param;

&#x20;       string json = JsonConvert.SerializeObject(ItemsReturnLst);

&#x20;       string result = client.UploadString("SaveReturnRm", json);

&#x20;       return JsonConvert.DeserializeObject<ChangeStatusResultVM>(result);

&#x20;   }

}

```



อ่าน = `client.DownloadString("ActionName")` เขียน = `client.UploadString("ActionName", json)`

ชื่อ action เป็น string ต่อท้าย `BaseAddress` ทั้งแอปจึงคุยกับ controller ตัวเดียว



การขยาย timeout:



```csharp

public class ExtendedWebClient : WebClient

{

&#x20;   protected override WebRequest GetWebRequest(Uri uri)

&#x20;   {

&#x20;       int requestTimeOut = DrawingReturn.Properties.Settings.Default.RequestTimeOutMinute;

&#x20;       WebRequest w = base.GetWebRequest(uri);

&#x20;       w.Timeout = requestTimeOut \* 60 \* 1000;

&#x20;       return w;

&#x20;   }

}

```



> ไฟล์นี้อยู่ใน `Innovation.DrawingReturn` แต่ประกาศ `namespace Innovation.ProductionManagement.Service.Implements`

> — ร่องรอยการคัดลอกโปรเจกต์



\### รุ่นที่ 2 — `HttpClient` แบบ static (มีเฉพาะ `TotalWeight\_PLC`)



```csharp

private static readonly HttpClient \_httpClient = new HttpClient

{

&#x20;   BaseAddress = new Uri(Properties.Settings.Default.apiAddress.TrimEnd('/') + "/")

};

private static readonly JsonSerializer \_jsonSerializer = JsonSerializer.CreateDefault();



private Task<T> GetQueryAsync<T>(string action, params (string key, object value)\[] parameters)

&#x20;   => QueryAsync<T>(HttpMethod.Get, action, parameters);



private Task<T> PostJsonAsync<T>(string action, object requestBody)

{

&#x20;   var requestJson = JsonConvert.SerializeObject(requestBody);

&#x20;   var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

&#x20;   return SendAsync<T>(HttpMethod.Post, action, content, requestJson);

}



private async Task<T> SendAsync<T>(HttpMethod method, string url, HttpContent content, string requestBodyForLog)

{

&#x20;   using (var response = await \_httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))

&#x20;   {

&#x20;       if (!response.IsSuccessStatusCode)

&#x20;       {

&#x20;           var errorBody = await response.Content.ReadAsStringAsync();

&#x20;           log.Error(logMsg);

&#x20;           throw new HttpRequestException($"API {method} {url} returned {(int)response.StatusCode}: {errorBody}");

&#x20;       }

&#x20;       using (var stream = await response.Content.ReadAsStreamAsync())

&#x20;       using (var sr = new StreamReader(stream))

&#x20;       using (var jr = new JsonTextReader(sr))

&#x20;       {

&#x20;           if (!await jr.ReadAsync()) return default(T);

&#x20;           return \_jsonSerializer.Deserialize<T>(jr);

&#x20;       }

&#x20;   }

}

```



เมธอดสาธารณะย่อเหลือบรรทัดเดียว จัดกลุ่มด้วยคอมเมนต์\*\*ตามฟอร์มที่เรียก\*\*:



```csharp

// frmInputBarcode \*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*

public Task<PreviewFormInputBarcodeResponseVM> PreviewFormInputBarcode(PreviewFormInputBarcodeRequestVM req)

&#x20;   => PostJsonAsync<PreviewFormInputBarcodeResponseVM>("PreviewFormInputBarcode", req);



// frmUserLogin \*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*\*

public Task<UsrWtResponseVM> GetUsrWt(int siteId, string appId, string username, string password)

&#x20;   => PostQueryAsync<UsrWtResponseVM>("GetUsrWt",

&#x20;       ("siteId", siteId), ("appId", appId), ("username", username), ("password", password));

```



`Service\_TotalWeightPlc` เรียก \*\*57 endpoint\*\* ผ่านเมธอดสาธารณะ \*\*68 ตัว\*\*

คู่ฝั่งเซิร์ฟเวอร์คือ `TotalWeightPlcController` (358 บรรทัด 52 action) ซึ่งจัดกลุ่มตามฟอร์มเช่นกัน



\### ที่มาของ base URL



\*\*เสมอ\*\* คือ `Properties.Settings.Default.ApiAddress` ซึ่งมาจาก section `userSettings` ของ `App.config`

ไม่เคยใช้ `appSettings` และไม่เคย hard-code:



```xml

<setting name="ApiAddress" serializeAs="String">

&#x20;   <value>http://10.0.0.x:20000/api/DrawingReturn/</value>

</setting>

```



> `KB\_PLC\_Control` เป็นข้อยกเว้น — ไม่มีชั้น HTTP เลย ต่อ SQL Server ตรงผ่าน typed DataSet

> ใน assembly `KB\_Control\_DAO` โดยมี connection string อยู่ในไฟล์ config



\---



\## 6c. การ deploy รายไซต์



`Program\_Config\_By\_Site/<SITE>/<App>.exe.config` และ `AutoUpdate\_Setting\_By\_Site/<SITE>/setting.ini`

เป็น \*\*payload สำหรับตอน deploy ไม่ใช่โค้ดตอนรัน\*\* — ประกาศใน `.csproj` เป็น `<None Include=...>`

จึงไม่ถูกคัดลอกไป output



```xml

<None Include="AutoUpdate\_Setting\_By\_Site\\CPL\\setting.ini" />

<None Include="AutoUpdate\_Setting\_By\_Site\\PI1\\setting.ini" />

```



\### ความต่างระหว่างไซต์มีแค่สองบรรทัด



diff ระหว่าง `PI1` กับ `PI2` ของ `Innovation.MasterData` ได้ผลเพียงเท่านี้:



| ไซต์ | API host | `SITE\_ID\_FOR\_AUTO\_UPDATE` | `setting.ini` `NAMESERVER` |

|---|---|---:|---|

| PI1 | `10.0.0.x:20000` | 11 | `DB-SERVER-A` |

| PI2 | `10.0.0.x:20000` | 12 | `DB-SERVER-B\\INSTANCE` |

| PI3 | `10.0.0.x:20000` | 13 | `DB-SERVER-A` |

| PI4 | `10.0.0.x:20000` | 14 | `DB-SERVER-B\\INSTANCE2` |

| BKK / CPL | `10.0.0.x:20000` | ตามแอป | ตามแอป |



ที่เหลือ — `ProgramCode`, `RequestTimeOutMinute`, `APP\_ID\_FOR\_AUTO\_UPDATE`, บล็อกตั้งค่า D365/ADFS,

ค่า skin — \*\*เหมือนกันทุกไบต์ทั้งหกไซต์\*\* แกนความต่างรายไซต์จึงมีแค่ "API เครื่องไหน" กับ "แถวไหนในฐานอัปเดต"



```ini

\[SERVERNAME]

NAMESERVER=DB-SERVER-A



\[DATABASENAME]

cnCenter\_Update=Center\_Update

cnCentralDB=CentralDB



\[APP\_PROGRAM]

APP\_ID=ERP0xx

```



\### แอปรู้ได้อย่างไรว่าอยู่ไซต์ไหน — คำตอบคือไม่รู้



\*\*ไม่มีการตรวจจับไซต์ตอนรันเลย\*\* มีสามกลไกและไม่มีอันไหน probe สภาพแวดล้อม:



1\. \*\*ตอนติดตั้ง\*\* — คัดลอกไฟล์ config ของไซต์นั้นทับลงเครื่อง

2\. \*\*จากผู้ใช้ที่ล็อกอิน\*\* (DrawingReturn, MasterData, InventoryManagement)

&#x20;  ```csharp

&#x20;  \_iView.OperationSiteId = \_iView.ApplicationUserObj?.UserOperationSite.FirstOrDefault()?.Site\_ID ?? 0;

&#x20;  ```

3\. \*\*ฝังไว้ในไฟล์ config ประจำเครื่อง\*\* (TotalWeight\_PLC) — `<setting name="siteID"><value>6</value></setting>`

&#x20;  อ่านด้วย `Properties.Settings.Default.siteID` แล้วส่งเป็นพารามิเตอร์ทุกครั้งที่เรียก service



\### กลไก AutoUpdate



\*\*ตัวอัปเดตอยู่นอก repository นี้\*\* — ไม่มีโค้ด C# ในโครงนี้ที่อ่าน `setting.ini` เลย

โครงสร้างที่อนุมานได้: มี launcher/updater แยกอ่าน `setting.ini` → ต่อ SQL Server ตาม `NAMESERVER`

→ ฐาน `Center\_Update` → หาแถวตาม `APP\_ID` + `SITE\_ID\_FOR\_AUTO\_UPDATE` → แทนที่ไบนารีก่อนเปิดแอป



ครึ่งที่อยู่ในแอปคือ\*\*ประตูตรวจเวอร์ชันบนฟอร์มล็อกอิน\*\*:



```csharp

private void CheckApplicationVersion()

{

&#x20;   RunSafe(() =>

&#x20;   {

&#x20;       ProgressWaitingFormHelper.ShowForm();

&#x20;       var Obj = Presenter.GetProgramAndProgramVersion();

&#x20;       ProgressWaitingFormHelper.CloseForm();

&#x20;       if (Obj.programId == 0)

&#x20;       {

&#x20;           MessageBox.Show($"ไม่พบโปรแกรม Code : {Settings.Default.ProgramCode} นี้...");

&#x20;           Close();

&#x20;       }

&#x20;       else if (string.IsNullOrWhiteSpace(Obj.versionProgram)) { /\* ... \*/ Close(); }

&#x20;   });

}

```





\---



\## 7. ตารางเปรียบเทียบรายแอป



`.cs` ไม่รวม `bin/ obj/ .vs/ packages/` — "DX" คือจำนวนบรรทัด reference ของ DevExpress ใน `.csproj`



| แอป | TFM | DX | `.cs` (Designer) | ฟอร์ม | รูปแบบ |

|---|---|---:|---|---:|---|

| `Innovation.MasterData` | 4.7.2 | 18 | 462 (39) | 37 | แบ่งตามชั้น — ใหญ่ที่สุด |

| `Innovation.ProductionManagement` | 4.7.2 | 111 | 309 (31) | 19 | \*\*แบ่งตามฟีเจอร์\*\* |

| `Innovation.TotalWeight\_PLC` | 4.6.2 | 98 | 256 (28) | 26 | MVP+ (async, HttpClient, `IViewBase`) |

| `Innovation.InventoryManagement` | 4.7.2 | 69 | 212 (63) | 50 | \*\*แบ่งตามฟีเจอร์\*\* + AppController รายฟีเจอร์ |

| `Innovation.SCADAReportViewer` | 4.6.2 | 22 | 85 (13) | 10 | แบ่งตามชั้น ชื่อโฟลเดอร์เอกพจน์ + มี installer |

| `Innovation.RM\_Confirm` | 4.6.2 | 25 | 53 (9) | 7 | แบ่งตามชั้น + เศษ `Form1` ไม่มีล็อกอิน |

| `Innovation.DrawingReturn` | 4.6.2 | 9 | 42 (5) | 3 | \*\*ตัวอย่างอ้างอิง\*\* — สะอาดที่สุด |

| `Innovation.LossPreventionManagement` | 4.7.2 | 6 | 40 (5) | 3 | แบ่งตามชั้น + sln ซ้อน + vendor PM เข้ามา |

| `Innovation.BomReplace` | 4.7.2 | 4 | 23 (4) | 2 | รวม interface ไว้ที่เดียว ไม่มี `IChildView` |

| `KB\_PLC\_Control` | 4.6.2 | 23 | 22 (10) | 8 | \*\*ระบบเก่าแบบแบน\*\* — ไม่มี DI/presenter/service/HTTP |



\### สี่รูปแบบที่พบ



1\. \*\*แบ่งตามชั้น\*\* (`UI/ Presenter/ Service/ AppController/ ViewModel/`) — DrawingReturn, MasterData,

&#x20;  LossPrevention, SCADAReportViewer, RM\_Confirm, TotalWeight\_PLC

2\. \*\*แบ่งตามฟีเจอร์\*\* (`<Feature>/{UI,Presenter,Service\[,AppController]}`) — ProductionManagement,

&#x20;  InventoryManagement — interface ชุดเดียวกัน แต่จัดกลุ่มไฟล์คนละแบบ เป็นวิวัฒนาการตามธรรมชาติ

&#x20;  เมื่อแอปโตเกิน \~200 ไฟล์

3\. \*\*รวม interface ไว้ต้นไม้เดียว\*\* (`Interfaces/{Views,Presenters,Services}` + impl แบน) — BomReplace

4\. \*\*ฟอร์มแบนแบบเก่า\*\* — KB\_PLC\_Control เท่านั้น



\### `KB\_PLC\_Control` — ตัวนอกคอกที่แท้จริง



ไม่มี DI ไม่มี presenter ไม่มี service ไม่มี interface ไม่มี HTTP

`Application.Run(new Main())` ตรงๆ, `Program.cs` ประกาศ `namespace WindowsApplication1`

ขณะที่ไฟล์อื่นใช้ `namespace KB\_PLC\_Control`, มี global state แบบ public static บนฟอร์ม:



```csharp

public static int LoginUserId;

public static int SiteId;

public static string SiteName;

public static int ProgramId;

```



สร้าง MDI child ตรงๆ จาก event handler: `new ProductionReport(); \_frmProReport.MdiParent = this;`



\### สิ่งเดียวที่แชร์กันจริง — DLL สามตัว



ทุกแอป MVP อ้าง DLL เดียวกันผ่าน `HintPath` ไปยังโซลูชันข้างเคียง:



```

..\\..\\Innovation.Core\\Innovation.Core\\bin\\Debug\\netstandard2.0\\Innovation.Core.dll

..\\..\\Innovation.UtilityCore\\Innovation.UtilityCore.CustomDetailMessageBox\\bin\\Debug\\...dll

..\\..\\Innovation.UtilityCore\\Innovation.UtilityCore.Helper\\bin\\Debug\\...dll

```



> \*\*สังเกตว่าเป็น path `bin\\Debug\\`\*\* — build แบบ Release ก็ยังลิงก์ไบนารี Debug

> และ \*\*สัญญา MVP เองไม่เคยถูกย้ายเข้า `Innovation.Core`\*\* จึงมีสำเนาอยู่ 10 ชุด



\---



\## 8. UX / UI



ข้อมูลทั้งหมดสกัดจากไฟล์ `.Designer.cs` 186 ไฟล์ ซึ่งเก็บ layout ระดับพิกเซล:

`ClientSize`, `StartPosition`, `Text`, และต่อ control มี `Location`, `Size`, `Text`,

`Appearance.BackColor` (ARGB), `Font`



\### 8.1 คลังหน้าจอ — 165 ฟอร์ม



| App | Form | ClientSize | Controls |

|---|---|---|---:|

| Innovation.BomReplace | `frmLogin` | 284x261 | 3 |

| Innovation.BomReplace | `frmMain` | 442x449 | 4 |

| Innovation.DrawingReturn | `frmLogin` | 398x268 | 8 |

| Innovation.DrawingReturn | `frmMain` | 966x736 | 5 |

| Innovation.DrawingReturn | `frmReturnRm` | 1498x1099 | 37 |

| Innovation.InventoryManagement | `frmApprovePending` | 979x449 | 26 |

| Innovation.InventoryManagement | `frmCreateReceivePlan` | 1141x510 | 95 |

| Innovation.InventoryManagement | `frmCustomerVendorProfile` | 890x678 | 21 |

| Innovation.InventoryManagement | `frmGeneratePickingTransfer` | 1398x736 | 198 |

| Innovation.InventoryManagement | `frmInterfaceLog` | 1618x919 | 90 |

| Innovation.InventoryManagement | `frmInventInterfaceFG` | 1179x469 | 42 |

| Innovation.InventoryManagement | `frmInventInterfaceReceiveHist` | 1175x449 | 122 |

| Innovation.InventoryManagement | `frmInventInterfaceReceive` | 1386x566 | 129 |

| Innovation.InventoryManagement | `frmInventInterfaceReviewFG` | 1466x560 | 47 |

| Innovation.InventoryManagement | `frmInventInterfaceReviewSe` | 1447x551 | 49 |

| Innovation.InventoryManagement | `frmInventInterfaceTransferHist` | 1598x919 | 54 |

| Innovation.InventoryManagement | `frmInventInterfaceTransfer` | 1156x829 | 153 |

| Innovation.InventoryManagement | `frmInventoryMainMenu` | 1051x670 | 31 |

| Innovation.InventoryManagement | `frmLogin` | 427x268 | 6 |

| Innovation.InventoryManagement | `frmMaterialPlanPicking` | 1398x786 | 146 |

| Innovation.InventoryManagement | `frmMaterialPlanTransfer` | 1398x919 | 241 |

| Innovation.InventoryManagement | `frmMaterialProfile` | 1598x799 | 43 |

| Innovation.InventoryManagement | `frmMaterialReceivePlan` | 1938x1099 | 100 |

| Innovation.InventoryManagement | `frmMaterialRequestTransferToProduction` | 1557x919 | 261 |

| Innovation.InventoryManagement | `frmMaterialRequest` | 1618x919 | 269 |

| Innovation.InventoryManagement | `frmNewLabel` | 448x499 | 21 |

| Innovation.InventoryManagement | `frmPreviewReportPackage` | 598x599 | 25 |

| Innovation.InventoryManagement | `frmPrintReport` | 1162x474 | 1 |

| Innovation.InventoryManagement | `frmQALockDetail` | 1078x533 | 26 |

| Innovation.InventoryManagement | `frmQALock` | 1084x609 | 51 |

| Innovation.InventoryManagement | `frmQAPlan` | 1067x449 | 46 |

| Innovation.InventoryManagement | `frmQCApproveReview` | 1127x562 | 57 |

| Innovation.InventoryManagement | `frmQCCheck` | 1072x483 | 59 |

| Innovation.InventoryManagement | `frmQCLabTest` | 1127x472 | 42 |

| Innovation.InventoryManagement | `frmQCReviewApprove` | 1181x432 | 41 |

| Innovation.InventoryManagement | `frmReportBalance` | 1598x919 | 33 |

| Innovation.InventoryManagement | `frmReportExpire` | 1232x550 | 27 |

| Innovation.InventoryManagement | `frmReportStockByBarcode` | 823x799 | 24 |

| Innovation.InventoryManagement | `frmReportStockCardPackage` | 1133x537 | 18 |

| Innovation.InventoryManagement | `frmReportStockSumary` | 1135x449 | 11 |

| Innovation.InventoryManagement | `frmRequestMaterialReceive` | 1718x776 | 110 |

| Innovation.InventoryManagement | `frmRequestMaterialSaleReturnOrder` | 1127x689 | 7 |

| Innovation.InventoryManagement | `frmRequestMaterialTransferToProduction` | 1938x1099 | 83 |

| Innovation.InventoryManagement | `frmRequestMaterialTransfer` | 1475x711 | 210 |

| Innovation.InventoryManagement | `frmRequestMaterial` | 1432x550 | 136 |

| Innovation.InventoryManagement | `frmRequestReceivePackage` | 1938x1099 | 116 |

| Innovation.InventoryManagement | `frmSelectPurpose` | 782x449 | 16 |

| Innovation.InventoryManagement | `frmShareShipping` | 600x449 | 20 |

| Innovation.InventoryManagement | `frmShippingDocsDetail` | 1418x522 | 37 |

| Innovation.InventoryManagement | `frmShippingDocs` | 1245x919 | 226 |

| Innovation.InventoryManagement | `frmShowBarcode` | 796x268 | 1 |

| Innovation.InventoryManagement | `frmShowDataReceivePlan` | 1413x685 | 69 |

| Innovation.InventoryManagement | `frmShowDialog` | 458x163 | 4 |

| Innovation.InventoryManagement | `frmShowNewLabel` | 598x568 | 1 |

| Innovation.InventoryManagement | `frmVendorProfile` | 890x678 | 22 |

| Innovation.LossPreventionManagement | `frmGainExtraWeight` | 442x449 | 4 |

| Innovation.LossPreventionManagement | `frmLogin` | 398x268 | 9 |

| Innovation.LossPreventionManagement | `frmMain` | 1051x670 | 18 |

| Innovation.MasterData | `frmAlternativeItemNumber` | 671x511 | 22 |

| Innovation.MasterData | `frmCustomerAddEdit` | 1193x725 | 119 |

| Innovation.MasterData | `frmCustomerDetail` | 1618x870 | 300 |

| Innovation.MasterData | `frmCustomerInterface` | 998x639 | 60 |

| Innovation.MasterData | `frmCustomerMain` | 1184x767 | 34 |

| Innovation.MasterData | `frmInterfaceItemMain` | 1184x767 | 195 |

| Innovation.MasterData | `frmInterfaceMain` | 1869x767 | 72 |

| Innovation.MasterData | `frmInterfaceNotification` | 863x573 | 17 |

| Innovation.MasterData | `frmItemDetails` | 1498x899 | 525 |

| Innovation.MasterData | `frmItemMain` | 1184x767 | 48 |

| Innovation.MasterData | `frmLineDetail` | 455x537 | 21 |

| Innovation.MasterData | `frmLineMain` | 935x594 | 28 |

| Innovation.MasterData | `frmLocationMain` | 935x594 | 31 |

| Innovation.MasterData | `frmLogin` | 374x218 | 7 |

| Innovation.MasterData | `frmMachineDetail` | 1193x803 | 109 |

| Innovation.MasterData | `frmMachineMain` | 935x594 | 29 |

| Innovation.MasterData | `frmMachinePropertiesDetail` | 1131x762 | 39 |

| Innovation.MasterData | `frmMachinePropertiesMain` | 929x586 | 25 |

| Innovation.MasterData | `frmMasterDataMain` | 1179x665 | 26 |

| Innovation.MasterData | `frmMeasurementDetail` | 1336x693 | 61 |

| Innovation.MasterData | `frmMeasurementMain` | 985x582 | 27 |

| Innovation.MasterData | `frmMoldDetail` | 1414x919 | 124 |

| Innovation.MasterData | `frmMoldMain` | 1138x581 | 25 |

| Innovation.MasterData | `frmSiloDetail` | 1279x565 | 44 |

| Innovation.MasterData | `frmSiloMain` | 938x556 | 25 |

| Innovation.MasterData | `frmStationDetail` | 1194x712 | 45 |

| Innovation.MasterData | `frmStationMain` | 935x594 | 27 |

| Innovation.MasterData | `frmUnitConversionDetail` | 1032x640 | 41 |

| Innovation.MasterData | `frmUnitConversionMain` | 1102x581 | 51 |

| Innovation.MasterData | `frmUnitsDetail` | 1193x725 | 38 |

| Innovation.MasterData | `frmUnitsMain` | 935x594 | 31 |

| Innovation.MasterData | `frmVendorDetail` | 1638x733 | 267 |

| Innovation.MasterData | `frmVendorMain` | 935x594 | 34 |

| Innovation.MasterData | `frmVendor` | 671x511 | 20 |

| Innovation.MasterData | `frmWaitForm` | 246x73 | 2 |

| Innovation.MasterData | `frmWarehouseDetail` | 1193x725 | 65 |

| Innovation.MasterData | `frmWarehouseMain` | 935x594 | 33 |

| Innovation.ProductionManagement | `frmAddTimeOfLine3` | 598x799 | 28 |

| Innovation.ProductionManagement | `frmApproveInterface` | 1938x1099 | 121 |

| Innovation.ProductionManagement | `frmBom` | 598x449 | 20 |

| Innovation.ProductionManagement | `frmDailyInputData` | 1598x919 | 67 |

| Innovation.ProductionManagement | `frmEditCycleTime` | 564x393 | 10 |

| Innovation.ProductionManagement | `frmInputDailyProductionData` | 998x468 | 50 |

| Innovation.ProductionManagement | `frmInputWorkingTime` | 1618x919 | 48 |

| Innovation.ProductionManagement | `frmInterrupt` | 598x449 | 12 |

| Innovation.ProductionManagement | `frmLogin` | 427x262 | 16 |

| Innovation.ProductionManagement | `frmMainCycleTime` | 1107x449 | 20 |

| Innovation.ProductionManagement | `frmPackingCheckSheet` | 1598x919 | 53 |

| Innovation.ProductionManagement | `frmPreviewReport` | 1598x899 | 21 |

| Innovation.ProductionManagement | `frmProductionLoss` | 998x899 | 22 |

| Innovation.ProductionManagement | `frmProductionMain` | 1051x670 | 22 |

| Innovation.ProductionManagement | `frmProductionOrder` | 798x599 | 25 |

| Innovation.ProductionManagement | `frmVerifyCancel` | 348x118 | 3 |

| Innovation.ProductionManagement | `frmWaitForm` | 246x73 | 2 |

| Innovation.ProductionManagement | `frmWorkingTimeData` | 1598x919 | 20 |

| Innovation.ProductionManagement | `frmWorkingTimeMain` | 698x399 | 10 |

| Innovation.RM\_Confirm | `Form1` | 800x450 | 4 |

| Innovation.RM\_Confirm | `frmCheckWT` | 586x387 | 9 |

| Innovation.RM\_Confirm | `frmHistory` | 1130x499 | 26 |

| Innovation.RM\_Confirm | `frmMain` | 1338x626 | 31 |

| Innovation.RM\_Confirm | `frmSelectKB` | 661x318 | 14 |

| Innovation.RM\_Confirm | `frmSetting` | 800x468 | 14 |

| Innovation.RM\_Confirm | `frmUserConfirm` | 349x140 | 6 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_Loginfrm` | 483x199 | 8 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_Loginfrm` | 483x199 | 8 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_Mainfrm` | 1008x712 | 12 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_Mainfrm` | 1008x712 | 12 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_PreveiwGraphfrm` | 1068x747 | 37 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_PreveiwGraphfrm` | 1938x1068 | 35 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_SearchFormulationfrm` | 408x525 | 9 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_SearchFormulationfrm` | 998x568 | 12 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_SearhDatafrm` | 1008x712 | 66 |

| Innovation.SCADAReportViewer | `SCADA\_Report\_Viewer\_SearhDatafrm` | 1008x712 | 66 |

| Innovation.TotalWeight\_PLC | `frmAddWeight` | 402x271 | 18 |

| Innovation.TotalWeight\_PLC | `frmAutomationMessage` | 648x432 | 5 |

| Innovation.TotalWeight\_PLC | `frmCheckTTW` | 417x286 | 19 |

| Innovation.TotalWeight\_PLC | `frmComPortSetup` | 242x237 | 16 |

| Innovation.TotalWeight\_PLC | `frmDetails` | 442x449 | 4 |

| Innovation.TotalWeight\_PLC | `frmInputBarcode` | 392x426 | 22 |

| Innovation.TotalWeight\_PLC | `frmLoginLine` | 226x70 | 3 |

| Innovation.TotalWeight\_PLC | `frmLogin` | 298x268 | 6 |

| Innovation.TotalWeight\_PLC | `frmMain` | 598x568 | 28 |

| Innovation.TotalWeight\_PLC | `frmMaster` | 626x449 | 8 |

| Innovation.TotalWeight\_PLC | `frmMenu` | 671x598 | 18 |

| Innovation.TotalWeight\_PLC | `frmMonitorAddressPLC` | 571x420 | 19 |

| Innovation.TotalWeight\_PLC | `frmPLCTest` | 674x545 | 132 |

| Innovation.TotalWeight\_PLC | `frmPassReset` | 166x28 | 2 |

| Innovation.TotalWeight\_PLC | `frmPasswordMaxMin` | 182x91 | 3 |

| Innovation.TotalWeight\_PLC | `frmPasswordSUP1` | 298x268 | 6 |

| Innovation.TotalWeight\_PLC | `frmSaveTotal` | 454x198 | 3 |

| Innovation.TotalWeight\_PLC | `frmSelectFactype` | 292x360 | 12 |

| Innovation.TotalWeight\_PLC | `frmSelectKB` | 505x330 | 12 |

| Innovation.TotalWeight\_PLC | `frmSelectLine` | 285x360 | 9 |

| Innovation.TotalWeight\_PLC | `frmSettingMaxMin` | 285x315 | 22 |

| Innovation.TotalWeight\_PLC | `frmShowAutoFeed` | 713x434 | 6 |

| Innovation.TotalWeight\_PLC | `frmTotalWeight` | 843x608 | 164 |

| Innovation.TotalWeight\_PLC | `frmTrayBarcode` | 594x272 | 20 |

| Innovation.TotalWeight\_PLC | `frmUserLogin` | 290x272 | 7 |

| Innovation.TotalWeight\_PLC | `frmcHEcKrm` | 1020x320 | 11 |

| KB\_PLC\_Control | `Check\_SaveData` | 448x84 | 3 |

| KB\_PLC\_Control | `InterruptionTime` | 790x575 | 88 |

| KB\_PLC\_Control | `Interruption\_Report` | 790x579 | 5 |

| KB\_PLC\_Control | `Login` | 344x282 | 13 |

| KB\_PLC\_Control | `Main` | 1016x739 | 21 |

| KB\_PLC\_Control | `ProductionReport` | 790x579 | 23 |

| KB\_PLC\_Control | `SearchProductPlan` | 757x612 | 23 |

| KB\_PLC\_Control | `Status\_KB\_Check` | 790x579 | 42 |



นอกจากฟอร์ม 165 ตัวข้างบน ยังมี \*\*เทมเพลตรายงาน `XtraReport` อีก 21 ไฟล์\*\* ที่เป็น `.Designer.cs`

เหมือนกันแต่ไม่ใช่ฟอร์ม (ไม่มี `ClientSize`) — ส่วนใหญ่อยู่ใน `InventoryManagement`

(`XtraReportBalance`, `XtraReportExpire`, `XtraReportStockByBarcode`, `XtraReportStockCardPackage`,

`XtraReportStockSumary`, `XtraReportDepositSlip`, `XtraReportQaLock`, `XtraReportRawMaterialBill`,

`XtraReportNewLabel`, `BarcodeReport`) และ `ProductionManagement`

(`XtraReportProductionLoss`, `XtraReportDailyOperationCondition`, …)



\### 8.2 แผนที่การนำทาง



`IApplicationController.Run\*()` \*\*คือ\*\* สัญญาการนำทาง อ่านจากที่นี่ที่เดียวได้ทั้งแอป



```mermaid

graph TD

&#x20;   subgraph BomReplace

&#x20;       A1\[RunLogin] --> A2\[RunMain]

&#x20;   end

&#x20;   subgraph DrawingReturn

&#x20;       B1\[RunLogin] --> B2\["RunMain(user)"] --> B3\["ShowRunReturnRm(user, parentView)"]

&#x20;   end

&#x20;   subgraph LossPrevention

&#x20;       C1\[RunLogin] --> C2\["RunMain(user)"] --> C3\["RunGainExtraWeight(user, parentView)"]

&#x20;   end

&#x20;   subgraph RM\_Confirm

&#x20;       D1\[RunMain<br/>ไม่มีล็อกอิน] --> D2\["RunSelectKB\_View()"]

&#x20;       D1 --> D3\[RunSetting]

&#x20;   end

```



\*\*`Innovation.MasterData`\*\* — รูปแบบเด่นคือคู่ \*\*หน้ารายการ (grid) → หน้ารายละเอียด (form)\*\*

ทำซ้ำประมาณ 20 คู่:



```

RunLogin → RunMasterDataMain

&#x20;            ├─ RunCustomerMain(parentView)   → RunCustomerDeatil(id, isInterface)   \[Deatil สะกดผิด]

&#x20;            ├─ RunVendorMain(parentView)     → RunVendorDetail(id)

&#x20;            ├─ RunLocationMain(parentView)   → RunLocationDetail(id, isInterface)

&#x20;            ├─ RunWarehouseMain(parentView)  → RunWarehouseDetail(id)

&#x20;            ├─ RunStationMain(parentView)    → RunStationDetail(id)

&#x20;            ├─ RunMachineMain(parentView)    → RunMachineDetail(id)

&#x20;            ├─ RunLineMain(parentView)       → RunLineDetail(id)

&#x20;            ├─ RunItemMain(parentView)       → RunItemDetail(id)

&#x20;            ├─ RunUnitsMain(parentView)      → RunUnitsDetail(id)

&#x20;            ├─ RunMeasurementMain(...)       → RunMeasurementDetail(id)

&#x20;            ├─ RunSiloMain(...)              → RunSiloDetail(id)

&#x20;            ├─ RunMoldMain(...)              → RunMoldDetail(id)

&#x20;            ├─ RunUnitConversionMain(...)    → RunUnitConversionDetail(id, tabSelect)

&#x20;            ├─ RunMachinePropertiesMain(...) → RunMachinePropertiesDetail(id)

&#x20;            ├─ RunInterfaceVendor(serviceId) / RunImportItemDetail(serviceId)

&#x20;            └─ RunInterfaceNotification(dataList)

```



\*\*`Innovation.ProductionManagement`\*\*



```

RunLogin → RunProductionMain(user, companyDtlId)

&#x20;            ├─ RunApproveInterface(...)   ├─ RunCycleTime(...)      ├─ RunDailyInputData(...)

&#x20;            ├─ RunWorkingTime(...)        ├─ RunInputWorkingTime(...) ├─ RunWorkingTimeData(...)

&#x20;            ├─ RunEditCycleTime(...)      ├─ RunInputDailyProductionData(...)

&#x20;            └─ รายงาน 5 ตัว: RunReportDailyOperationCondition / DailyProduction /

&#x20;                             WeeklyProduction / MonthlyProduction / YearlyProduction

```



\*\*`Innovation.TotalWeight\_PLC` — controller ที่คืนค่า ไม่ใช่แค่ router\*\*



ไม่มีประตูล็อกอิน เข้า `RunTotalWeight()` ตรง แล้วยืนยันตัวตนเมื่อจำเป็น

และเมธอดหลายตัว\*\*คืนค่ากลับ\*\* ทำให้ controller ทำหน้าที่เป็น \*\*dialog broker\*\*:



```csharp

KanbanDatasVM RunSelectKB(KanbanRequestVM req);

LinefileVM    RunLoginLine();

bool          RunPasswordSUP1();

UsrWtResponseVM RunUserLogin();

```



\*\*`KB\_PLC\_Control`\*\* — ไม่มี controller เลย สร้าง MDI child ใน event handler ของ nav bar โดยตรง



\### 8.3 Design tokens



| Token | ค่า | ที่มา |

|---|---|---|

| DevExpress skin | `"DevExpress Style"` | `AppSettings\["ApplicationSkinName"]` + `BonusSkins.Register()` |

| skin สำรอง (KB\_PLC\_Control) | `"Springtime"` | ฝังในโค้ด `Main.cs` |

| สีพื้นแผงหลัก | `Color.FromArgb(234, 230, 223)` | `frmLogin` panelControl1 |

| ปุ่มมาตรฐาน | `75 × 30` | `btnOk` / `btnCancel` |

| ช่องกรอกมาตรฐาน | `156 × 20` | `txtUsername` / `txtPassword` |

| ตำแหน่งเริ่มต้นของ dialog | `FormStartPosition.CenterScreen` | ทุก dialog |



ตั้งค่า skin ก่อน `ConfigureServices()` เสมอ:



```csharp

DevExpress.Skins.SkinManager.EnableFormSkins();

DevExpress.UserSkins.BonusSkins.Register();

UserLookAndFeel.Default.SkinName = ConfigurationManager.AppSettings\["ApplicationSkinName"];

```



\### 8.4 คลังคำศัพท์ control และตัวแทนใน WinForms มาตรฐาน



นับจากการประกาศ field ในไฟล์ `.Designer.cs` ทั้งหมด



| ครั้ง | ชนิด DevExpress | ตัวแทนใน WinForms มาตรฐาน |

|---:|---|---|

| 2,981 | `XtraGrid.Columns.GridColumn` | `DataGridViewColumn` |

| 954 | `XtraEditors.LabelControl` | `Label` |

| 568 | `XtraEditors.TextEdit` | `TextBox` |

| 567 | `XtraReports.UI.XRTableCell` | \*(ต้องหาตัวแทนงานรายงาน)\* |

| 415 | `XtraBars.BarButtonItem` | `ToolStripButton` |

| 285 | `System.Windows.Forms.BindingSource` | เหมือนเดิม |

| 258 | `XtraEditors.LookUpEdit` | `ComboBox` (DropDownStyle=DropDownList) |

| 201 | `XtraGrid.Views.Grid.GridView` | `DataGridView` |

| 195 | `XtraReports.UI.XRLabel` | \*(งานรายงาน)\* |

| 187 | `XtraBars.Ribbon.RibbonPageGroup` | `ToolStrip` / `MenuStrip` |

| 185 | `XtraEditors.GroupControl` | `GroupBox` |

| 138 | `XtraGrid.GridControl` | `DataGridView` |

| 127 | `XtraVerticalGrid.Rows.EditorRow` | `PropertyGrid` หรือ layout เอง |

| 126 | `System.Windows.Forms.Label` | เหมือนเดิม |

| 117 | `XtraBars.Ribbon.RibbonControl` | `MenuStrip` + `ToolStrip` |

| 111 | `XtraEditors.SimpleButton` | `Button` |

| 111 | `XtraBars.Ribbon.RibbonStatusBar` | `StatusStrip` |

| 110 | `XtraEditors.DateEdit` | `DateTimePicker` |

| 84 | `XtraEditors.PanelControl` | `Panel` |



> ตารางนี้บอกขนาดของงานถ้าจะถอด DevExpress ออก: grid กับ ribbon คือสองส่วนที่หนักที่สุด

> ส่วน `XtraReports` (567 + 195 + …) ต้องหาทางออกเรื่องรายงานต่างหาก



\### 8.5 Wireframe หน้าจอหลัก



สร้างขึ้นใหม่จากค่า `Location` / `Size` / `Text` จริงในไฟล์ Designer

คำบรรยายใต้ภาพระบุ `ClientSize` จริงเสมอ



\#### `Innovation.DrawingReturn / frmLogin` — 398 × 268, 8 controls



ฟอร์มล็อกอินต้นแบบที่แอปอื่นลอกไป



```text

+--------------------------------------------------------------+  398 x 268

|  pictureEdit1                             (0,14)  398 x 50    |  CenterScreen

|  \[------------------ แบนเนอร์โลโก้ ------------------------]  |

|                                                              |

|        +----------------------------------------+            |

|        |  panelControl2        (62,75) 268x154  |            |

|        |                                        |            |

|        |   txtUsername    (53,31)  156 x 20     |            |

|        |   \[\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_]       |            |

|        |                                        |            |

|        |   txtPassword    (53,57)  156 x 20     |            |

|        |   \[\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_]       |            |

|        |                                        |            |

|        |     \[   OK   ]      \[  Cancel  ]       |            |

|        |     (53,90)          (134,90)          |            |

|        |      75x30            75x30            |            |

|        +----------------------------------------+            |

|                                                              |

|  panelControl1 (0,0) 398x268  BackColor ARGB(234,230,223)     |

+--------------------------------------------------------------+

```



\#### `Innovation.TotalWeight\_PLC / frmSelectKB` — 505 × 330, 12 controls



หน้าเลือกคัมบัง (`Text = "เลือกคัมบัง"`) — grid เต็มพื้นที่ + แถบปุ่มด้านล่าง



```text

+------------------------------------------------------------+  505 x 330

|  panelControl2  (2,2)  501 x 269                           |

|  +------------------------------------------------------+  |

|  |  DBGrid5   (2,2)  497 x 265                          |  |

|  |  +--------+------------+----------+---------------+  |  |

|  |  | คัมบัง | สูตร        | ไลน์      | สถานะ         |  |  |

|  |  +--------+------------+----------+---------------+  |  |

|  |  |        |            |          |               |  |  |

|  |  |             (แถวข้อมูลคัมบัง)                    |  |  |

|  |  |                                                |  |  |

|  |  +--------+------------+----------+---------------+  |  |

|  +------------------------------------------------------+  |

|                                                            |

|  tileNavPane1  (2,271)  501 x 57                           |

|  \[  ตกลง  ]  \[  ยกเลิก  ]                                  |

+------------------------------------------------------------+

```



> `Presenter\_SelectKB.GetKanban()` อาจสั่งปิดฟอร์มนี้ทันทีถ้า `res.HasNoSetting`

> หรือ `res.HasNoModuleId` — เป็นสาเหตุที่ UI test จับหน้าต่างนี้ไม่ทัน (checklist §K)



\#### `Innovation.TotalWeight\_PLC / frmInputBarcode` — 392 × 426, 22 controls



หน้ากรอก/ยิงบาร์โค้ดวัตถุดิบระหว่าง auto-feed



```text

+----------------------------------------------------------+  392 x 426

|  +---------------------+  +---------------------------+  |

|  | panelControl3       |  | panelControl4 (133,75)    |  |

|  | (6,6) 217x68        |  | 228 x 81                  |  |

|  | Edit18 (5,5) 207x58 |  |  รหัสวัตถุดิบ  (144,63)     |  |

|  | \[   บาร์โค้ด   ]     |  |  Label1     (144,110)     |  |

|  +---------------------+  |  Barcode Rawmat (144,132) |  |

|                           +---------------------------+  |

|  edRMBC  (56,154)  283 x 30                              |

|  \[\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_]            |

|                                                          |

|  +----------------------------------------------------+  |

|  |  GroupBox2   (5,190)   382 x 229                   |  |

|  |                                                    |  |

|  |   สูงสุด  (12,29)      Edit14 (158,15) 176x48       |  |

|  |                        \[        น้ำหนักสูงสุด    ]   |  |

|  |   น้ำหนัก (12,94)                                   |  |

|  |                                                    |  |

|  |   ต่ำสุด  (12,179)     Edit15 (158,169) 176x48      |  |

|  |                        \[        น้ำหนักต่ำสุด    ]   |  |

|  +----------------------------------------------------+  |

|  gridWTmp (2,2) 388x103  ← ตารางน้ำหนักชั่วคราว           |

+----------------------------------------------------------+

```



\#### `Innovation.TotalWeight\_PLC / frmTotalWeight` — 843 × 608, \*\*164 controls\*\*



หน้าจอปฏิบัติงานหลักหน้าโรงงาน — control เยอะที่สุดในระบบ วาดเป็น\*\*ราย region\*\*

โครงสร้างจริงคือ `panelControl1` (0,0 843×608) ครอบทั้งหมด และ `panelControl2` (2,2 839×109)

เป็นแถบบนสุด ส่วนที่เหลือซ้อนอยู่ในคอนเทนเนอร์ของ DevExpress



```text

+========================================================================+  843 x 608

|  panelControl2 (2,2) 839 x 109  — แถบสถานะบนสุด                        |

|  คัมบัง | สูตร | ไลน์ | แบท | โหมด: Mode\[2] : Auto Weight Only          |

+========================================================================+

|  ภูมิภาค A — ขั้นตอนการชั่ง (5 step)                                     |

|  +------------------------------------------------------------------+  |

|  | step | วัตถุดิบ | เป้าหมาย | สูงสุด | ต่ำสุด | น้ำหนักจริง | สถานะ |  |

|  |  1   |         |          |       |       |            |  \[ ]   |  |

|  |  2   |         |          |       |  <-- ขั้น 2/3 ใช้ ±0.02 คงที่ |  |

|  |  3   |         |          |       |       |            |  \[ ]   |  |

|  |  4   |         |          |       |       |            |  \[ ]   |  |

|  |  5   |         |          |       |       |            |  \[S]   |  |

|  +------------------------------------------------------------------+  |

|                                                                        |

|  ภูมิภาค B — เวลาผสม (lbMixingTimeSet1..5 / lbMixingTimeActual1..5)     |

|  ตั้งไว้ : \[\_\_] \[\_\_] \[\_\_] \[\_\_] \[\_\_]                                     |

|  จริง   : \[\_\_] \[\_\_] \[\_\_] \[\_\_] \[\_\_]                                     |

|                                                                        |

|  ภูมิภาค C — ตำแหน่งก่อนเข้าขั้น (lbPositionBeforeStep1..5)              |

|  \[\_\_] \[\_\_] \[\_\_] \[\_\_] \[\_\_]                                              |

|                                                                        |

|  ภูมิภาค D — อุณหภูมิ / เวลา                                            |

|  lbDumpTemp \[\_\_]   lbIndicatorTemperature \[\_\_]   lbAlphaTime \[\_\_]      |

|                                                                        |

|  ภูมิภาค E — บาร์โค้ด/คัมบัง + ปุ่มสั่งงาน                                |

|  \[ ยิงคัมบัง \_\_\_\_\_\_\_\_\_\_\_\_\_\_ ]   \[Accept]  \[Save]  \[PLC Realy=Label48]   |

+========================================================================+

```



> `Label48` ("PLC Realy" — สะกดผิด ที่ถูกคือ Relay) คือปุ่มเปิด `frmPLCTest`

> handler ของมันเป็น synchronous และ\*\*ไม่ได้ห่อด้วย `RunSafeAsync`\*\* (checklist §K)



\#### `Innovation.MasterData / frmMasterDataMain` — 1179 × 665, 26 controls



เมนูหลักแบบ Ribbon + dock panel



```text

+==========================================================================+  1179 x 665

|  ribbon                                                                  |

|  \[หน้าแรก] \[ข้อมูลหลัก] \[เชื่อมต่อ ERP] \[รายงาน]                            |

|  ( ลูกค้า )( ผู้ขาย )( สินค้า )( คลัง )( สถานี )( เครื่องจักร )( ไลน์ )...   |

+==========================================================================+

|  dockPanel1        |                                                      |

|  +--------------+  |            พื้นที่เอกสาร (MDI-like)                    |

|  | ต้นไม้เมนู    |  |                                                      |

|  |  ลูกค้า      |  |     หน้ารายการ (grid) --เลือกแถว--> หน้ารายละเอียด     |

|  |  ผู้ขาย      |  |                                                      |

|  |  สถานที่     |  |     คู่ Main/Detail นี้ทำซ้ำ \~20 ครั้ง                  |

|  |  คลัง       |  |                                                      |

|  |  ...        |  |                                                      |

|  +--------------+  |                                                      |

+==========================================================================+

|  ribbonStatusBar    ผู้ใช้ | ไซต์ | เวอร์ชัน                                |

+==========================================================================+

```



\#### `Innovation.RM\_Confirm / frmMain` — 1338 × 626, "เมนูหลัก"



หน้าจอยืนยันวัตถุดิบ — ตัวอย่างที่ดีของ layout แบบสแกนบาร์โค้ด



```text

+============================================================================+  1338 x 626

|  panel2 (1,3) 847 x 522              |  panel5 (846,3) 480 x 522           |

|  +--------------------------------+  |  วันที่และเวลา  (3,6)                 |

|  | gridControl1 (3,6) 836 x 516   |  |  lbDatetime   (5,54)  115x29        |

|  | +------+--------+-----------+  |  |                                     |

|  | |คัมบัง | วัตถุดิบ | สถานะ    |  |  |  สแกนบาร์โค้ด (5,103) 154x29        |

|  | +------+--------+-----------+  |  |  txtBarcode  (5,140)  472 x 38      |

|  | |      |        |           |  |  |  \[\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_]    |

|  | |    (รายการที่สแกนแล้ว)      |  |  |                                     |

|  | |      |        |           |  |  |  ไลน์ผลิต (3,190)  หมายเลขแบท(218,190)|

|  | +------+--------+-----------+  |  |  หมายเลขคัมบัง (3,271)                |

|  +--------------------------------+  |  ชื่อสูตร (3,352)                     |

|                                      |  txtFormulation (5,384) 472 x 46    |

|                                      |  รหัสแผน (8,433)                     |

+============================================================================+

|  panel4 (1,531) 1325 x 92                                                  |

|  \[เรียกดูกัมบัง F5] \[ประวัติการสแกน Ctrl+H] \[ตั้งค่า F2]     \[ออกโปรแกรม Esc] |

|   (11,21) 183x62     (212,20) 249x64      (479,21) 183x64   (1110,19) 199x64|

+============================================================================+

```



> สังเกตว่าปุ่มทุกตัวมี \*\*hotkey กำกับในข้อความปุ่ม\*\* (`\[F5]`, `\[Ctrl+H]`, `\[F2]`, `\[Esc]`)

> เพราะผู้ใช้หน้าโรงงานใส่ถุงมือและใช้คีย์บอร์ดเป็นหลัก ไม่ใช่เมาส์ — เป็นข้อจำกัดด้าน UX ที่ต้องคงไว้



\#### `Innovation.DrawingReturn / frmMain` — 966 × 736, 5 controls



โครงเรียบที่สุด: ribbon + accordion + status bar



```text

+==========================================================+  966 x 736

|  ribbon  (0,0)  966 x 158                                |

+==========================================================+

| accordion   |                                            |

| (0,158)     |          พื้นที่เนื้อหา                      |

| 218 x 554   |          (frmReturnRm เปิดซ้อนที่นี่)         |

|             |                                            |

+==========================================================+

|  ribbonStatusBar  (0,712)  966 x 24                      |

+==========================================================+

```



\#### `KB\_PLC\_Control / Main` — 1016 × 739 — เชลล์ MDI แบบเก่า



```text

+==========================================================+  1016 x 739

|  ribbon                                                  |

+==========================================================+

|  splitContainerControl2                                  |

|  +------------+  +-------------------------------------+ |

|  | navBar     |  |  MDI child area                     | |

|  | - รายงาน    |  |  new ProductionReport();            | |

|  | - หยุดเครื่อง |  |  \_frmProReport.MdiParent = this;    | |

|  | - ตรวจสอบ   |  |  ← สร้างตรงใน event handler          | |

|  +------------+  +-------------------------------------+ |

+==========================================================+

|  ribbonStatusBar                                         |

+==========================================================+

```



\#### ฟอร์มที่ใหญ่ที่สุดในระบบ



| ฟอร์ม | ClientSize | Controls |

|---|---|---:|

| `MasterData/frmItemDetails` | 1498 × 899 | \*\*525\*\* |

| `MasterData/frmCustomerDetail` | 1618 × 870 | 300 |

| `InventoryManagement/frmMaterialRequest` | 1618 × 919 | 269 |

| `MasterData/frmVendorDetail` | 1638 × 733 | 267 |

| `InventoryManagement/frmMaterialRequestTransferToProduction` | 1557 × 919 | 261 |



> `frmItemDetails` ที่ 525 control คือหน้าจอที่ซับซ้อนที่สุด — เป็นหน้ารายละเอียดสินค้าที่มีแท็บจำนวนมาก

> ความกว้าง 1498–1638 px บ่งชี้ว่าออกแบบให้ใช้บนจอเดสก์ท็อปกว้าง ไม่ใช่จอสถานีงาน



\### 8.6 ภาษาและการแปล



ข้อความที่ผู้ปฏิบัติงานเห็นเป็น\*\*ภาษาไทย ฝังตายในโค้ด presenter และฟอร์ม ไม่ได้อยู่ใน `.resx`\*\*



```csharp

\_view.ShowMessage($"ไม่พบข้อมูล Setting สำหรับ Station ID: {\_view.req.KBNormal.StationId}");

MessageBoxHelper.ShowWarning("กรุณาใส่รหัสผ่านด้วย");

errorMessage = string.Join("\\n", "ไม่พบการกำหนดค่า Send Step Parameter", ...);

```



> `Prompt/REFACTOR\_GUIDE.md` ของ `TotalWeight\_PLC` \*\*ห้ามแก้ข้อความเหล่านี้แม้แต่ตัวอักษรเดียว\*\*

> เหตุผลที่ระบุไว้: ผู้ปฏิบัติงานคุ้นชินกับถ้อยคำเดิม และข้อความเหล่านี้อาจมีผลต่อคนที่กำลังคุมเครื่องจักรอยู่

> ข้อห้ามนี้ครอบคลุมถึง `\\n`, ช่องว่าง, ไอคอน, ปุ่ม และ `DialogResult` ที่ตรวจสอบ

>

> นี่เป็นข้อจำกัดที่แท้จริงต่อการออกแบบใหม่ — และเป็นเหตุผลที่การย้ายไปใช้ resource file

> ต้องทำแบบ 1:1 ก่อน แล้วค่อยเพิ่มภาษาอังกฤษเป็นภาษาที่สอง



\---



\## 9. การเชื่อมต่อ PLC และฮาร์ดแวร์



\### 9.1 ไดรเวอร์ PLC



\*\*Mitsubishi MX Component\*\* ผ่าน COM reference `ActUtlTypeLib` คลาส `ActUtlType`



```xml

<COMReference Include="ActUtlTypeLib">

```



```csharp

using ActUtlTypeLib;



public ActUtlType ActFXCPU1 { get; set; }     // ← ประกาศบน interface ของ view

ActFXCPU1 = new ActUtlType();

```



API ที่ใช้: `Open()`, `Close()`, `GetDevice(...)`, `SetDevice(...)`, block read/write



\### 9.2 โค้ดอยู่ที่ไหน



`UI/Implementations/frmTotalWeight.PlcCommunication.cs` — \*\*1,066 บรรทัด เป็น partial ของฟอร์ม\*\*

มีคำศัพท์ห่อหุ้มภายในของตัวเอง:



```csharp

public void ResetPlcDevices(PlcDevices v)

{

&#x20;   CallTracer.Record("UI", nameof(ResetPlcDevices), $"device={v}");

&#x20;   ExecutePlcAction(() =>

&#x20;   {

&#x20;       if (PlcDevices.Oil == v)

&#x20;       {

&#x20;           IsAutomixUseOil = false;

&#x20;           if (!TryWriteDevice(weightSiloA, 0)) return;

&#x20;           if (!TryWriteDevice(weightSiloB, 0)) return;

&#x20;           if (!TryWriteDevice(minWeightOilFeed, 0)) return;

&#x20;           // ...

&#x20;       }

&#x20;   });

}

```



`ActUtlType` ถูก `new` ข้างในฟอร์ม (`frmPLCTest`, `frmShowAutoFeed`) และ

\*\*ถูกเปิดเป็นชนิดรูปธรรมบน interface ของ view\*\*:



```csharp

public interface IView\_PLCTest : ...

{

&#x20;   ActUtlType ActFXCPU1 { get; }     // ← COM type รั่วออกมาถึงชั้น interface

}

```



> นี่คือเหตุผลที่แทน PLC ด้วยตัวจำลองไม่ได้ถ้าไม่แก้โครงก่อน — ไม่มี interface คั่นเลย



\### 9.3 แผนที่ address



\- `Common/SendStepParamConfiguration.PlcId` — ค่าคงที่ของ address

\- `Common/Enums/PlcEnums.cs` — enum ของอุปกรณ์ (`PlcDevices.Oil`, …)

\- ตั้งค่าจากฐานข้อมูลผ่านตาราง `SEND\_STEP\_PARAMETER` เช่น `Id=2` คือ Feeddoor Step

&#x20; และ address ID70–74 คือคำสั่งเปิดประตูป้อน



`BaseForm` มีตัวช่วยอ่านค่าเหล่านี้ ทั้งที่เป็นคลาสฐานของ UI:



```csharp

protected virtual SendStepParameterVM TryGetStep(int id) => null;



protected bool TryGetStepParam(int id, out SendStepParameterVM param, out string errorMessage,

&#x20;                              string lineName, bool? isAutoMixing = null)

{

&#x20;   param = TryGetStep(id);

&#x20;   if (param != null) { errorMessage = string.Empty; return true; }

&#x20;   string detail = isAutoMixing != null

&#x20;       ? $"Check Mode {(isAutoMixing == true ? "Auto Mixing" : "Auto Inject Oil")}"

&#x20;       : SendStepParamConfiguration.GetDescription(id);

&#x20;   errorMessage = string.Join("\\n", "ไม่พบการกำหนดค่า Send Step Parameter", ...);

&#x20;   return false;

}

```



\### 9.4 ตาชั่งและบาร์โค้ด



\- \*\*ตาชั่ง\*\* — `System.IO.Ports.SerialPort` อ่านใน `frmMain.cs` ป้อนน้ำหนักเข้าช่องของขั้นที่กำลังทำงาน

&#x20; ไฟล์ที่เกี่ยวข้อง: `frmMain.cs`, `frmSettingMaxMin.cs`, `frmTotalWeight.Designer.cs`

\- \*\*บาร์โค้ด\*\* — เครื่องอ่านแบบ keyboard wedge ยิงเข้าช่องข้อความ แล้วตรวจตามลำดับ:

&#x20; ว่างเปล่า → จำนวน → ชนิด → ล็อก/หมดอายุ → ซ้ำ

\- ยังมี P/Invoke `GetPrivateProfileString` ที่ `frmTotalWeight.cs:1734` สำหรับอ่านค่าตั้งต้น PLC จาก ini



\### 9.5 จุดที่ต้องใส่ interface ถ้าจะจำลองฮาร์ดแวร์



| ปัจจุบัน | ต้องมี | ตัวจริง | ตัวจำลอง |

|---|---|---|---|

| `ActUtlType` เรียกตรง | `IPlcDevice` | `MxComponentPlcDevice` | `SimulatedPlcDevice` |

| `SerialPort` ใน `frmMain` | `IScaleReader` | `SerialScaleReader` | `SimulatedScaleReader` |

| keyboard wedge เข้า TextBox | `IBarcodeSource` | `KeyboardWedgeBarcodeSource` | `ScriptedBarcodeSource` |



สถานการณ์ที่ตัวจำลองต้องทำได้ อ่านจาก `RUNTIME\_TEST\_CHECKLIST.md` โดยตรง:

ชั่งปกติ · น้ำหนักนอกช่วง · PLC ต่อไม่ติด · `RM\_BAL` ไม่มีบาร์โค้ด · ไม่ได้ตั้งค่า Feeddoor Step ·

`PRODSTD\_MIXTEMP` ไม่มีแถว · เขียนฐานข้อมูลไม่สำเร็จระหว่าง auto-feed



\---



\## 10. Threading และ modality ของ WinForms



\- \*\*ไม่มี `Application.Run()`\*\* — message loop คือ `ShowDialog()` ของฟอร์มแรก

&#x20; หน้าจอถัดไปเป็น `ShowDialog` ซ้อนกันเข้าไปอีกชั้น

\- `KB\_PLC\_Control` เป็นตัวเดียวที่เป็น MDI parent จริง

\- \*\*มีเฉพาะ `TotalWeight\_PLC` ที่ใช้ `async`/`await`\*\* ใน presenter และ service

&#x20; (`public async Task<bool> Login()`) แอปอื่น synchronous ล้วน

\- การข้ามเธรดจากงานเบื้องหลังต้อง `Invoke` เสมอ:

&#x20; ```csharp

&#x20; this.Invoke((MethodInvoker)(() => { MessageBoxHelper.ShowError("โหลดข้อมูลไม่สำเร็จ", "Error"); }));

&#x20; ```

\- `\_caseActive` ใน `RunSafeAsync` เป็น `static` — ปลอดภัยเฉพาะกรณีสถานีเดียวเธรดเดียวเท่านั้น



\---



\## 11. สิ่งที่ไม่ควรทำตาม



| ปัญหา | รายละเอียด |

|---|---|

| สัญญา MVP คัดลอก 10 ชุด | ไม่เคยย้ายเข้าไลบรารีกลาง ทั้งที่เหมือนกันทุกตัวอักษร |

| `NotFound()` รายงาน\*\*และ\*\*ปิดฟอร์มพร้อมกัน | ต้นเหตุของ checklist §H/§I/§J — ตอนนี้ต้องเขียนคอมเมนต์ห้ามเรียกกระจาย 6 ที่ |

| ธง `bool` \~30 ตัวแทน `Result<T>` | ต้องแก้สามที่ทุกครั้งที่เพิ่มกรณีล้มเหลว |

| `res == null` เป็นผลลัพธ์เงียบ | แยกไม่ออกจาก "สำเร็จแต่ไม่มีข้อมูล" |

| controller ใช้ view ส่งพารามิเตอร์ | view ต้องเปิด public mutable property จำนวนมาก ทดสอบแยกไม่ได้ |

| `\_view.Presenter = this` ใน ctor | ผูกกันเป็นวง จน `TotalWeight\_PLC` ต้อง cross-wire ด้วยมือ |

| `Application.Run()` ไม่ถูกเรียก | ใช้ `ShowDialog` ซ้อนแทน message loop |

| `\_caseActive` เป็น static บน form base | ปลอดภัยเฉพาะสถานีเดียว |

| `BaseForm` รู้เรื่อง PLC | คลาสฐาน UI ไม่ควรรู้จัก `SendStepParameter` |

| handler แบบ sync ไม่ผ่าน `RunSafeAsync` | exception ถูกกลืน (checklist §K) |

| `PresenterLocator` แข่งกับ DI | มีสองกลไก resolve ในแอปเดียว |

| `HintPath` ชี้ `bin\\Debug\\` | build Release ลิงก์ไบนารี Debug |

| `HintPath` เป็น absolute path | `D:\\Library\\InnoControlLibrary.dll` — build บนเครื่องอื่นไม่ได้ |

| รหัสผ่าน SQL ในไฟล์ config รายไซต์ | plaintext |

| namespace ไม่ตรงโปรเจกต์ | `ExtendedWebClient` อยู่ใน `namespace Innovation.ProductionManagement...`, `namespace WindowsApplication1` |

| global state แบบ public static บนฟอร์ม | `KB\_PLC\_Control.Main.LoginUserId` ฯลฯ |

| ข้อความไทยฝังตายในโค้ด | ไม่ได้อยู่ใน `.resx` แปลไม่ได้ |



\---



\## 12. Recreate prompt



```

สร้างแอปเดสก์ท็อป WinForms ตามสถาปัตยกรรม MVP นี้



ข้อจำกัด

\- .NET Framework 4.6.2 หรือ 4.7.2, csproj รูปแบบเก่า (ระบุไฟล์ทีละไฟล์)

\- DevExpress v21.2 สำหรับ control ทั้งหมด (XtraForm, GridControl, RibbonControl, TextEdit, ...)

\- C# 7.3 เท่านั้น — ห้าม switch expression, using declaration, nullable reference type, record

\- ไม่มี Application.Run() — message loop คือ ShowDialog() ของฟอร์มแรก



โครงโฟลเดอร์ (เลือกหนึ่งแบบแล้วใช้ให้สม่ำเสมอ)

&#x20; แบ่งตามชั้น:   UI/{Implements,Interfaces}  Presenter\[s]/{Implements,Interfaces}

&#x20;                Service\[s]/{Implements,Interfaces}  AppController/{Implements,Interfaces}  ViewModel/

&#x20; แบ่งตามฟีเจอร์: <Feature>/{UI,Presenter,Service,AppController}  + Const/ Global/ Helper/ ViewModel/



สัญญาหลัก — คัดลอกตามนี้ทุกตัวอักษร

&#x20; public interface IView<TPresenter> { TPresenter Presenter { set; } void Run(); }

&#x20; public interface IChildView<TPresenter, TParentPresenter> : IView<TPresenter>

&#x20;     { IView<TParentPresenter> ParentView { set; } }

&#x20; public interface IPresenter<TView> { TView View { get; } }

&#x20; public interface IGeneralViewPresenter<TView> : IPresenter<TView> { void Run(); }

&#x20; public interface IChildViewPresenter<TView, TParentPresenter> : IPresenter<TView>

&#x20;     { void Run(IView<TParentPresenter> parentView); }

&#x20; IApplicationController = ลิสต์แบนของ RunXxx() หนึ่งเมธอดต่อหนึ่งหน้าจอ



รูปแบบบังคับ

&#x20; presenter ctor รับ (IView, IService) แล้วตั้ง \_iView.Presenter = this

&#x20; ฟอร์ม implement IView\_<Screen> และ Run() => ShowDialog()

&#x20; controller resolve presenter จาก ServiceProvider แล้วเรียก presenter.Run()

&#x20; controller ที่ต้องคืนค่า: เขียนพารามิเตอร์ลง property ของ view ก่อน Run() แล้วอ่านผลกลับหลัง Run()

&#x20; service เรียก API ด้วย WebClient.BaseAddress = Properties.Settings.Default.ApiAddress

&#x20;     อ่าน = DownloadString("ActionName")  เขียน = UploadString("ActionName", json)

&#x20; ยืนยันตัวตนด้วย PrincipalContext(ContextType.Domain, "company.local").ValidateCredentials

&#x20; ตอบกลับจาก service ใช้ VM ที่มีธง bool รายกรณี (HasNoXxx / IsEmptyXxx / IsNotEqualXxx)

&#x20;     แล้ว presenter ไล่ if ทีละธง — ไม่ใช้ exception



Program.cs

&#x20; ServiceCollection → BuildServiceProvider() เก็บไว้ใน public static IServiceProvider ServiceProvider

&#x20; ลงทะเบียนเรียงเป็นสามบล็อก: Service / Presenter / UI แล้วปิดท้ายด้วย

&#x20;     services.AddSingleton<IApplicationController, ApplicationController>()

&#x20; ตั้ง DevExpress skin ก่อน ConfigureServices():

&#x20;     SkinManager.EnableFormSkins(); BonusSkins.Register();

&#x20;     UserLookAndFeel.Default.SkinName = ConfigurationManager.AppSettings\["ApplicationSkinName"];

&#x20; Main() ปิดท้ายด้วย app.RunLogin()



การ deploy รายไซต์

&#x20; Program\_Config\_By\_Site/<SITE>/<App>.exe.config  และ  AutoUpdate\_Setting\_By\_Site/<SITE>/setting.ini

&#x20; ประกาศเป็น <None Include=...> ไม่คัดลอกไป output

&#x20; ต่างกันแค่ API host กับ SITE\_ID\_FOR\_AUTO\_UPDATE



ข้อความบนหน้าจอเป็นภาษาไทย ฝังในโค้ด ห้ามแก้ถ้อยคำ

ผลลัพธ์ที่ต้องการ: ASCII tree ตามด้วยเนื้อไฟล์แต่ละไฟล์เป็น code block

```



\---



\## เอกสารที่เกี่ยวข้อง



\- \[../README.md](../README.md) — ภาพรวม โดเมน บทเรียน และแผนสร้างใหม่ให้รันได้จริง

\- \[../Backend (The Server-Side)/ROADMAP.md](../Backend%20\\(The%20Server-Side\\)/ROADMAP.md) — พิมพ์เขียวฝั่งเซิร์ฟเวอร์







