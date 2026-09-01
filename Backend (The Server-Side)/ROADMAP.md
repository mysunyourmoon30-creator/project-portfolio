\# Backend (The Server-Side) — Architecture Blueprint



> เอกสารนี้บรรยาย\*\*ระบบจริงตามที่เป็นอยู่\*\* รวมข้อบกพร่องและคำที่สะกดผิด — ไม่ได้แก้ให้สวยขึ้น

> เพราะเป้าหมายคือให้อ่านแล้วสร้างใหม่ได้เหมือนเดิมทุกประการ ส่วนแผน "สร้างใหม่ให้สะอาด"

> อยู่ใน \[../README.md](../README.md)

>

> ค่าที่เป็นความลับ (รหัสผ่าน ชื่อเซิร์ฟเวอร์ IP โดเมน ชื่อลูกค้า ชื่อนักพัฒนา) ถูกแทนด้วย placeholder ทั้งหมด



\---



\## 1. ภาพรวมและข้อจำกัด



ระบบหลังบ้านของ MES (Manufacturing Execution System) โรงงานยาง/คอมพาวนด์ ให้บริการแอปเดสก์ท็อป

ประมาณ 10 ตัวที่กระจายอยู่ 6 ไซต์โรงงาน



\### โปรเจกต์ในโซลูชัน `BackEnd.sln`



| โปรเจกต์ | ไฟล์ `.cs` | TFM | บทบาท |

|---|---:|---|---|

| `Innovation.Core` | 5,811 | `netstandard2.0` | โมเดลทั้งหมด (Data/Dto/Domain/Api) + interface กลาง |

| `Innovation.Repositories` | 1,217 | `netstandard2.0` | `DbContext` 16 ตัว, `RepositoryImpl<T>`, UnitOfWork, Factory |

| `Innovation.Library` | 871 | .NET Framework | \*\*ระบบรุ่นก่อน\*\* (DATA/DAO/BLL) — ดู §11 |

| `Innovation.Services` | 92 | `netstandard2.0` | business service \~78 ตัว + domain service + background service |

| `Innovation.API` | 57 | `net6.0` | Web API — 53 controller |

| `Innovation.UtilityCore` | 33 | mixed | helper ข้ามชั้น (รวม `DataMappingHelper`) |

| `Innovation.WebApp` | 26 | `net6.0` | `Innovation.PlanOnline` (Blazor) + `Innovation.WIOnline` (MVC) |

| `Innovation.Class.LibraryCore` | 21 | `netstandard2.0` | DB connection, D365 interface |

| `Innovation.PrinterService.API` | 15 | `net6.0` | บริการพิมพ์ป้ายแยกต่างหาก |

| `Innovation.ReportServices` | 1 | `netstandard2.0` | เรนเดอร์ `.rdlc` |



> \*\*ข้อสังเกตเรื่อง TFM\*\*: ไลบรารีเป็น `netstandard2.0` แต่อ้าง NuGet package รุ่น `8.0.x`

> ขณะที่ EF Core ตรึงไว้ที่ `3.1.32` ในไลบรารี แต่ `Innovation.API` ใช้ `6.0.32` —

> เป็นการผสมรุ่นที่ต้องระวังเวลาอัปเกรด



\### ข้อจำกัดที่ต้องคงไว้ถ้าจะสร้างใหม่ให้เหมือนเดิม



\- \*\*ห้ามใช้ nullable reference types\*\* — ทุกโปรเจกต์ตั้ง `<Nullable>disable</Nullable>`

&#x20; และ EF scaffold ก็ generate มาโดยสมมติว่าปิด NRT

\- \*\*`netstandard2.0`\*\* สำหรับไลบรารี แปลว่าใช้ฟีเจอร์ C# ใหม่ๆ หลายอย่างไม่ได้

\- \*\*SQL Server เท่านั้น\*\* — `UseSqlServer` ฝังอยู่ใน `DbContext` ทุกตัว

\- แพ็กเกจที่ขาดไม่ได้: `Microsoft.EntityFrameworkCore.SqlServer`, `EFCore.BulkExtensions`,

&#x20; `AutoMapper`, `Mapster`, `DinkToPdf`, `AspNetCore.Reporting`, `QRCoder`, `ZXing.Net`,

&#x20; `log4net`, `Swashbuckle.AspNetCore`



\---



\## 2. โครงสร้างโซลูชัน



ทุกโฟลเดอร์ปรากฏครบ \*\*519 โฟลเดอร์\*\* (ไม่รวม `bin/`, `obj/`, `.vs/`, `packages/`)

ตัวเลข `\[n cs]` คือจำนวนไฟล์ `.cs` ที่อยู่ในโฟลเดอร์นั้นโดยตรง (ไม่นับโฟลเดอร์ย่อย)

วงเล็บมุม `<...>` คือไฟล์สำคัญประจำโฟลเดอร์



```text

Backend (The Server-Side)/

|-- Innovation.API/  <Innovation.API.sln>

|   `-- Innovation.API/  \[2 cs]  <Innovation.API.csproj, Program.cs, appsettings.json>

|       |-- Controllers/  \[53 cs]

|       |-- Middleware/  \[1 cs]

|       |-- Properties/

|       |   |-- DataSources/

|       |   `-- PublishProfiles/

|       |-- Report/

|       |   |-- Component/

|       |   `-- Style/

|       |       `-- Image/

|       |-- Resource/

|       |   `-- ImageGHS/

|       |-- Service/  \[1 cs]

|       `-- wwwroot/

|-- Innovation.Class.LibraryCore/  <Innovation.Class.LibraryCore.sln>

|   |-- Innovation.Class.D365InterfaceService/  <Innovation.Class.D365InterfaceService.csproj>

|   |   |-- Implement/  \[1 cs]

|   |   |-- Interface/  \[2 cs]

|   |   `-- Model/  \[1 cs]

|   |-- Innovation.Class.DBConnectionsCore/  \[11 cs]  <Innovation.Class.DBConnectionsCore.csproj>

|   |-- Innovation.Class.DBConnectionsCore.Tests/  \[1 cs]  <Innovation.Class.DBConnectionsCore.Tests.csproj, appsettings.json>

|   |   `-- Properties/

|   `-- Innovation.Class.LibraryCore/  \[5 cs]  <Innovation.Class.LibraryCore.csproj>

|-- Innovation.Core/  <Innovation.Core.sln>

|   `-- Innovation.Core/  <Innovation.Core.csproj>

|       |-- ApiModel/  \[4 cs]

|       |   |-- AutoWeight/  \[5 cs]

|       |   |-- InvoiceExport/  \[1 cs]

|       |   `-- Packing/  \[3 cs]

|       |-- Core/

|       |   |-- Repository/  \[12 cs]

|       |   |   |-- CentralDB/  \[5 cs]

|       |   |   |-- DBCenter/  \[115 cs]

|       |   |   |-- DBMaster/  \[408 cs]

|       |   |   |-- DBMasterHist/  \[67 cs]

|       |   |   |-- DBSdb/  \[5 cs]

|       |   |   |-- DBTransection/  \[266 cs]

|       |   |   |-- DBTransectionHist/  \[55 cs]

|       |   |   |-- DB\_Auto\_Report/  \[3 cs]

|       |   |   |-- DataCenter/  \[192 cs]

|       |   |   |-- Data\_Center\_Master/  \[3 cs]

|       |   |   |-- Material\_Management/  \[10 cs]

|       |   |   |-- ProductionSTD/  \[5 cs]

|       |   |   `-- SILO/  \[13 cs]

|       |   |-- Service/  \[64 cs]

|       |   `-- UnitOfWork/  \[1 cs]

|       |       `-- Context/  \[14 cs]

|       |-- DataModel/

|       |   |-- CentralDB/  \[114 cs]

|       |   |-- DBCenter/  \[150 cs]

|       |   |-- DBMaster/  \[601 cs]  <Program.cs>

|       |   |-- DBMasterHist/  \[71 cs]

|       |   |-- DBSdb/  \[9 cs]

|       |   |-- DBTransection/  \[517 cs]

|       |   |-- DBTransectionHist/  \[74 cs]

|       |   |-- DB\_Auto\_Report/  \[3 cs]

|       |   |-- DataCenter/  \[209 cs]  <Program.cs>

|       |   |-- DataCenterMaster/  \[344 cs]  <Program.cs>

|       |   |-- MasterPOPRPan/  \[24 cs]

|       |   |-- MaterialManagement/  \[444 cs]

|       |   |-- Packing/  \[40 cs]

|       |   |-- ProductionSTD/  \[306 cs]

|       |   |-- QCDB/  \[109 cs]

|       |   `-- SILO/  \[101 cs]

|       |-- DomainModel/  \[281 cs]

|       |   |-- DBCenter/  \[24 cs]

|       |   |   |-- Const/  \[1 cs]

|       |   |   `-- InventoryManagement/  \[1 cs]

|       |   |-- DBMaster/  \[14 cs]

|       |   |   |-- Const/  \[9 cs]

|       |   |   `-- MasterData/  \[9 cs]

|       |   |-- DBMasterCompare/  \[4 cs]

|       |   |-- DBTransection/  \[22 cs]

|       |   |-- Data\_Center\_Master/  \[2 cs]

|       |   |-- KeySpecification/  \[3 cs]

|       |   `-- LogisticManangment/  \[2 cs]

|       `-- DtoModel/  \[30 cs]

|           |-- AutoUpdate/  \[10 cs]

|           |-- BackLog/  \[5 cs]

|           |-- CentralDB/

|           |   `-- ApplicationControl/  \[1 cs]

|           |-- DBCenter/  \[36 cs]

|           |   |-- ArpMaterialPackage/

|           |   |   `-- Insert/  \[4 cs]

|           |   |-- ArpMaterialPickingList/

|           |   |   |-- Data/  \[2 cs]

|           |   |   `-- Insert/  \[1 cs]

|           |   |-- ArpMaterialReceivePlan/

|           |   |   |-- Data/

|           |   |   |   |-- InventoryTransaction/  \[3 cs]

|           |   |   |   `-- ReceivePlanRequest/  \[5 cs]

|           |   |   |-- Insert/  \[3 cs]

|           |   |   |-- InventoryManagement/  \[1 cs]

|           |   |   `-- Update/  \[3 cs]

|           |   |-- ArpMaterialRequest/

|           |   |   |-- Insert/  \[10 cs]

|           |   |   |-- InventoryManagement/  \[7 cs]

|           |   |   |   |-- Edit/  \[10 cs]

|           |   |   |   `-- Status/  \[1 cs]

|           |   |   `-- Update/  \[7 cs]

|           |   |-- InterfaceDynamicData/  \[3 cs]

|           |   |-- InventoryTransection/  \[6 cs]

|           |   |-- RequestMaterial/  \[1 cs]

|           |   |-- ServiceStatusTracking/  \[3 cs]

|           |   |-- TransitCustomer/  \[12 cs]

|           |   |-- TransitUnit/  \[1 cs]

|           |   `-- TransitUnitConversion/  \[4 cs]

|           |-- DBMaster/  \[162 cs]

|           |   |-- InventoryManagement/  \[3 cs]

|           |   |-- InventoryPurpose/  \[3 cs]

|           |   |-- Item/  \[56 cs]

|           |   |   `-- InventoryManagement/  \[3 cs]

|           |   |-- Line/  \[4 cs]

|           |   |-- ProcessManagement/  \[3 cs]

|           |   |-- Vendor/  \[7 cs]

|           |   |-- WeightingCMB/  \[1 cs]

|           |   `-- WeightingCuring/  \[3 cs]

|           |-- DBTransection/  \[63 cs]

|           |   |-- BOMRoute/  \[15 cs]

|           |   |   `-- RouteInBomInsert/  \[6 cs]

|           |   |-- InvExportRequestMst/  \[1 cs]

|           |   |-- InventoryManagement/  \[11 cs]

|           |   |   |-- ApproveInterface/  \[3 cs]

|           |   |   `-- QALock/  \[2 cs]

|           |   |-- OnHand/  \[3 cs]

|           |   |-- PlanOnline/  \[1 cs]

|           |   |-- ProcessManagement/  \[22 cs]

|           |   |-- ProductionManangement/  \[44 cs]

|           |   |-- ProductionOrder/  \[20 cs]

|           |   |-- ProductionOrderRevise/  \[8 cs]

|           |   |-- RouteMaster/  \[9 cs]

|           |   |   |-- Edit/  \[2 cs]

|           |   |   |-- Insert/  \[1 cs]

|           |   |   `-- Interface/  \[1 cs]

|           |   |-- ServiceWehingCMB/  \[5 cs]

|           |   |-- ServiceWeighingCMB/  \[1 cs]

|           |   `-- WeightingCuring/  \[4 cs]

|           |-- DBTransectionHist/  \[5 cs]

|           |   `-- ProcessManagement/  \[2 cs]

|           |-- DB\_Auto\_Report/  \[6 cs]

|           |-- Data\_Center\_Master/  \[2 cs]

|           |-- DrawingReturn/  \[2 cs]

|           |-- InhouseToD365ExportedResult/  \[2 cs]

|           |-- InventoryManagement/  \[22 cs]

|           |   |-- PackageReport/  \[9 cs]

|           |   `-- TransferReport/  \[6 cs]

|           |-- InvoiceExportERP/  \[18 cs]

|           |-- KeySpecification/  \[36 cs]

|           |-- LogisticManagement/  \[2 cs]

|           |-- MaterialReceiveMobile/  \[23 cs]

|           |   |-- PickingDto/  \[9 cs]

|           |   |-- PickingWeight/  \[3 cs]

|           |   |-- ReceiveDto/  \[11 cs]

|           |   `-- ReturnItemDto/  \[1 cs]

|           |-- Material\_Management/  \[5 cs]

|           |   |-- DailyPlan/  \[2 cs]

|           |   `-- RmBal/  \[1 cs]

|           |-- MixingFg/  \[19 cs]

|           |-- Packing/  \[2 cs]

|           |-- PackingCompoundTag/  \[3 cs]

|           |-- PrinterService/  \[5 cs]

|           |-- ProductionAnalyst/  \[1 cs]

|           |-- Report/  \[1 cs]

|           |-- SILO/  \[8 cs]

|           |-- SealSystem/

|           |-- SieveRoom/  \[55 cs]

|           |-- SiloApprove/  \[3 cs]

|           |-- SiloBypass/  \[5 cs]

|           |-- SoftwareManagement/  \[22 cs]

|           |-- TestForMy/  \[7 cs]

|           |-- TotalWeightPlc/  \[18 cs]

|           |-- TraceReport/

|           |   |-- Formulation/  \[2 cs]

|           |   `-- TracebackRm/  \[9 cs]

|           |-- WIOnline/  \[97 cs]

|           |   `-- Const/  \[2 cs]

|           |-- WeighingERP/  \[3 cs]

|           `-- WeightBTMT/  \[7 cs]

|-- Innovation.Library/

|   |-- BuildProcessTemplates/

|   |-- Dev/

|   |   |-- Innovation.Control/  <Innovation.Control.sln>

|   |   |   |-- InnoControlLibrary/  \[2 cs]  <InnoControlLibrary.csproj>

|   |   |   |   |-- Images/

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   `-- InnoControlLibraryExample/  \[3 cs]  <InnoControlLibraryExample.csproj, Program.cs, app.config>

|   |   |       |-- Properties/  \[3 cs]

|   |   |       `-- Resource/

|   |   |-- Innovation.DataCenter/  <Innovation.DataCenter.sln>

|   |   |   |-- Innovation.BLL.Data\_Center\_Master/  \[61 cs]  <Innovation.BLL.Data\_Center\_Master.csproj, Program.cs>

|   |   |   |   `-- Properties/  \[1 cs]

|   |   |   |-- Innovation.BLL.Data\_Detail/  \[21 cs]  <Innovation.BLL.Data\_Detail.csproj>

|   |   |   |   `-- Properties/  \[1 cs]

|   |   |   |-- Innovation.DAO.Data\_Center\_Master/  \[62 cs]  <Innovation.DAO.Data\_Center\_Master.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |-- Innovation.DAO.Data\_Detail/  \[16 cs]  <Innovation.DAO.Data\_Detail.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |-- Innovation.DATA.Data\_Center\_Master/  \[3 cs]  <Innovation.DATA.Data\_Center\_Master.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |-- Innovation.DATA.Data\_Detail/  \[1 cs]  <Innovation.DATA.Data\_Detail.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |-- Innovation.Data.CentralDB/  \[10 cs]  <Innovation.Data.CentralDB.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |       `-- DataSources/

|   |   |   |-- Innovation.Data.MaterialManagement/  \[13 cs]  <Innovation.Data.MaterialManagement.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |-- Innovation.Data.ProductionSTD/  \[23 cs]  <Innovation.Data.ProductionSTD.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   |-- Innovation.DataCenter/  \[131 cs]  <Innovation.DataCenter.csproj, app.config>

|   |   |   |   `-- Properties/  \[2 cs]

|   |   |   `-- Innovation.ICTDB/  \[7 cs]  <Innovation.ICTDB.csproj, app.config>

|   |   |       `-- Properties/  \[2 cs]

|   |   |-- Innovation.Utility/  <Innovation.Utility.sln>

|   |   |   |-- Innovation.Report\_Utility/  \[1 cs]  <Innovation.Report\_Utility.csproj>

|   |   |   |   `-- Properties/  \[1 cs]

|   |   |   |-- Innovation.Utility.AuthenticationCenter/  <Innovation.Utility.AuthenticationCenter.csproj, app.config>

|   |   |   |   |-- Entities/  \[7 cs]  <Program.cs>

|   |   |   |   |-- Presentation/  \[3 cs]

|   |   |   |   |-- Properties/  \[2 cs]

|   |   |   |   |-- Repositories/  \[16 cs]

|   |   |   |   |-- Services/  \[4 cs]

|   |   |   |   `-- ViewInterfaces/  \[3 cs]

|   |   |   |-- Innovation.Utility.Convertor/

|   |   |   |   `-- My Project/

|   |   |   |-- Innovation.Utility.ConvertorTest/

|   |   |   |   `-- My Project/

|   |   |   |-- Innovation.Utility.Generator/

|   |   |   |   `-- My Project/

|   |   |   |-- Innovation.Utility.GeneratorTest/

|   |   |   |   `-- My Project/

|   |   |   |-- Innovation.Utility.Translation/  <Innovation.Utility.Translation.csproj>

|   |   |   |   |-- DataContext/

|   |   |   |   |-- Properties/  \[1 cs]

|   |   |   |   |-- Repository/

|   |   |   |   |   |-- Implement/  \[1 cs]

|   |   |   |   |   `-- Interface/  \[1 cs]

|   |   |   |   |-- Service/

|   |   |   |   |   |-- Implement/  \[1 cs]

|   |   |   |   |   `-- Interface/  \[1 cs]

|   |   |   |   `-- VM\_Model/  \[2 cs]

|   |   |   |-- Innovation.Utility.Winform/  \[1 cs]  <Innovation.Utility.Winform.csproj>

|   |   |   |   `-- Properties/  \[1 cs]

|   |   |   |-- TestApplication/

|   |   |   |   `-- My Project/

|   |   |   |-- TestUsingAuthenticationCenter/  \[3 cs]  <Program.cs, TestUsingAuthenticationCenter.csproj, app.config>

|   |   |   |   |-- Properties/  \[3 cs]

|   |   |   |   |   `-- DataSources/

|   |   |   |   `-- Resources/

|   |   |   `-- TestUsingNormalAuthentication/  \[3 cs]  <Program.cs, TestUsingNormalAuthentication.csproj, app.config>

|   |   |       `-- Properties/  \[3 cs]

|   |   `-- InnovationClassLibrary/  <InnovationClassLibrary.sln>

|   |       |-- EPI/  <app.config>

|   |       |   |-- BLL/

|   |       |   |-- DAL/

|   |       |   `-- My Project/

|   |       `-- EPI\_TEST/

|   |           `-- My Project/

|   `-- Main/

|       |-- Innovation.Control/  <Innovation.Control.sln>

|       |   |-- InnoControlLibrary/  \[2 cs]  <InnoControlLibrary.csproj>

|       |   |   |-- Images/

|       |   |   `-- Properties/  \[1 cs]

|       |   `-- InnoControlLibraryExample/  \[3 cs]  <InnoControlLibraryExample.csproj, Program.cs, app.config>

|       |       |-- Properties/  \[3 cs]

|       |       `-- Resource/

|       |-- Innovation.DataCenter/  <Innovation.DataCenter.sln>

|       |   |-- Innovation.BLL.Data\_Center\_Master/  \[61 cs]  <Innovation.BLL.Data\_Center\_Master.csproj, Program.cs>

|       |   |   `-- Properties/  \[1 cs]

|       |   |-- Innovation.BLL.Data\_Detail/  \[21 cs]  <Innovation.BLL.Data\_Detail.csproj>

|       |   |   `-- Properties/  \[1 cs]

|       |   |-- Innovation.DAO.Data\_Center\_Master/  \[62 cs]  <Innovation.DAO.Data\_Center\_Master.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |-- Innovation.DAO.Data\_Detail/  \[16 cs]  <Innovation.DAO.Data\_Detail.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |-- Innovation.DATA.Data\_Center\_Master/  \[3 cs]  <Innovation.DATA.Data\_Center\_Master.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |-- Innovation.DATA.Data\_Detail/  \[1 cs]  <Innovation.DATA.Data\_Detail.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |-- Innovation.Data.CentralDB/  \[10 cs]  <Innovation.Data.CentralDB.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |       `-- DataSources/

|       |   |-- Innovation.Data.MaterialManagement/  \[13 cs]  <Innovation.Data.MaterialManagement.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |-- Innovation.Data.ProductionSTD/  \[23 cs]  <Innovation.Data.ProductionSTD.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   |-- Innovation.DataCenter/  \[131 cs]  <Innovation.DataCenter.csproj, app.config>

|       |   |   `-- Properties/  \[2 cs]

|       |   `-- Innovation.ICTDB/  \[7 cs]  <Innovation.ICTDB.csproj, app.config>

|       |       `-- Properties/  \[2 cs]

|       |-- Innovation.Utility/  <Innovation.Utility.sln>

|       |   |-- Innovation.Report\_Utility/  \[1 cs]  <Innovation.Report\_Utility.csproj>

|       |   |   `-- Properties/  \[1 cs]

|       |   |-- Innovation.Utility.AuthenticationCenter/  <Innovation.Utility.AuthenticationCenter.csproj, app.config>

|       |   |   |-- Entities/  \[7 cs]  <Program.cs>

|       |   |   |-- Presentation/  \[3 cs]

|       |   |   |-- Properties/  \[2 cs]

|       |   |   |-- Repositories/  \[16 cs]

|       |   |   |-- Services/  \[4 cs]

|       |   |   `-- ViewInterfaces/  \[3 cs]

|       |   |-- Innovation.Utility.Convertor/

|       |   |   `-- My Project/

|       |   |-- Innovation.Utility.ConvertorTest/

|       |   |   `-- My Project/

|       |   |-- Innovation.Utility.Generator/

|       |   |   `-- My Project/

|       |   |-- Innovation.Utility.GeneratorTest/

|       |   |   `-- My Project/

|       |   |-- Innovation.Utility.Translation/  <Innovation.Utility.Translation.csproj>

|       |   |   |-- DataContext/

|       |   |   |-- Properties/  \[1 cs]

|       |   |   |-- Repository/

|       |   |   |   |-- Implement/  \[1 cs]

|       |   |   |   `-- Interface/  \[1 cs]

|       |   |   |-- Service/

|       |   |   |   |-- Implement/  \[1 cs]

|       |   |   |   `-- Interface/  \[1 cs]

|       |   |   `-- VM\_Model/  \[2 cs]

|       |   |-- Innovation.Utility.Winform/  \[1 cs]  <Innovation.Utility.Winform.csproj>

|       |   |   `-- Properties/  \[1 cs]

|       |   |-- TestApplication/

|       |   |   `-- My Project/

|       |   |-- TestUsingAuthenticationCenter/  \[3 cs]  <Program.cs, TestUsingAuthenticationCenter.csproj, app.config>

|       |   |   |-- Properties/  \[3 cs]

|       |   |   |   `-- DataSources/

|       |   |   `-- Resources/

|       |   `-- TestUsingNormalAuthentication/  \[3 cs]  <Program.cs, TestUsingNormalAuthentication.csproj, app.config>

|       |       `-- Properties/  \[3 cs]

|       `-- InnovationClassLibrary/  <InnovationClassLibrary.sln>

|           |-- EPI/  <app.config>

|           |   |-- BLL/

|           |   |-- DAL/

|           |   `-- My Project/

|           `-- EPI\_TEST/

|               `-- My Project/

|-- Innovation.PrinterService.API/  \[1 cs]  <Innovation.PrinterService.API.csproj, Program.cs, appsettings.json>

|   |-- BackgroundService/  \[1 cs]

|   |-- Controllers/  \[5 cs]

|   |-- Helper/  \[1 cs]

|   |-- Models/  \[4 cs]

|   |-- Properties/

|   |   `-- PublishProfiles/

|   |-- Service/  \[3 cs]

|   |-- Views/

|   |   |-- Account/

|   |   |-- Printer/

|   |   `-- Shared/

|   `-- wwwroot/

|       |-- css/

|       |-- js/

|       `-- lib/

|           |-- bootstrap/

|           |   `-- dist/

|           |       |-- css/

|           |       `-- js/

|           |-- jquery/

|           |-- jquery-validation/

|           `-- jquery-validation-unobtrusive/

|               `-- dist/

|               `-- dist/

|-- Innovation.ReportServices/  <Innovation.ReportServices.csproj>

|   `-- Service/  \[1 cs]

|-- Innovation.Repositories/  <Innovation.Repositories.sln>

|   |-- ConsoleApp1/  \[1 cs]  <GenerateContext.csproj, Program.cs>

|   |   `-- DBContext/

|   |-- Innovation.Repository/  <Innovation.Repository.csproj>

|   |   |-- DBContext/  \[16 cs]

|   |   |-- GenericRepository/  \[1 cs]

|   |   |   |-- CentralDB/  \[5 cs]

|   |   |   |-- DBCenter/  \[115 cs]

|   |   |   |-- DBMaster/  \[408 cs]

|   |   |   |-- DBMasterHist/  \[67 cs]

|   |   |   |-- DBSdb/  \[5 cs]

|   |   |   |-- DBTransection/  \[264 cs]

|   |   |   |-- DBTransectionHist/  \[55 cs]

|   |   |   |-- DB\_Auto\_Report/  \[3 cs]

|   |   |   |-- DataCenter/  \[190 cs]

|   |   |   |-- Data\_Center\_Master/  \[3 cs]

|   |   |   |-- Material\_Management/  \[10 cs]

|   |   |   |-- ProductionSTD/  \[5 cs]

|   |   |   `-- SILO/  \[13 cs]

|   |   |-- Helper/  \[1 cs]

|   |   |-- OldRepositories/  \[16 cs]

|   |   |   `-- DataCenterGenericRepository/  \[2 cs]

|   |   |-- RepositoryFactory/  \[21 cs]

|   |   `-- UnitOfWork/  \[13 cs]

|   `-- Innovation.UnitOfWork/  \[3 cs]  <Innovation.UnitOfWork.csproj>

|-- Innovation.Services/  <Innovation.Services.sln>

|   |-- Innovation.Authentication/  \[1 cs]  <Innovation.Authentication.csproj, Program.cs>

|   |   |-- Infrastructure/  \[1 cs]

|   |   |-- Properties/

|   |   `-- Service/  \[2 cs]

|   `-- Innovation.Services/  <Innovation.Services.csproj>

|       |-- BackgroundService/  \[2 cs]

|       |-- Domain/

|       |   |-- DBMaster/  \[2 cs]

|       |   `-- DBTransection/  \[4 cs]

|       |-- Helper/  \[2 cs]

|       |-- Properties/

|       |   |-- DataSources/

|       |   `-- PublishProfiles/

|       |-- Report/

|       |   `-- Extension/  \[1 cs]

|       `-- Service/  \[77 cs]

|-- Innovation.UtilityCore/  <Innovation.UtilityCore.sln>

|   |-- Innovation.UtilityCore.CustomDetailMessageBox/  \[2 cs]  <App.config, Innovation.UtilityCore.CustomDetailMessageBox.csproj>

|   |   |-- Presenter/  \[3 cs]

|   |   |-- Properties/  \[3 cs]

|   |   |-- UI/  \[4 cs]

|   |   `-- ViewModel/  \[1 cs]

|   `-- Innovation.UtilityCore.Helper/  \[17 cs]  <App.config, Innovation.UtilityCore.Helper.csproj>

|       `-- Properties/  \[3 cs]

`-- Innovation.WebApp/  <Innovation.WebApp.sln>

&#x20;   |-- Innovation.PlanOnline/  \[1 cs]  <Innovation.PlanOnline.csproj, Program.cs, appsettings.json>

&#x20;   |   |-- Areas/

&#x20;   |   |   `-- Identity/  \[1 cs]

&#x20;   |   |       `-- Pages/

&#x20;   |   |           |-- Account/

&#x20;   |   |           `-- Shared/

&#x20;   |   |-- Data/  \[1 cs]

&#x20;   |   |   `-- Migrations/  \[3 cs]

&#x20;   |   |-- Pages/  \[1 cs]

&#x20;   |   |   `-- PlanOnline/  \[1 cs]

&#x20;   |   |-- Properties/

&#x20;   |   |   `-- PublishProfiles/

&#x20;   |   |-- Shared/

&#x20;   |   `-- wwwroot/

&#x20;   |       `-- css/

&#x20;   |           |-- bootstrap/

&#x20;   |           `-- open-iconic/

&#x20;   |               `-- font/

&#x20;   |                   |-- css/

&#x20;   |                   `-- fonts/

&#x20;   `-- Innovation.WIOnline/  \[1 cs]  <Innovation.WIOnline.csproj, Program.cs, appsettings.json>

&#x20;       |-- Const/  \[1 cs]

&#x20;       |-- Controllers/  \[3 cs]

&#x20;       |-- Models/  \[8 cs]

&#x20;       |-- Properties/

&#x20;       |   `-- PublishProfiles/

&#x20;       |-- Services/  \[2 cs]

&#x20;       |-- Shared/

&#x20;       |   |-- constant/

&#x20;       |   |-- css/

&#x20;       |   |-- favicon/

&#x20;       |   |-- fonts/

&#x20;       |   |   `-- Poppins/

&#x20;       |   |-- images/

&#x20;       |   |-- js/

&#x20;       |   |   `-- maps/

&#x20;       |   |-- lib/

&#x20;       |   |-- pages/

&#x20;       |   |   |-- charts/

&#x20;       |   |   |-- documentation/

&#x20;       |   |   |-- forms/

&#x20;       |   |   |-- icons/

&#x20;       |   |   |-- samples/

&#x20;       |   |   |-- tables/

&#x20;       |   |   `-- ui-features/

&#x20;       |   |-- partials/

&#x20;       |   |-- templates/

&#x20;       |   |-- tool/

&#x20;       |   |   `-- confirm-modal/

&#x20;       |   `-- vendors/

&#x20;       |       |-- codemirror/

&#x20;       |       |-- css/

&#x20;       |       |-- font-awesome/

&#x20;       |       |   |-- css/

&#x20;       |       |   `-- fonts/

&#x20;       |       |-- jquery-file-upload/

&#x20;       |       |-- js/

&#x20;       |       |-- jsgrid/

&#x20;       |       |-- mdi/

&#x20;       |       |   |-- css/

&#x20;       |       |   `-- fonts/

&#x20;       |       |-- pwstabs/

&#x20;       |       |-- quill/

&#x20;       |       |-- select2/

&#x20;       |       |-- simplemde/

&#x20;       |       `-- typicons/

&#x20;       |           `-- fonts/

&#x20;       |-- Utility/  \[1 cs]

&#x20;       |-- ViewModel/  \[1 cs]

&#x20;       |-- Views/

&#x20;       |   |-- App/

&#x20;       |   |   |-- AutoMix/

&#x20;       |   |   |   |-- MixerProfile/

&#x20;       |   |   |   `-- Shared/

&#x20;       |   |   |-- Home/

&#x20;       |   |   |-- Layout/

&#x20;       |   |   |-- Login/

&#x20;       |   |   |-- ProductionAnalyst/

&#x20;       |   |   |-- SelectSite/

&#x20;       |   |   |-- UploadFileCenter/

&#x20;       |   |   |   `-- uploadfilepicture/

&#x20;       |   |   `-- WiOnline/

&#x20;       |   |       |-- PDD/

&#x20;       |   |       |-- ReviseFormularion/

&#x20;       |   |       |-- STDLayout/

&#x20;       |   |       |   |-- ModalComponent/

&#x20;       |   |       |   `-- TabComponent/

&#x20;       |   |       `-- STDMainApprove/

&#x20;       |   |           `-- TabComponent/

&#x20;       |   |-- PlanOnline/

&#x20;       |   |-- Shared/  \[1 cs]

&#x20;       |   |   `-- Component/

&#x20;       |   `-- View/

&#x20;       `-- wwwroot/

&#x20;           `-- planonline/

&#x20;               |-- css/

&#x20;               |   |-- bootstrap/

&#x20;               |   |-- jquery/

&#x20;               |   `-- open-iconic/

&#x20;               |       `-- font/

&#x20;               |           |-- css/

&#x20;               |           `-- fonts/

&#x20;               |-- images/

&#x20;               `-- js/

&#x20;                   `-- jquery/

```



\### สิ่งที่อ่านออกจากทรี



\- `Innovation.Core/DataModel/` แบ่งย่อย \*\*ตามฐานข้อมูล\*\* ส่วน `DtoModel/` แบ่งทั้งตามฐานข้อมูลและตามฟีเจอร์

\- `GenericRepository/DBMaster/` มี 408 ไฟล์ = \*\*หนึ่งคลาสต่อหนึ่งตาราง\*\* ทั้งหมดเป็น shim บางๆ

\- `Innovation.Library/` มี `Dev/` กับ `Main/` เป็นสแนปช็อตของ branch ที่เกือบเหมือนกัน — ดู §11

\- แต่ละโปรเจกต์มี `.sln` ของตัวเองด้วย นอกเหนือจาก `BackEnd.sln` รวม



\---



\## 3. Interface contracts



ทั้งหมดอยู่ใน `Innovation.Core/Innovation.Core/Core/`



\### 3.1 Repository — มีสองตัว ตัวหนึ่งตายแล้ว



`Core/Repository/IRepository.cs` — \*\*ตัวที่ใช้งานจริง\*\*



```csharp

public interface IRepository<T> where T : class, new()

{

&#x20;   T Get(object id);

&#x20;   T Find(Expression<Func<T, bool>> predicate);

&#x20;   IQueryable<T> GetAll();

&#x20;   IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate);

&#x20;   void Add(T entity);

&#x20;   void AddRange(IEnumerable<T> entities);

&#x20;   void BulkInsert(List<T> entities);

&#x20;   void Update(T entity);

&#x20;   void TryUpdate(T entity);

&#x20;   void UpdateRange(IEnumerable<T> entities);

&#x20;   void TryUpdateRange(IEnumerable<T> entities);

&#x20;   void BulkUpdate(List<T> entities);

&#x20;   void Delete(T entity);

&#x20;   void DeleteRange(IEnumerable<T> entities);

&#x20;   void BulkDelete(List<T> entities);

&#x20;   List<dynamic> GetDynamicBySql(string sql, params object\[] parameters);

&#x20;   int GetMaxPK(Expression<Func<T, int>> predicate);

}

```



`Core/Repository/IGenericRepository.cs` — \*\*legacy ที่ยังลงทะเบียนใน DI แต่ไม่มีใคร inject\*\*



```csharp

public interface IGenericRepository<T> where T : class, new()

{

&#x20;   T Get(string id);

&#x20;   T Get(int id);

&#x20;   T Find(Expression<Func<T, bool>> predicate);

&#x20;   IEnumerable<T> GetAll();

&#x20;   IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate);

&#x20;   void Add(T entity);

&#x20;   void Add(IEnumerable<T> entities);

&#x20;   void Update(T entity);

&#x20;   void Update(IEnumerable<T> entities);

&#x20;   void Delete(T entity);

&#x20;   void Delete(IEnumerable<T> entities);

&#x20;   EntityState GetEntityState(T Entity);

}

```



ต่างกันตรง: ตัวเก่าคืน `IEnumerable` (materialize ทันที) และ \*\*เรียก `SaveChanges()` ข้างในทุก write\*\*

ส่วนตัวใหม่คืน `IQueryable` (deferred), มี bulk operation, และไม่ save เอง — ปล่อยให้ UnitOfWork จัดการ



`Program.cs` ลงทะเบียน `AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))` ไว้

แต่ไม่มี controller หรือ service ตัวไหน inject มันเลย — \*\*เป็น registration ที่ตายแล้ว\*\*



\### 3.2 UnitOfWork



```csharp

// Core/UnitOfWork/IUnitOfWork.cs

public interface IUnitOfWork : IDisposable

{

&#x20;   bool Save();

}



// Core/UnitOfWork/Context/IContextUnitOfWork.cs

public interface IContextUnitOfWork : IDisposable, IUnitOfWork

{

&#x20;   bool CheckConnection();

&#x20;   void BeginTransaction();

&#x20;   void CommitTransaction();

&#x20;   void RollbackTransaction();

&#x20;   void DetachAllEntities();

}

```



จากนั้นมี \*\*หนึ่ง interface ต่อหนึ่งฐานข้อมูล\*\* รวม 13 ตัวใน `Core/UnitOfWork/Context/` แต่ละตัวเปิด

property ของ repository ที่พิมพ์ชนิดไว้แล้ว ตัวอย่างเต็มของ `ISiloUnitOfWork` (ตัวที่ demo slice ใช้):



```csharp

public interface ISiloUnitOfWork : IContextUnitOfWork

{

&#x20;   IKbTogetherRepository KbTogetherRepository { get; }

&#x20;   IRmConfirmMstRepository RmConfirmMstRepository { get; }

&#x20;   IKbPlcSortRepository KbPlcSortRepository { get; }

&#x20;   IWeightingRepository WeightingRepository { get; }

&#x20;   ITotalWeightRepository TotalWeightRepository { get; }

&#x20;   ISendStepParameterRepository SendStepParameterRepository { get; }

&#x20;   ITrayPlanRepository TrayPlanRepository { get; }

&#x20;   ITrayWeightRepository TrayWeightRepository { get; }

&#x20;   ITwAcceptWeightHisRepository TwAcceptWeightHisRepository { get; }

&#x20;   ITypeTrayRepository TypeTrayRepository { get; }

&#x20;   ITrayBarcodeRepository TrayBarcodeRepository { get; }

&#x20;   IStationRepository StationRepository { get; }

&#x20;   IUsrWtRepository UsrWtRepository { get; }

}

```



อีก 12 ตัวรูปแบบเดียวกัน: `ICentralDbUnitOfWork`, `IDataCenterUnitOfWork`,

`IDataCenterMasterUnitOfWork`, `IDBAutoReportUnitOfWork`, `IDBCenterUnitOfWork`,

`IDBMasterUnitOfWork` (427 บรรทัด), `IDBMasterHistUnitOfWork`, `IDBSdbUnitOfWork`,

`IDBTransectionUnitOfWork` (301 บรรทัด), `IDBTransectionHistUnitOfWork`,

`IMaterialManagementUnitOfWork`, `IProductionStdUnitOfWork`



\### 3.3 Repository ต่อ entity — เป็น marker interface ล้วนๆ



```csharp

// Core/Repository/SILO/IKbTogetherRepository.cs

namespace Innovation.Core.Core.Repository.SILO

{

&#x20;   public interface IKbTogetherRepository : IRepository<KbTogether> { }

}

```



แทบทุกตัวว่างเปล่าแบบนี้ ทำหน้าที่แค่ปิด generic ให้เป็นชนิดที่ระบุได้



\### 3.4 Service — ไม่มี base interface



\*\*ไม่มี `IBaseService` หรือ `IGenericService<T>`\*\* — `Core/Service/` มี interface แยกตามโมดูล \*\*68 ตัว\*\*

generic abstraction ตัวเดียวที่มีคือของ background service:



```csharp

public interface IInnovationBaseBackgroundService<T>

{

&#x20;   ILogger<T> Logger { get; set; }

&#x20;   Timer TimerWork { get; set; }

&#x20;   int ServiceDataId { get; set; }

&#x20;   int GetServiceTimerInterval();

&#x20;   string GetServiceName();

}

```



ตัวอย่าง service contract จริง — สังเกต \*\*`int siteId` เป็นพารามิเตอร์ตัวแรกแทบทุกเมธอด\*\*

การ route ไปคนละไซต์จึงเป็นเรื่องระดับ signature ไม่ใช่เรื่อง infrastructure:



```csharp

public interface ISiloApproveService

{

&#x20;   GeneralDataSiloApproveDto GetGeneralDataSiloApprove(int siteId, string Serial\_Number, string Line\_id);

&#x20;   List<ListSiloNameDto> GetlistSiloName(int siteId, int LineId, int itemId);

&#x20;   string InsertSiloApprove(int siteId, int onhandId, SiloApproveDto siloAppInsert);

&#x20;   bool DeleteSiloApprove(int siteId, int SiloApp\_Id);

&#x20;   bool InsertSiloHistByDelete(int siteId, siloApproveDto siloApproveHistInsert);

&#x20;   LineDto GetLineName(int siteId, int LineID);

&#x20;   GetAddressPlcDto GetPlcAddress(int siteId, string programCode, int stationId, int lineId);

&#x20;   GetLineStationDto GetLineStation(int siteId);

}

```



> สังเกต `SiloApproveDto` กับ `siloApproveDto` — ตัวพิมพ์เล็กใหญ่ต่างกัน เป็นคนละชนิดจริงๆ



\### 3.5 Repository รุ่นเก่าแบบไม่ generic



ที่ราก `Core/Repository/` ยังมี interface ต่อฐานข้อมูลแบบเขียน SQL/LINQ เอง:

`ICentralDBRepository`, `IDBSdbRepository`, `IDataCenterRepository`, `IDataCenterMasterRepository`,

`IMasterPOPRPanRepository`, `IMaterialManagementRepository`, `IPackingRepository`,

`IProductionSTDRepository`, `IQCDBRepository`, `ISILORepository`

ตัวเหล่านี้คุม transaction เอง (`SaveChange()`, `BeginTransaction()` อยู่บน interface)



\---



\## 4. Base classes และรูปแบบหลัก



\### 4.1 `RepositoryImpl<T>` — implementation เดียวรับใช้ทุกฐานข้อมูล



```csharp

public class RepositoryImpl<T> : IRepository<T> where T : class, new()

{

&#x20;   protected readonly DbContext context;

&#x20;   public RepositoryImpl(DbContext context) { this.context = context; }



&#x20;   public T Get(object id)

&#x20;   {

&#x20;       var entity = context.Set<T>().Find(id);

&#x20;       if (entity != null) context.Entry(entity).State = EntityState.Detached;

&#x20;       return entity;

&#x20;   }

&#x20;   public IQueryable<T> GetAll() => context.Set<T>().AsNoTracking();

&#x20;   public T Find(Expression<Func<T, bool>> p) => context.Set<T>().AsNoTracking().Where(p).FirstOrDefault();

&#x20;   public void BulkInsert(List<T> entities) { context.BulkInsert(entities); }   // EFCore.BulkExtensions

&#x20;   public int GetMaxPK(Expression<Func<T, int>> p) => context.Set<T>().DefaultIfEmpty().Max(p);

&#x20;   public void TryUpdate(T entity)

&#x20;   {

&#x20;       var entries = context.ChangeTracker.Entries()

&#x20;           .Where(e => e.Entity.GetType().Name == entity.GetType().Name).ToList();

&#x20;       entries.ForEach(x => x.State = EntityState.Detached);

&#x20;       context.Set<T>().Update(entity);

&#x20;   }

}

```



คุณสมบัติสำคัญสามข้อ: constructor รับ `DbContext` ตัวฐาน (จึงใช้ได้กับทุกฐานข้อมูล),

อ่านด้วย `AsNoTracking()` เสมอ, และ\*\*ไม่เคยเรียก `SaveChanges()`\*\*



คลาสลูกต่อ entity เป็น shim ที่ต่างกันแค่ชนิดที่ปิด generic และ context ที่รับ:



```csharp

internal class KbTogetherRepository : RepositoryImpl<KbTogether>, IKbTogetherRepository

{

&#x20;   public KbTogetherRepository(SILOContext context) : base(context) { }

}

```



ทุกตัวเป็น `internal` — โลกภายนอกเข้าถึงได้ผ่าน UnitOfWork เท่านั้น



\### 4.2 การ resolve context — โซ่สามชั้น



`UnitOfWorkFactory` (public static) → `DbContextFactory` / `DbContextConnectSiteInvoice` (internal static)

→ `<Db>RepositoryFactory` (internal static)



```csharp

// RepositoryFactory/UnitOfWorkFactory.cs — ทางเข้าสาธารณะทางเดียว

public static ISiloUnitOfWork GetSiloUnitOfWork()

{

&#x20;   SILOContext siloContext = DbContextConnectSiteInvoice.GetSiloDbContext();

&#x20;   return new SiloUnitOfWork(siloContext);

}



public static IDBTransectionUnitOfWork GetDBTransectionUnitOfWork(int connectionSiteId)

{

&#x20;   DBTransectionContext ctx = DbContextFactory.GetDBTransectionDbContext(connectionSiteId);

&#x20;   return new DBTransectionUnitOfWork(ctx);

}



public static IDBTransectionUnitOfWork GetDBTransectionUnitOfWork(string connectionString)

{

&#x20;   return new DBTransectionUnitOfWork(new DBTransectionContext(connectionString));

}

```



แต่ละฐานข้อมูลมี overload สามแบบ: `()` = ไซต์ปริยาย, `(int connectionSiteId)` = route ตามไซต์,

`(string connectionString)` = ระบุเอง



```csharp

internal static class DbContextFactory

{

&#x20;   private static MDBCON \_mdbCon = new MDBCON();

&#x20;   private static DSDBCON \_dbCon = new DSDBCON();

&#x20;   private static DataBaseConnectionCore erpConn = new DataBaseConnectionCore();



&#x20;   public static DBTransectionContext GetDBTransectionDbContext(int connSite)

&#x20;   {

&#x20;       SqlConnection conn = erpConn.DBConnection(connSite, DBConnection.DB\_TRANSECTION);

&#x20;       return new DBTransectionContext(conn.ConnectionString);

&#x20;   }

}

```



> มี connection provider \*\*สามตัว\*\* — ฐาน ERP (`DBCenter`/`DBMaster`/`DBTransection`) ผ่าน `erpConn`

> ที่เหลือผ่าน `\_dbCon` ชื่อฐานข้อมูลเป็นค่าคงที่ใน `RepositoryFactory/DBConnection.cs`



UnitOfWork ถือ context หนึ่งตัวและสร้าง repository แบบ lazy:



```csharp

internal class SiloUnitOfWork : ISiloUnitOfWork

{

&#x20;   private readonly SILOContext \_context;

&#x20;   public SiloUnitOfWork(SILOContext context) { \_context = context; }



&#x20;   private IKbTogetherRepository \_kbTogetherRepository;

&#x20;   public IKbTogetherRepository KbTogetherRepository

&#x20;       => \_kbTogetherRepository ?? (\_kbTogetherRepository = SILORepositoryFactory.GetKbTogetherRepository(\_context));



&#x20;   public bool Save() => \_context.SaveChanges() > 0;

&#x20;   public void BeginTransaction()    { \_context.Database.BeginTransaction(); }

&#x20;   public void CommitTransaction()   { \_context.Database.CommitTransaction(); }

&#x20;   public void RollbackTransaction() { \_context.Database.RollbackTransaction(); }

&#x20;   public bool CheckConnection()     => \_context.Database.CanConnect();

&#x20;   public void DetachAllEntities()

&#x20;   {

&#x20;       \_context.ChangeTracker.Entries().Where(e => e.State != EntityState.Detached)

&#x20;           .ToList().ForEach(x => x.State = EntityState.Detached);

&#x20;   }

&#x20;   public void Dispose() { \_context.Dispose(); }

}

```



\### 4.3 `DbContext` — รูปแบบเดียวกันทั้ง 16 ตัว



```csharp

public partial class DBTransectionContext : DbContext

{

&#x20;   public DBTransectionContext() { }

&#x20;   public DBTransectionContext(string connectionString) : base(GetOptions(connectionString)) { }



&#x20;   private static DbContextOptions GetOptions(string connectionString)

&#x20;       => SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;



&#x20;   public DBTransectionContext(DbContextOptions<DBTransectionContext> options) : base(options) { }



&#x20;   public virtual DbSet<TTRParameterSpecReport> TTRParameterSpecReport { get; set; }

&#x20;   // ...

}

```



\*\*ไม่มี `OnConfiguring` override\*\* — connection string เข้ามาทาง `GetOptions` เท่านั้น

ทุกตัวเป็นผลลัพธ์จาก EF scaffold ขนาดรวม \*\*163,795 บรรทัด\*\*



\### 4.4 `BaseAPIController`



```csharp

\[AllowAnonymous]

\[ApiExplorerSettings(IgnoreApi = true)]

\[ApiController]

public class BaseAPIController : ControllerBase

{

&#x20;   \[NonAction] public ObjectResult ResponseError(Exception ex);

&#x20;   \[NonAction] public ObjectResult ResponseOK();

&#x20;   \[NonAction] public ObjectResult ResponseOK<T>(T value);

&#x20;   \[NonAction] public ObjectResult ResponseNotFound(string Msg);

&#x20;   \[NonAction] public ObjectResult ResponseBadRequest(string Msg);

&#x20;   \[NonAction] public ObjectResult ResponseUnauthorized(string Msg);

&#x20;   \[NonAction] public ActionResult<int?> GetSiteAPI();

&#x20;   \[NonAction] public static string SerializeObjectWithoutNulls(object obj);

&#x20;   \[NonAction] public static void ErrorLog(HttpContext httpContext, Exception error, string RequestPath = null);

}

```



\- `GetSiteAPI()` อ่าน \*\*HTTP header ชื่อ `SiteID`\*\* คืน `int?` (โยน `"Headers SiteID is Number."` ถ้าแปลงไม่ได้)

&#x20; นี่คือจุดเชื่อมของการ route หลายไซต์

\- `ErrorLog` ดึง body ดิบจาก `httpContext.Items\["RawBody"]` (middleware เป็นคนใส่ให้) แล้วเขียนลง

&#x20; \*\*UNC path ที่ฝังตายในโค้ด\*\* `\\\\FILE-SERVER\\APILogs` (บรรทัดที่อ่านจาก `IConfiguration` ถูกคอมเมนต์ทิ้งไว้ข้างบน)

\- \*\*controller ส่วนใหญ่ไม่ได้สืบทอดคลาสนี้\*\* — สืบจาก `ControllerBase` หรือ `Controller` ตรงๆ

&#x20; envelope `MyDataAPI<T>` จึงถูกใช้ไม่สม่ำเสมอ



\### 4.5 Background service — ขับด้วยฐานข้อมูล



```csharp

public class InnovationBaseBackgroundService<T> : BackgroundService, IInnovationBaseBackgroundService<T>

{

&#x20;   private readonly IDBCenterUnitOfWork \_dbCenterUnitOfWork;

&#x20;   public ILogger<T> Logger { get; set; }

&#x20;   public System.Timers.Timer TimerWork { get; set; }

&#x20;   public int ServiceDataId { get; set; }



&#x20;   public InnovationBaseBackgroundService()

&#x20;   {

&#x20;       \_dbCenterUnitOfWork = UnitOfWorkFactory.GetDBCenterUnitOfWork();

&#x20;       TimerWork = new System.Timers.Timer();

&#x20;       TimerWork.Elapsed += OnTimerEvent;

&#x20;   }



&#x20;   public int GetServiceTimerInterval()

&#x20;   {

&#x20;       var t = \_dbCenterUnitOfWork.ServiceDataMstRepository.Find(d => d.Id == ServiceDataId)?.TimerInterval;

&#x20;       if (t == null) throw new ArgumentException();

&#x20;       return (int)t \* 1000;

&#x20;   }

&#x20;   public bool GetServiceState()

&#x20;       => \_dbCenterUnitOfWork.ServiceDataMstRepository

&#x20;            .GetWhere(d => d.Id == ServiceDataId \&\& d.IsActive == true).Any();



&#x20;   private void OnTimerEvent(object sender, ElapsedEventArgs e)

&#x20;   {

&#x20;       TimerWork.Stop();

&#x20;       try { Work(e); }

&#x20;       catch (Exception ex) { Logger.LogError(ex.ToString()); }

&#x20;       finally { TimerWork.Start(); }

&#x20;   }

&#x20;   protected virtual void Work(ElapsedEventArgs e) { }

}

```



\*\*ทั้งคลาสนี้และคลาสลูกประกาศไว้ที่ global scope — ไม่มี namespace\*\*



รอบเวลาและสถานะเปิด/ปิดมาจากตาราง `ServiceDataMst` ในฐาน `DBCenter` โดยใช้ `ServiceDataId` เป็นคีย์

ตัวเดียวที่ลงทะเบียนจริงคือ `ServiceTransectionOnHandFIFOBackgroundService` (`ServiceDataId = 53`)

ซึ่งคำนวณสต๊อกคงเหลือแบบ FIFO ใหม่ห้าส่วน: RM ของบริษัท, RM ของลูกค้า, semi-finished ของบริษัท,

semi-finished ของลูกค้า, และสต๊อกคลัง/พนักงานผลิต



\### 4.6 ชนิดห่อหุ้ม (wrapper types)



```csharp

public class MyDataAPI<T>

{

&#x20;   public bool Success { get; set; }

&#x20;   public int StatusCode { get; set; }

&#x20;   public T Data { get; set; }

&#x20;   public string Messenger { get; set; }      // สะกดผิด — ที่ถูกคือ Message

&#x20;   public object ErrorException { get; set; }

}



public class ApiData<T> { public int Site\_ID { get; set; } public T MyData { get; set; } }



public class ApiErrorResponse { public string Type; public string Message; public string StackTrace; }



public class ErrorExceptionHandling : Exception

{

&#x20;   public ApiErrorResponse ErrorResponse { get; set; }

&#x20;   public ErrorExceptionHandlingEnum ErrorType { get; set; }   // { Info, Werning, Error } — Werning สะกดผิด

}

```



`ApiData<T>` คือ envelope ของ \*\*request\*\* สำหรับ PUT/POST ส่วน `MyDataAPI<T>` คือของ response



\---



\## 5. ตัวอย่างฟีเจอร์ครบวงจร — SiloApprove



เลือกตัวนี้เพราะขนาดกลางและผ่านครบทุกชั้น



\*\*Controller\*\* — `Innovation.API/Controllers/SiloApproveController.cs`



```csharp

\[Route("api/\[controller]/\[action]")]

\[ApiController]

public class SiloApproveController : ControllerBase

{

&#x20;   private readonly IDBMasterService \_masterService;

&#x20;   ISiloApproveService \_siloApproveService;



&#x20;   public SiloApproveController(ISiloApproveService siloApproveService, IDBMasterService masterService, ...) { }



&#x20;   \[HttpGet]

&#x20;   public ActionResult<GeneralDataSiloApproveDto> GetGeneralDataSiloApprove(int siteId, string serialNo, string lineId)

&#x20;       => \_siloApproveService.GetGeneralDataSiloApprove(siteId, serialNo, lineId);

}

```



\*\*Service\*\* — `Innovation.Services/Service/InnovationSiloApproveService.cs` (617 บรรทัด)



```csharp

public class InnovationSiloApproveService : ISiloApproveService

{

&#x20;   private IDBTransectionUnitOfWork \_dbTransactionunitOfWork;

&#x20;   private IDBMasterUnitOfWork \_dbMasterUnitOfWork;

&#x20;   private IDBTransectionHistUnitOfWork \_dbTranHistUnitOfWork;

&#x20;   private DBTransectionDomainService \_dbTransectionService;

&#x20;   private Dictionary<int, IDBTransectionUnitOfWork> \_dbTransectionUnitOfWorkPerSiteServerDict

&#x20;       = new Dictionary<int, IDBTransectionUnitOfWork>();



&#x20;   public InnovationSiloApproveService()          // ← constructor ไม่รับอะไรเลย

&#x20;   {

&#x20;       \_dbTransactionunitOfWork = UnitOfWorkFactory.GetDBTransectionUnitOfWork();

&#x20;       \_dbMasterUnitOfWork      = UnitOfWorkFactory.GetDBMasterUnitOfWork();

&#x20;       \_dbTranHistUnitOfWork    = UnitOfWorkFactory.GetDBTransectionHistUnitOfWork();

&#x20;       \_dbTransectionService    = new DBTransectionDomainService(\_dbMasterUnitOfWork, \_dbTransactionunitOfWork);

&#x20;   }



&#x20;   private void SetUnitOfWorkBySite(int siteId)

&#x20;   {

&#x20;       \_dbTransactionunitOfWork = UnitOfWorkFactory.GetDBTransectionUnitOfWork(siteId);

&#x20;       if (!\_dbTransectionUnitOfWorkPerSiteServerDict.ContainsKey(siteId))

&#x20;       {

&#x20;           \_dbTransectionUnitOfWorkPerSiteServerDict.Add(siteId, UnitOfWorkFactory.GetDBTransectionUnitOfWork(siteId));

&#x20;           \_dbTransectionUnitOfWorkPerSiteServerDict\[siteId].BeginTransaction();

&#x20;       }

&#x20;       \_dbTransectionService = new DBTransectionDomainService(

&#x20;           \_dbMasterUnitOfWork, \_dbTransectionUnitOfWorkPerSiteServerDict\[siteId]);

&#x20;   }

}

```



\*\*ทุกเมธอดสาธารณะเริ่มด้วย `SetUnitOfWorkBySite(siteId);`\*\* — นั่นคือขั้นตอน route ตามไซต์

จากนั้นจึงเข้าถึงข้อมูล:



```csharp

var \_silo = \_dbTransactionunitOfWork.SiloApproveRepository.GetWhere(x => x.Id == SiloApp\_Id).FirstOrDefault();

\_dbTransactionunitOfWork.SiloApproveRepository.Delete(\_silo);

\_dbTransactionunitOfWork.Save();

```



\*\*Domain service\*\* ที่คั่นกลาง — `Innovation.Services/Domain/DBTransection/DBTransectionDomainService.cs`



```csharp

internal class DBTransectionDomainService : IDBTransectionDomainService

{

&#x20;   private const int APP\_ID = 186;

&#x20;   public DBTransectionDomainService(IDBMasterUnitOfWork m, IDBTransectionUnitOfWork t) { }



&#x20;   public OnHandDto GetOnHandBySerialLacation(int siteId, string serialNo, int LocationId)  // Lacation สะกดผิด

&#x20;   {

&#x20;       ChangeConnection(siteId);

&#x20;       return \_dbTransectionUnitOfWork.OnHandRepository.GetWhere(...)

&#x20;           .Select(x => DataMappingHelper.GetSimpleDataMap<OnHand, OnHandDto>(x))

&#x20;           .FirstOrDefault();

&#x20;   }

}

```



\### โซ่ทั้งหมด



```

Controller (DI ให้ I\*Service)

&#x20; └─ Service ctor เรียก UnitOfWorkFactory.Get<Db>UnitOfWork(\[siteId])   ← static ไม่ใช่ DI

&#x20;      └─ DbContextFactory หา SqlConnection ของคู่ (site, dbName)

&#x20;           └─ new <Db>Context(connectionString)

&#x20;                └─ <Db>UnitOfWork สร้าง <Entity>Repository : RepositoryImpl<T> แบบ lazy

&#x20;                     └─ LINQ บน DbSet<TEntity>

&#x20;                          └─ unitOfWork.Save() → \_context.SaveChanges() > 0

```



> \*\*จุดสำคัญที่สุดของสถาปัตยกรรมนี้: DI กับ factory แยกกัน\*\*

> controller ได้ service มาจาก DI container แต่ service \*\*`new` UnitOfWork เองจาก static factory\*\*

> ผลคือ service ตัวใดก็ตามที่แตะฐานข้อมูลจะ mock ไม่ได้ และเขียน unit test ไม่ได้

> `DrawingReturnService` มี constructor ทั้งสองแบบอยู่ข้างกัน — แบบไม่รับอะไร (เรียก factory)

> กับแบบรับ UnitOfWork ทาง parameter (สำหรับ inject/test) แต่ตัวหลัง DI ไม่เคยเรียกใช้



\---



\## 6. Composition root



`Innovation.API/Program.cs` — 188 บรรทัด แบบ minimal hosting ไม่มี `Startup` ที่ใช้งานจริง



```csharp

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<ISCADAReportViewerService, SCADAReportViewerService>();

builder.Services.AddScoped<IDataCenterMasterService, DataCenterMasterService>();

// ... รวมประมาณ 65 บรรทัดแบบเดียวกัน เรียงแบนๆ ไม่มีการจัดกลุ่ม ...

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));   // ตายแล้ว



builder.Services.AddSingleton<ICompositeViewEngine, CompositeViewEngine>();

builder.Services.AddSingleton<ITempDataProvider, SessionStateTempDataProvider>();

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddScoped<IReportService, ReportRenderService>();

builder.Services.AddHostedService<ServiceTransectionOnHandFIFOBackgroundService>();



builder.Services.AddControllers().AddBadRequestServices();

builder.Services.AddCors(o => o.AddPolicy("AllowAllOrigins",

&#x20;   b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.Configure<JsonOptions>(o =>

&#x20;   o.SerializerOptions.Converters.Add(new CustomLocalDateTimeConverter()));



var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseExceptionHandler("/error");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<RequestBodyLoggingMiddleware>();

app.UseCors("AllowAllOrigins");

ReportRenderService.CleanUpReportTempFolders();

app.MapControllers();

app.Run();

```



\- \*\*`Startup.cs` (70 บรรทัด) เป็นซากที่ตายแล้ว\*\* เข้าถึงได้ทาง `CreateWebHostBuilder` ที่ไม่มีใครเรียก

\- lifetime: service ทั้งหมด `Scoped`, PDF converter / view engine เป็น `Singleton`

\- มีไฟล์ตั้งค่าตามสภาพแวดล้อม 9 ไฟล์ `appsettings.<env>.json`



\### Middleware ตัวเดียวที่มี



```csharp

public class RequestBodyLoggingMiddleware

{

&#x20;   public async Task Invoke(HttpContext context)

&#x20;   {

&#x20;       context.Request.EnableBuffering();

&#x20;       context.Request.Body.Position = 0;

&#x20;       using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);

&#x20;       context.Items\["RawBody"] = await reader.ReadToEndAsync();

&#x20;       context.Request.Body.Position = 0;

&#x20;       await \_next(context);

&#x20;   }

}

```



มีหน้าที่เดียวคือเก็บ body ดิบไว้ให้ `BaseAPIController.ErrorLog`

\*\*สังเกตลำดับ: ลงทะเบียน \*หลัง\* `UseExceptionHandler` แล้ว\*\* ซึ่งเป็นลำดับที่ผิด



\### ตัวจัดการ error ส่วนกลาง



```csharp

\[Route("error")]

public ApiErrorResponse Error()

{

&#x20;   var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

&#x20;   var exception = context.Error;

&#x20;   Response.StatusCode = 500;

\#if !DEBUG

&#x20;   BaseAPIController.ErrorLog(HttpContext, exception, context.Path);

\#endif

&#x20;   if (exception is ErrorExceptionHandling error)

&#x20;   {

&#x20;       error.ErrorResponse.Type = typeof(WebException).Name;

&#x20;       error.ErrorResponse.StackTrace = exception.StackTrace;

&#x20;       return error.ErrorResponse;

&#x20;   }

&#x20;   return new ApiErrorResponse(exception);

}

```



คืน \*\*500 เสมอ\*\* ไม่ว่า `ErrorType` จะเป็น `Info` / `Werning` / `Error` และคืน `ApiErrorResponse` เปล่าๆ

ไม่ใช่ `MyDataAPI<T>` — รูปร่าง error จาก handler ส่วนกลางจึงต่างจากที่ `ResponseError` คืน



\---



\## 7. Naming conventions



\### 7.1 ตระกูลโมเดลสี่ตระกูล แยกกันด้วย suffix



| โฟลเดอร์ | Suffix | บทบาท | Namespace | ตัวอย่าง |

|---|---|---|---|---|

| `DataModel/` | ไม่มี | EF entity ตรงกับตาราง 1:1 แบ่งย่อย\*\*ตามฐานข้อมูล\*\* | `Innovation.Core.DataModel.<Db>` | `AmTotalAddStep`, `AfterMixingWeight`, `AmFeedingPath` |

| `DtoModel/` | `Dto` | สัญญาเข้า/ออกของ service แบ่งทั้งตาม db และฟีเจอร์ | `Innovation.Core.DtoModel\[.<X>]` | `GeneralDataSiloApproveDto`, `CurrencyDto`, `ImportItemParamsDto` |

| `DomainModel/` | `VM` | projection สำหรับ query | `Innovation.Core.DomainModel\[.<X>]` | `ApplicationUserVM`, `ComboBoxDataVM`, `ItemOutVM` |

| `ApiModel/` | prefix `Api` | envelope ของ HTTP | `Innovation.Core.ApiModel\[.<X>]` | `ApiData`, `ApiErrorResponse`, `ApiGetKB` |



\### 7.2 ชื่อชนิดอื่นๆ



```

I<Entity>Repository / <Entity>Repository (internal)   ← ต่อ entity

I<Db>Repository / <Db>Repository                      ← ต่อฐานข้อมูล (legacy, ใน OldRepositories/)

I<Db>UnitOfWork / <Db>UnitOfWork (internal)

<Db>RepositoryFactory (internal static) / DbContextFactory / UnitOfWorkFactory (public static)

<DbName>Context                                       ← ชื่อมี underscore ติดมาด้วย: Data\_Center\_MasterContext

I<Feature>Service → <Feature>Service หรือ Innovation<Feature>Service   ← ไม่มีกฎแน่นอนว่าเมื่อไรใส่ Innovation

<Db>DomainService / I<Db>DomainService                ← Innovation.Services.Domain.<Db>

<Feature>Controller                                   ← Innovation.API.Controllers เสมอ

```



Namespace รากคือ `Innovation.<Layer>\[.<SubLayer>]\[.<DbOrFeature>]`

มีจุดที่ assembly กับ namespace ไม่ตรงกัน: โฟลเดอร์ `Innovation.Repositories/` สร้าง namespace

`Innovation.Repository.\*` และโฟลเดอร์ `Core` ใน `Innovation.Core` ทำให้ได้ `Innovation.Core.Core.\*` ซ้อนกัน



\### 7.3 การแตกไฟล์ `partial` — เป็นเรื่องขนาดไฟล์ ไม่ใช่ขอบเขตการออกแบบ



controller ตัวหนึ่งถูกกระจายเป็นหลายไฟล์ โดย\*\*ไฟล์ฐานเก็บ attribute และ constructor ไว้ที่เดียว\*\*:



```csharp

// InvoiceExportERPController.cs — ไฟล์ฐาน

\[Route("api/\[controller]/\[action]")]

\[ApiController]

public partial class InvoiceExportERPController : ControllerBase

{

&#x20;   public InvoiceExportERPController(IInvoiceExportERPService a, IDBSdbService b, ICentralDBService c)

&#x20;   {

&#x20;       Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");   // ตั้ง culture ใน ctor

&#x20;   }

}



// InvoiceExportERPPartialController01.cs — ไม่มี attribute ไม่มี ctor มีแต่ action

public partial class InvoiceExportERPController

{

&#x20;   \[HttpGet] public ActionResult<IEnumerable<GeneralUnitVM>> searchGeneralUnits(...) { }

}

```



suffix ที่ใช้: ตัวเลข (`Partial`, `Partial01`, `Partial02`, `Partial03`) หรือ\*\*ชื่อนักพัฒนา\*\*

(`\_DevA`, `\_DevB`, `\_DevC` — ในโค้ดจริงเป็นชื่อคน) แบบหลังยืนยันว่าแรงจูงใจคือการเลี่ยง merge conflict

รูปแบบเดียวกันปรากฏในชั้น service (`InvoiceExportERPServicePartial\_DevA.cs`) และ repository ด้วย



\### 7.4 คำที่สะกดผิดและฝังอยู่ใน public API — ต้องคงไว้ถ้าสร้างใหม่ให้เหมือนเดิม



| ในโค้ด | ที่ถูกต้อง |

|---|---|

| `Transection` | Transaction |

| `Messenger` (property บน `MyDataAPI<T>`) | Message |

| `Werning` (enum member) | Warning |

| `Brife` | Brief |

| `Confrim` | Confirm |

| `Managment` / `Manangment` | Management |

| `Lacation` | Location |

| `Reposiory` | Repository |

| `Satatus` | Status |

| `PackageReposrt` (โฟลเดอร์ฝั่ง client) | PackageReport |



\---



\## 7b. รูปแบบข้ามชั้นที่สำคัญที่สุด



\### 7b.1 ช่องทางแจ้งความล้มเหลวสองช่องขนานกัน



\*\*ช่องที่หนึ่ง — exception สำหรับ error จริง\*\*

`throw new ErrorExceptionHandling(...)` ปรากฏ \*\*118 จุด\*\* ใน `Innovation.Services` เช่น



```csharp

throw new ErrorExceptionHandling("ไม่พบ On Hand ที่จะหยิบ !", ErrorExceptionHandlingEnum.Werning);

```



ข้อความเป็นภาษาไทยและถึงผู้ใช้โดยตรง ถูกจับที่ `UseExceptionHandler("/error")` → `ErrorsController` → HTTP 500



\*\*ช่องที่สอง — ธง boolean บน response DTO สำหรับ "หาไม่เจอตามคาด"\*\* คืนด้วย HTTP 200



`Innovation.Core/DtoModel/TotalWeightPlc/\*Dto` ประกาศธงชุดนี้:



```

IsEmptyRmBal          IsEmptySiloApprove     IsEmptyKbTogether      IsEmptyWeighting

IsEmptyForId          IsEmptyKbBarcode       IsLockedOrExpired      IsAlreadyInTotalWeight

IsNotEqualSiloId      IsNotEqualLineName     IsNotEqualKbBarcode    IsNotEqualOwnerSiteId

IsBalWtLessThanWeightH HasRmAutomation       IsManual               IsMixingCheck

IsKbTogether          IsInsert               IsOil                  IsNext\_Plan\_Manual

```



> \*\*ประเด็นสำคัญ\*\*: ฝั่ง desktop ประกาศธง\*\*ชุดเดียวกันเป๊ะ\*\*ใน `ViewModel/Response/\*VM`

> นี่จึงไม่ใช่นิสัยของฝั่ง client แต่เป็น \*\*contract ข้ามฝั่งที่ตั้งใจออกแบบ และดูแลด้วยมือทั้งสองข้าง\*\*

> ผลคือเวลาเพิ่มกรณีความล้มเหลวใหม่หนึ่งกรณี ต้องแก้ทั้ง Dto ทั้ง VM และทั้ง presenter ที่ไล่ `if`



\### 7b.2 `DataMappingHelper.GetSimpleDataMap` — ปัญหาประสิทธิภาพที่วัดได้



`Innovation.UtilityCore/Innovation.UtilityCore.Helper/DataMappingHelper.cs` เป็นตัวแปลง entity → Dto

ที่ใช้ทั่วทั้งชั้น service (AutoMapper ถูกอ้างใน csproj แต่ใช้ผ่าน helper ตัวนี้แทน)



```csharp

public static TDestination GetSimpleDataMap<TSource, TDestination>(TSource sourceData)

{

&#x20;   var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());

&#x20;   var mapper = new Mapper(config);

&#x20;   return mapper.Map<TDestination>(sourceData);

}

```



`MapperConfiguration` คอมไพล์ expression tree ตอนสร้าง จึงถูกออกแบบมาให้\*\*สร้างครั้งเดียวต่อ process\*\*

แต่ helper ตัวนี้สร้างใหม่\*\*ทุกครั้งที่เรียก\*\* และถูกเรียก \*\*1,716 ครั้ง\*\* ทั่ว `Innovation.Services`

รวมถึงเรียก\*\*ข้างใน projection ราย row\*\*:



```csharp

.Select(x => DataMappingHelper.GetSimpleDataMap<OnHand, OnHandDto>(x))

```



แปลว่า query ที่คืน 1,000 แถว จะสร้าง `MapperConfiguration` 1,000 ตัว

นี่คือข้อบกพร่องที่วัดได้ชัดที่สุดในโค้ดเบสนี้ และน่าจะเป็นคำตอบของคำถาม "ทำไมช้า"



\### 7b.3 Transaction ข้ามฐานข้อมูลโดยไม่มี distributed transaction



```csharp

// AutoUpdateService.cs

\_dbTransectionUnitOfWork.BeginTransaction();

\_dbMasterUnitOfWork.BeginTransaction();

// ... งาน ...

//    \_dbMasterUnitOfWork.CommitTransaction();     ← ถูกคอมเมนต์ทิ้งไว้

\_dbTransectionUnitOfWork.CommitTransaction();

// catch:

\_dbTransectionUnitOfWork.RollbackTransaction();

\_dbMasterUnitOfWork.RollbackTransaction();

```



สอง UnitOfWork = สอง `DbContext` = สองการเชื่อมต่อคนละฐานข้อมูล ไม่มีตัวประสาน transaction

ถ้าล้มเหลวคาบเกี่ยวระหว่างสอง commit ข้อมูลจะไม่สอดคล้องกัน และการที่ commit ตัวหนึ่งถูกคอมเมนต์ไว้

ยิ่งทำให้พฤติกรรมกำกวม ยังมีรูปแบบเดียวกันใน `InnovationSiloApproveService` ที่เปิด transaction

ต่อไซต์เก็บไว้ใน dictionary ตั้งแต่ตอน `SetUnitOfWorkBySite`



\### 7b.4 Raw SQL มีน้อยมาก



ทั้ง backend มี `GetDynamicBySql` เพียง \*\*12 จุด\*\* และ `FromSqlRaw`/`ExecuteSqlRaw` อีก \*\*2 จุด\*\*



> \*\*`spRMBAL\_WITHDRAW` ไม่ใช่การเรียก stored procedure\*\* — เป็นเพียงชื่อเมธอดบน

> `ITotalWeightPlcService` และชื่อ action ที่สืบชื่อ stored procedure เดิมสมัย Delphi มา

> ตัว implementation เป็น C#/EF ปกติ \*\*จึงไม่มี dependency กับ stored procedure ที่จะขวางการย้ายไป SQLite\*\*



\---



\## 7c. คู่ฝั่งเซิร์ฟเวอร์ของ TotalWeight\_PLC



`Innovation.API/Controllers/TotalWeightPlcController.cs` — \*\*358 บรรทัด 52 action\*\* คือฝั่งเซิร์ฟเวอร์

ของแอปชั่งน้ำหนักหน้าโรงงาน จับคู่กับ `ITotalWeightPlcService` (`Core/Service/`) และ

`DtoModel/TotalWeightPlc/`



controller \*\*จัดกลุ่มตามฟอร์มที่เรียก\*\* ด้วยคอมเมนต์คั่น เหมือนกับที่ฝั่ง desktop ทำ:



```csharp

// frmSelectKB \_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_

\[HttpPost]

public ActionResult<IEnumerable<KanbanDatasDto>> GetKanban(\[FromBody] KanbanRequestDto obj)

&#x20;   => \_iTotalWeightPlcService.GetKanban(obj);

```



การจับคู่ชื่อ Dto ↔ VM ตรงไปตรงมา: `TotalWeightRequestDto` ↔ `TotalWeightRequestVM`,

`KanbanDatasDto` ↔ `KanbanDatasVM`, `InputBarcodeResponseDto` ↔ `InputBarcodeResponseVM`



\---



\## 8. ฐานข้อมูลและ ER



\### 8.1 ภาพรวม 16 ฐานข้อมูล



| ฐานข้อมูล | บทบาท | จำนวน entity | connection key |

|---|---|---:|---|

| `DBMaster` | ข้อมูลหลัก (ลูกค้า ผู้ขาย สินค้า สายผลิต) | 408 | `DBMasterDB` |

| `DBTransection` | ธุรกรรมการผลิต | 264 | `DBTransectionDB` |

| `Material\_Management` | จัดการวัตถุดิบ | — | `MaterialManagementDB` |

| `ProductionSTD` | มาตรฐานการผลิต | — | `ProductionStdDB` |

| `DBMasterHist` | ประวัติของ `DBMaster` | — | `DBMasterHistDB` |

| `DBTransectionHist` | ประวัติของ `DBTransection` | — | `DBTransectionHistDB` |

| `DBCenter` | ตั้งค่ากลาง + `ServiceDataMst` (คุม background service) | — | `DBCenterDB` |

| `CentralDB` | ข้อมูลกลางข้ามไซต์ | 5 | `CentralDB` |

| `Data\_Center\_Master` | ทะเบียนไซต์/ผู้ใช้/สิทธิ์ | — | `DataCenterMasterDB` |

| `Data Center` | ข้อมูลศูนย์กลาง (ชื่อฐานมีเว้นวรรค) | — | `DataCenterDB` |

| `SILO` | \*\*ไซโล คัมบัง การชั่ง\*\* — หัวใจของ demo slice | 13 | `SILODB` |

| `DBSdb` | ข้อมูลเสริม | — | `DBSdbDB` |

| `DB\_Auto\_Report` | รายงานอัตโนมัติ | — | `DBAutoReportDB` |

| `QCDB` | ควบคุมคุณภาพ | — | \*(ถูกกันออกจาก build)\* |

| `Packing` | บรรจุภัณฑ์ | — | \*(ถูกกันออกจาก build)\* |

| `MasterPOPRPan` | PO/PR | — | \*(ถูกกันออกจาก build)\* |



สามตัวสุดท้ายมี `DbContext` อยู่ใน `Innovation.Repositories/DBContext/` แต่โมเดลถูกกันออกจากการคอมไพล์

ด้วย `<Compile Remove="...">` ใน `Innovation.Core.csproj`



ขนาด `DbContext` ที่ scaffold มา: `DBTransectionContext` 26,462 บรรทัด, `DBMasterContext` 25,334,

`Material\_ManagementContext` 24,977, `ProductionSTDContext` 21,144 — \*\*รวม 163,795 บรรทัดใน 16 ไฟล์\*\*



\### 8.2 entity ทั้งหมดของฐาน `SILO` (หัวใจของ demo slice)



| Entity | หน้าที่ |

|---|---|

| `KbTogether` | คัมบังที่จับกลุ่มชั่งพร้อมกัน |

| `Weighting` | บันทึกการชั่งราย step |

| `TotalWeight` | ผลรวมน้ำหนักต่อคัมบัง |

| `TwAcceptWeightHis` | ประวัติการกด Accept |

| `SendStepParameter` | \*\*แผนที่ address ของ PLC\*\* ต่อ step |

| `TrayPlan` / `TrayWeight` / `TrayBarcode` / `TypeTray` | ระบบถาด |

| `Station` | สถานีงาน |

| `UsrWt` | \*\*ผู้ใช้ฝั่งชั่งน้ำหนัก\*\* (ดู §9 — เก็บรหัสผ่านเป็น plaintext) |

| `RmConfirmMst` | การยืนยันวัตถุดิบ |

| `KbPlcSort` | ลำดับคัมบังที่ส่งให้ PLC |



\### 8.3 ตารางจากฐานอื่นที่ demo slice แตะ



`RM\_BAL` (ยอดคงเหลือวัตถุดิบ), `SiloApprove` (อนุมัติไซโล), `OnHand` (สต๊อกคงเหลือ),

`PRODSTD\_MIXTEMP` (รูปแบบการผสม), `Application\_Setting` (ค่าตั้งต้นรายสายผลิต เช่น

`Setting\_ID 4/5` = ค่าเผื่อ min/max, `23` = EnabledWeightInput, `24` = CheckMixingFinished)



\### 8.4 ER ของ demo slice



```mermaid

erDiagram

&#x20;   UsrWt ||--o{ Weighting : "ชั่งโดย"

&#x20;   Station ||--o{ Weighting : "ที่สถานี"

&#x20;   KbTogether ||--o{ Weighting : "ตามคัมบัง"

&#x20;   KbTogether ||--|| TotalWeight : "สรุปเป็น"

&#x20;   TotalWeight ||--o{ TwAcceptWeightHis : "ประวัติ accept"

&#x20;   SendStepParameter }o--|| Station : "address PLC ของ"

&#x20;   Weighting }o--|| RM\_BAL : "หักยอดจาก"

&#x20;   SiloApprove }o--|| RM\_BAL : "อ้างบาร์โค้ด"

&#x20;   KbTogether }o--|| PRODSTD\_MIXTEMP : "รูปแบบผสม"

&#x20;   Application\_Setting }o--|| Station : "ตั้งค่าให้"

```



> ความเสี่ยงที่ต้องบันทึก: service ตัวเดียวถือ UnitOfWork 3–4 ตัวข้ามฐานข้อมูลพร้อมกัน

> โดยไม่มี distributed transaction (ดู §7b.3)



\---



\## 9. การยืนยันตัวตนและสิทธิ์



\### 9.1 สองเส้นทางที่ไม่เกี่ยวกัน และไม่มีเส้นทางไหนใช้ ASP.NET Core authentication



\*\*เส้นทางที่ 1 — ผู้ใช้สำนักงาน ผ่าน Active Directory\*\*

แอปเดสก์ท็อปตรวจรหัสผ่านเองที่เครื่อง ไม่ได้ส่งรหัสผ่านมาที่ API:



```csharp

using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "company.local"))

{

&#x20;   isValid = pc.ValidateCredentials(username, password);

}

```



แล้วจึงเรียก API เพื่อขอ\*\*สิทธิ์\*\*อย่างเดียว (`GetUserPermission` → `ApplicationUserPermissionVM`)



\*\*เส้นทางที่ 2 — ผู้ใช้หน้าโรงงาน (ชั่งน้ำหนัก)\*\*



```csharp

// SILOService.cs

public UsrWt CheckPasswordWeighing(string user, string pass, int siteID, string programID, string prgname)

&#x20;   => \_siloRepository.CheckPasswordWeighing(user, pass, programID, prgname);



// SILORepository.cs

public UsrWt CheckPasswordWeighing(string user, string pass, string programID, string prgname)

{

&#x20;   return (from x in siloContext.UsrWt

&#x20;           where x.LoginName == user

&#x20;           \&\& x.Password == pass          // ← เทียบ plaintext ข้างใน LINQ query

&#x20;           \&\& x.PrgId == programID

&#x20;           \&\& x.PrgName == prgname

&#x20;           select x).FirstOrDefault();

}

```



\*\*รหัสผ่านเก็บและเทียบเป็น plaintext ทั้งหมด ไม่มีการ hash ที่ใดเลยบนเส้นทางนี้\*\*

ทั้งที่โปรเจกต์ `Innovation.Authentication` มี `Infrastructure/AESThenHMAC.cs`

(AES-then-HMAC-SHA256) อยู่แล้ว แต่ไม่ถูกใช้กับเส้นทางรหัสผ่าน



\### 9.2 โมเดลสิทธิ์



สิทธิ์ผูกกับสามสิ่ง: \*\*ผู้ใช้ × โปรแกรม × ไซต์\*\*



\- `ProgramCode` ระบุแอป (รูปแบบ `ERP0xx` — ตัวอย่าง `ERP005`, `ERP024`, `ERP026`)

\- `UserOperationSite` ระบุไซต์ที่ผู้ใช้ทำงานได้

\- `ApplicationUserPermissionVM` ถือรายการสิทธิ์



แอปเดสก์ท็อปใช้การเรียกเดียวกันนี้ตรวจ\*\*เวอร์ชันโปรแกรม\*\*ด้วย (`CheckApplicationVersion()`)

ดังนั้น `ProgramCode` ที่ไม่รู้จักจะบล็อกการล็อกอินทั้งหมด



\### 9.3 ประตูที่ฝังรหัสไว้ในโค้ดฝั่ง client



\- `admin` — สำหรับรีเซ็ตพารามิเตอร์ PLC และรีเซ็ตรหัสผ่าน

\- `123` — สำหรับตั้งค่า MAX/MIN และล็อกอินสายผลิต



เคยมีบั๊กที่ประตูตรวจกลับด้าน (`frmMonitorAddressPLC` — ใส่รหัส\*\*ผิด\*\*แล้วรีเซ็ตได้)

บันทึกไว้ใน `RUNTIME\_TEST\_CHECKLIST.md` §E



\### 9.4 โปรเจกต์ `Innovation.Authentication`



มีแค่ 4 ไฟล์: `Service/AuthenticationService.cs`, `Service/AuthorizationService.cs`,

`Infrastructure/AESThenHMAC.cs`, `Program.cs`



\---



\## 10. การเชื่อมต่อ D365 / ERP



`Innovation.Class.LibraryCore/Innovation.Class.D365InterfaceService/` (`Implement/`, `Interface/`, `Model/`)



\- \*\*รูปแบบ endpoint OData\*\*: `/api/services/IGT\_ProfileServiceGroup/IGT\_<Entity>Service/<Action>`

&#x20; เช่น `IGT\_CustomerService`, `IGT\_VendorService/GetMainData`,

&#x20; `IGT\_VendorService/GetRegistrationData`, `IGT\_InventoryService/GetItemData`

\- \*\*การยืนยันตัวตนแบบ ADFS/OAuth\*\* ตั้งค่าอยู่ในไฟล์ config ของแต่ละแอปเดสก์ท็อป:

&#x20; `ClientId`, `BaseURL`, `AuthenURL`, `apiURL`, `CustomerERP` (ค่าจริงถูกแทนด้วย placeholder)

\- \*\*ผู้ใช้งาน\*\*: `Innovation.MasterData` ดึงข้อมูลหลักเข้ามา, `InvoiceExportERP` ส่งใบแจ้งหนี้ออกไป

\- ฝั่ง API มี `D365SpcInterfaceService` / `ID365SpcInterfaceService`

\- DTO ตระกูล `InhouseToD365ExportedResult` ใช้รับผลการส่งออก

\- `Innovation.MasterData/Const/OdataURL.cs` สลับ endpoint ระหว่าง production กับ test

&#x20; ด้วย helper `IsProduction()`



\---



\## 11. `Innovation.Library` — ระบบรุ่นก่อน



871 ไฟล์ `.cs` กระจายใน \*\*สแนปช็อต branch สองชุดที่เกือบเหมือนกัน\*\*: `Dev/` (436 ไฟล์)

และ `Main/` (435 ไฟล์) ชุดละ 19 โปรเจกต์ — เป็นการเช็ค branch เข้ามาไว้ในโครงไฟล์แทนที่จะอยู่ใน version control



\### เนื้อหา



\*\*`Innovation.DataCenter/`\*\* — สถาปัตยกรรมสามชั้นแบบคลาสสิก `DATA` / `DAO` / `BLL`



```

Innovation.DATA.Data\_Center\_Master   Innovation.DATA.Data\_Detail

Innovation.DAO.Data\_Center\_Master    Innovation.DAO.Data\_Detail

Innovation.BLL.Data\_Center\_Master    Innovation.BLL.Data\_Detail

Innovation.Data.CentralDB            Innovation.Data.MaterialManagement

Innovation.Data.ProductionSTD        Innovation.DataCenter        Innovation.ICTDB

```



\*\*นี่คือบรรพบุรุษของชั้น Repository/UnitOfWork ในปัจจุบัน\*\* — เส้นทางวิวัฒนาการคือ

`DATA/DAO/BLL` → `Repository/UnitOfWork/DI`



\*\*`Innovation.Control/`\*\* — `InnoControlLibrary` (control WinForms ที่เขียนเอง) + แอปตัวอย่าง

นี่คือ DLL ที่แอปเดสก์ท็อปทุกตัวอ้างผ่าน \*\*absolute `HintPath` `D:\\Library\\InnoControlLibrary.dll`\*\*

ซึ่งเป็นอุปสรรคต่อการ reproduce build บนเครื่องอื่น



\*\*`Innovation.Utility/`\*\* — `AuthenticationCenter`, `Translation`, `Winform`, `Report\_Utility`

พร้อม test harness สองตัว (`TestUsingAuthenticationCenter`, `TestUsingNormalAuthentication`)



\### ร่องรอยอื่นของ version control รุ่นเก่า



ทุก `.csproj` ในโซลูชันมี binding ของ SourceSafe/TFS ค้างอยู่:



```xml

<SccProjectName>SAK</SccProjectName>

<SccProvider>SAK</SccProvider>

<SccAuxPath>SAK</SccAuxPath>

<SccLocalPath>SAK</SccLocalPath>

```



และโครงไฟล์ที่ส่งมาให้วิเคราะห์นี้\*\*ไม่ได้เป็น git repository\*\*



\---



\## 12. ASP.NET Core pipeline — ข้อควรรู้



\- \*\*route เป็นแบบ RPC ไม่ใช่ REST\*\*: `\[Route("api/\[controller]/\[action]")]` ทำให้ชื่อ action

&#x20; กลายเป็นส่วนหนึ่งของ URL (`api/SiloApprove/GetGeneralDataSiloApprove`) และ verb attribute

&#x20; เป็นแบบเปล่า (`\[HttpGet]`, `\[HttpPost]`) ไม่มี template

\- return type ปกติคือ `ActionResult<TDto>` หรือ `ActionResult<List<TDto>>` บาง action คืน `string` หรือ `void`

\- \*\*ไม่มี `\[Authorize]` ที่ไหนเลย\*\* และ `app.UseAuthorization()` ถูกเรียกทั้งที่ไม่มีการลงทะเบียน

&#x20; authentication scheme ใดๆ — pipeline จึงไม่ได้ authorize อะไรจริง

\- CORS เปิดกว้างสุด (`AllowAnyOrigin` + `AllowAnyMethod` + `AllowAnyHeader`)

\- `CustomLocalDateTimeConverter` แปลงวันเวลาขาออกเป็น UTC รูปแบบ `yyyy-MM-ddTHH:mm:ssZ`

&#x20; และขาเข้าใช้ `DateTime.Parse(reader.GetString())` ตรงๆ

\- controller หลายตัวตั้ง `Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US")` ใน constructor



\---



\## 13. สิ่งที่ไม่ควรทำตาม



ทั้งหมดนี้มีอยู่จริงในโค้ด บันทึกไว้เพื่อ\*\*ไม่ให้ทำซ้ำ\*\* ตอนสร้างใหม่



| ปัญหา | รายละเอียด |

|---|---|

| `catch (Exception ex) { throw ex; }` | ทำลาย stack trace เดิม ควรใช้ `throw;` |

| `StatusCode(500, ex)` | ส่ง exception object ดิบกลับไปให้ client |

| `ErrorsController` คืน 500 เสมอ | ไม่ได้แมป `Info`/`Werning`/`Error` เป็น status code ที่ต่างกัน |

| รหัสผ่าน DB อยู่ใน `appsettings.json` | รวม 22 ไฟล์ config ทั้งระบบ |

| รหัสผ่าน plaintext ในตาราง `UsrWt` | เทียบข้างใน LINQ query (§9.1) |

| `AllowAnyOrigin()` | บน API ที่ให้บริการข้อมูลการผลิตจริง |

| `UseAuthorization()` โดยไม่มี authentication | pipeline ไม่ได้ป้องกันอะไรจริง |

| UNC path ฝังตาย | `\\\\FILE-SERVER\\APILogs` โดยบรรทัดที่อ่านจาก config ถูกคอมเมนต์ไว้ข้างบน |

| `RequestBodyLoggingMiddleware` ผิดลำดับ | ลงทะเบียนหลัง `UseExceptionHandler` |

| `MapperConfiguration` สร้างใหม่ทุกครั้ง | 1,716 จุด รวมถึงข้างใน projection ราย row (§7b.2) |

| Transaction ข้ามฐานโดยไม่มีตัวประสาน | มี commit หนึ่งตัวถูกคอมเมนต์ทิ้ง (§7b.3) |

| service `new` UnitOfWork จาก static factory | ทำให้ทดสอบไม่ได้ทั้งชั้น (§5) |

| `Startup.cs` ที่ตายแล้ว | ยังอยู่ในโปรเจกต์ สร้างความสับสน |

| registration ที่ไม่มีใครใช้ | `IGenericRepository<>` |

| background service ไม่มี namespace | ประกาศที่ global scope |



\---



\## 14. Recreate prompt



> ใช้บล็อกนี้เป็น prompt ตั้งต้นเพื่อสร้างโครงระบบนี้ขึ้นใหม่ให้เหมือนเดิม



```

สร้างโซลูชัน .NET ชื่อ BackEnd.sln สำหรับระบบ MES โรงงาน ตามข้อกำหนดนี้



ข้อจำกัด

\- ไลบรารีเป็น netstandard2.0, Web API เป็น net6.0, ปิด nullable reference types ทุกโปรเจกต์

\- ฐานข้อมูล SQL Server เท่านั้น ผ่าน EF Core (ไลบรารี 3.1.x, API 6.0.x)

\- แพ็กเกจบังคับ: EntityFrameworkCore.SqlServer, EFCore.BulkExtensions, AutoMapper, Mapster,

&#x20; DinkToPdf, AspNetCore.Reporting, QRCoder, ZXing.Net, log4net, Swashbuckle



โครงสร้างโปรเจกต์ (ดูทรีเต็มในเอกสารข้อ 2)

&#x20; Innovation.Core            — DataModel/(ต่อฐานข้อมูล) DtoModel/ DomainModel/ ApiModel/ Core/(Repository,Service,UnitOfWork)

&#x20; Innovation.Repositories    — DBContext/ GenericRepository/(ต่อฐานข้อมูล) UnitOfWork/ RepositoryFactory/ OldRepositories/

&#x20; Innovation.Services        — Service/ Domain/ BackgroundService/ Innovation.Authentication/

&#x20; Innovation.API             — Controllers/ Middleware/ Report/ Service/

&#x20; Innovation.UtilityCore, Innovation.Class.LibraryCore, Innovation.ReportServices,

&#x20; Innovation.PrinterService.API, Innovation.WebApp, Innovation.Library



สัญญา (คัดลอก signature จากข้อ 3 มาทั้งหมด)

&#x20; IRepository<T> where T : class, new()   — 18 สมาชิก

&#x20; IUnitOfWork / IContextUnitOfWork

&#x20; I<Db>UnitOfWork หนึ่งตัวต่อหนึ่งฐานข้อมูล รวม 13 ตัว แต่ละตัวเปิด property repository ต่อ entity

&#x20; I<Entity>Repository : IRepository<TEntity> { }  — marker เปล่า

&#x20; ไม่มี IBaseService — service เป็น interface ต่อโมดูล 68 ตัว ทุกเมธอดรับ int siteId เป็นพารามิเตอร์แรก



รูปแบบบังคับ

&#x20; RepositoryImpl<T> ตัวเดียวรับ DbContext ฐาน อ่านด้วย AsNoTracking() ไม่เรียก SaveChanges

&#x20; <Db>UnitOfWork สร้าง repository แบบ lazy ผ่าน <Db>RepositoryFactory

&#x20; UnitOfWorkFactory เป็น public static มี overload สามแบบต่อฐานข้อมูล: () / (int siteId) / (string connStr)

&#x20; DbContext ทุกตัวมีสาม constructor + private static GetOptions(string) + UseSqlServer ไม่มี OnConfiguring

&#x20; service ทุกตัวเปิดเมธอดด้วย SetUnitOfWorkBySite(siteId)

&#x20; BaseAPIController มี helper \[NonAction] และ GetSiteAPI() ที่อ่าน HTTP header ชื่อ SiteID

&#x20; background service สืบจาก InnovationBaseBackgroundService<T> อ่านรอบเวลาจากตาราง ServiceDataMst



การตั้งชื่อ

&#x20; DataModel ไม่มี suffix / DtoModel ลงท้าย Dto / DomainModel ลงท้าย VM / ApiModel ขึ้นต้น Api

&#x20; <Feature>Controller, I<Feature>Service, <Db>DomainService, <DbName>Context

&#x20; controller ใหญ่แตกเป็น partial หลายไฟล์ โดย attribute และ ctor อยู่ในไฟล์ฐานไฟล์เดียว

&#x20; คงคำสะกดผิดเดิมไว้: Transection, Messenger, Werning, Brife, Confrim, Managment, Lacation, Reposiory, Satatus



Composition root

&#x20; Program.cs แบบ minimal hosting ลงทะเบียน service \~65 ตัวเป็น AddScoped เรียงแบน

&#x20; ลำดับ pipeline: UseExceptionHandler("/error") → UseStaticFiles → UseRouting → UseAuthorization

&#x20;                 → UseMiddleware<RequestBodyLoggingMiddleware> → UseCors → MapControllers



ผลลัพธ์ที่ต้องการ: ASCII tree ของโซลูชัน ตามด้วยเนื้อไฟล์แต่ละไฟล์เป็น code block

```



\---



\## เอกสารที่เกี่ยวข้อง



\- \[../README.md](../README.md) — ภาพรวม โดเมน บทเรียน และแผนสร้างใหม่ให้รันได้จริง

\- \[../Frontend (The Client-Side)/ROADMAP.md](../Frontend%20\\(The%20Client-Side\\)/ROADMAP.md) — พิมพ์เขียวฝั่งเดสก์ท็อป







