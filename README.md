\# Innovation MES — พิมพ์เขียวระบบ และแผนสร้างใหม่ให้รันได้จริง



> \*\*หมายเหตุสำคัญก่อนอ่าน\*\*

>

> เอกสารชุดนี้แบ่งเป็นสองส่วนที่ต้องไม่สับสนกัน

>

> - \*\*§1–§6 บรรยายระบบจริงตามที่เป็นอยู่\*\* รวมข้อบกพร่อง คำที่สะกดผิด และการตัดสินใจที่ควรปรับปรุง

>   เขียนไว้แบบไม่แต่งเติม เพราะเป้าหมายคืออ่านแล้วเข้าใจของจริงได้

> - \*\*§7–§8 คือแผนสร้างใหม่ให้สะอาด\*\* ซึ่งแก้ปัญหาเหล่านั้น

>

> ค่าที่เป็นความลับของบริษัท (รหัสผ่าน ชื่อเซิร์ฟเวอร์ IP ภายใน โดเมน AD ชื่อลูกค้า ชื่อนักพัฒนา)

> \*\*ถูกแทนด้วย placeholder ทั้งหมด\*\* โครงสร้างและรูปแบบยังคงอยู่ครบ แต่ค่าจริงไม่มีในเอกสาร



\---



\## 1. ระบบนี้คืออะไร



ระบบ \*\*MES (Manufacturing Execution System)\*\* ของโรงงานผลิตยาง/คอมพาวนด์ ครอบคลุมตั้งแต่

ชั่งวัตถุดิบ ผสม จัดการไซโล วางแผนผลิตด้วยระบบคัมบัง บรรจุ ควบคุมคุณภาพ ไปจนถึงส่งใบแจ้งหนี้เข้า D365/ERP



```

&#x20;                      6 ไซต์โรงงาน (PI1 PI2 PI3 PI4 BKK CPL)

&#x20;                                     |

&#x20;  +----------------------------------+----------------------------------+

&#x20;  |                                  |                                  |

แอปเดสก์ท็อป WinForms 10 ตัว      ASP.NET Core Web API            SQL Server 16 ฐาน

(เครื่องหน้าโรงงาน)          <---->  (53 controller)         <---->  (routing ตามไซต์)

&#x20;  |                                  |

&#x20;  +-- PLC (Mitsubishi MX Component)  +-- D365 / ERP (OData + ADFS)

&#x20;  +-- ตาชั่ง (RS-232)                 +-- บริการพิมพ์ป้าย

&#x20;  +-- เครื่องอ่านบาร์โค้ด

```



| ส่วน | เอกสาร |

|---|---|

| ฝั่งเซิร์ฟเวอร์ | \[Backend (The Server-Side)/ROADMAP.md](Backend%20\\(The%20Server-Side\\)/ROADMAP.md) |

| ฝั่งเดสก์ท็อป | \[Frontend (The Client-Side)/ROADMAP.md](Frontend%20\\(The%20Client-Side\\)/ROADMAP.md) |



\---



\## 2. ขนาดและความท้าทาย



\### ตัวเลขที่นับจริงจากซอร์ส



| รายการ | จำนวน |

|---|---:|

| ไฟล์ `.cs` รวม (ไม่รวม `bin`/`obj`) | \~12,400 |

| โฟลเดอร์ | 916 |

| ฐานข้อมูล SQL Server | 16 |

| บรรทัดของ EF `DbContext` ที่ scaffold มา | 163,795 |

| Entity ในฐานเดียว (`DBMaster`) | 408 |

| Controller ฝั่ง API | 53 |

| Service ที่ลงทะเบียนใน DI | \~65 |

| Interface ของ service | 68 |

| แอปเดสก์ท็อป | 10 |

| ฟอร์ม WinForms | 165 |

| เทมเพลตรายงาน `XtraReport` | 21 |

| Endpoint ที่แอปชั่งน้ำหนักเรียก | 57 |



\### โจทย์ที่ยากจริงในระบบนี้



1\. \*\*การ route ฐานข้อมูลตามไซต์ตอนรันไทม์\*\* — service ตัวเดียวต้องเลือกเซิร์ฟเวอร์ปลายทาง

&#x20;  จาก `siteId` ที่ส่งมาในทุกเมธอด และบางครั้งถือ connection ข้ามไซต์พร้อมกันหลายตัว

2\. \*\*การย้ายจาก Delphi มา C# โดยพฤติกรรมต้องเหมือนเดิมทุกตัวอักษร\*\* — ผู้ปฏิบัติงานถูกฝึกมากับ

&#x20;  ข้อความเดิม การเปลี่ยนคำเดียวคือการเปลี่ยนพฤติกรรมระบบ

&#x20;  (`Frontend/Innovation.TotalWeight\_PLC/Prompt/REFACTOR\_GUIDE.md` ระบุข้อห้ามนี้ไว้ชัดเจน)

3\. \*\*การคุยกับฮาร์ดแวร์แบบเรียลไทม์หน้าโรงงาน\*\* — PLC ผ่าน COM, ตาชั่งผ่านพอร์ตอนุกรม,

&#x20;  เครื่องอ่านบาร์โค้ดแบบ keyboard wedge ทั้งหมดต้องทำงานร่วมกันในฟอร์มเดียว

4\. \*\*การอัปเดตแอปข้าม 6 ไซต์ที่เครือข่ายไม่เชื่อมกัน\*\* — แต่ละไซต์มีเซิร์ฟเวอร์อัปเดตของตัวเอง

5\. \*\*ระบบที่สืบทอดมาสองรุ่น\*\* — โค้ดรุ่นก่อน (`DATA`/`DAO`/`BLL`) ยังอยู่ในโครงเดียวกันกับ

&#x20;  รุ่นปัจจุบัน (`Repository`/`UnitOfWork`/`DI`)



\---



\## 3. อภิธานศัพท์เฉพาะทาง



โค้ดนี้อ่านไม่รู้เรื่องถ้าไม่รู้ศัพท์เหล่านี้ก่อน



| คำ | ในโค้ด | ความหมาย |

|---|---|---|

| \*\*คัมบัง (kanban)\*\* | `KbBarcode`, `KbTogether`, `frmSelectKB` | ใบสั่งงานผลิตหนึ่งใบ มีบาร์โค้ดกำกับ เป็นหน่วยตั้งต้นของทุกอย่าง |

| \*\*คัมบังจับกลุ่ม\*\* | `KbTogether` | คัมบังหลายใบที่ชั่งไปพร้อมกันได้ |

| \*\*แผน (plan)\*\* | `PlanId`, `GetPlanDataByBarcode` | แผนผลิตที่ผูกกับคัมบัง บอกว่าต้องใช้วัตถุดิบอะไรบ้าง |

| \*\*แบท (batch)\*\* | `GetBatchDataByBarcode`, `GetBatchQtyByPlanId` | รอบการผลิตย่อยภายใต้แผน |

| \*\*สูตร (formulation)\*\* | `For\_code`, `ForId`, `GetForIdByPlanId` | สูตรผสมของสินค้า |

| \*\*ขั้นตอนชั่ง (step)\*\* | `SendStepParameter`, `MaxStep` | ลำดับการชั่งวัตถุดิบทีละชนิดในหนึ่งคัมบัง |

| \*\*Total Weight\*\* | `TotalWeight`, `frmTotalWeight` | ผลรวมน้ำหนักทุกขั้นของคัมบังหนึ่งใบ และชื่อหน้าจอปฏิบัติงานหลัก |

| \*\*`RM\_BAL`\*\* | `GetRmBal`, `ExecuteRmBalWithdraw` | ยอดคงเหลือวัตถุดิบ (Raw Material Balance) หักเมื่อชั่งเสร็จ |

| \*\*Silo Approve\*\* | `SiloApprove`, `frmShowAutoFeed` | การอนุมัติให้ดึงวัตถุดิบจากไซโลอัตโนมัติ |

| \*\*Auto-feed\*\* | `frmTotalWeight.AutoFeed.cs` | โหมดที่ PLC ป้อนวัตถุดิบเอง แทนคนตัก |

| \*\*รูปแบบผสม\*\* | `PRODSTD\_MIXTEMP`, `ChkMixingPattern` | ลำดับและอุณหภูมิการผสมตามมาตรฐานการผลิต |

| \*\*ประตูป้อน / ประตูปล่อย\*\* | `GetDropDoorSteps`, Feeddoor Step | ประตูกลของเครื่องผสม สั่งผ่าน address PLC ID70–74 |

| \*\*`SEND\_STEP\_PARAMETER`\*\* | ตารางในฐาน `SILO` | แผนที่ address ของ PLC ต่อขั้นตอน ตั้งค่าได้จากฐานข้อมูล |

| \*\*`WP\_CODE`\*\* | ใน checklist | รหัสตำแหน่งงาน ใช้อ้างเวลาเขียนน้ำหนักกลับ |

| \*\*BOM Replace\*\* | `Innovation.BomReplace` | การสับเปลี่ยนวัตถุดิบทดแทนในสูตร |

| \*\*TTR\*\* | `TestTrialRequest`, `TtrMst` | คำขอทดลองผลิต (Test Trial Request) |

| \*\*On-hand FIFO\*\* | `ServiceTransectionOnHandFIFOBackgroundService` | การคำนวณสต๊อกคงเหลือแบบเข้าก่อนออกก่อน ทำเป็นงานเบื้องหลัง |

| \*\*ไซต์ (site)\*\* | `siteId`, `SITE\_ID\_FOR\_AUTO\_UPDATE` | โรงงานสาขา — PI1 PI2 PI3 PI4 BKK CPL |

| \*\*ไลน์ (line)\*\* | `LineId`, `frmSelectLine` | สายการผลิตภายในไซต์ |

| \*\*สถานี (station)\*\* | `StationId`, `Station` | จุดทำงานบนไลน์ หนึ่งเครื่องคอมพิวเตอร์ต่อหนึ่งสถานี |

| \*\*ถาด (tray)\*\* | `TrayPlan`, `TrayWeight`, `TrayBarcode` | ภาชนะรองวัตถุดิบที่ชั่งแล้ว มีบาร์โค้ดของตัวเอง |

| \*\*`UsrWt`\*\* | ตารางในฐาน `SILO` | ผู้ใช้ฝั่งชั่งน้ำหนัก แยกจากผู้ใช้ AD ของสำนักงาน |



\---



\## 4. กระบวนการทำงานหลัก



\### 4.1 การชั่งวัตถุดิบ — เส้นทางหลักของระบบ



```

ผู้ปฏิบัติงานยิงบาร์โค้ดคัมบัง

&#x20; └─ CheckBarcode / GetPlanDataByBarcode      หาแผนและสูตร

&#x20;      └─ GetMaxStepByBarcode                 มีกี่ขั้นตอน

&#x20;           └─ วนทีละขั้น:

&#x20;                ├─ แสดงเป้าหมาย + ช่วง \[ต่ำสุด, สูงสุด]

&#x20;                │    ช่วงปกติ = เป้าหมาย ± Application\_Setting 4/5

&#x20;                │    ยกเว้นขั้น 2 (เมื่อ Number != 1) และขั้น 3 (เมื่อ Number == 1)

&#x20;                │    ที่ใช้ ±0.02 คงที่ ตามพฤติกรรม Delphi เดิม

&#x20;                ├─ อ่านน้ำหนักจากตาชั่งผ่านพอร์ตอนุกรมแบบเรียลไทม์

&#x20;                └─ กดส่งน้ำหนัก → เขียนลง PLC

&#x20;           └─ Accept  (ต้องส่งน้ำหนักขั้นปัจจุบันก่อน มิฉะนั้นถูกบล็อก)

&#x20;                └─ InsertTwAcceptWeightHis   บันทึกประวัติ

&#x20;           └─ Save

&#x20;                ├─ InsertTotalWeight         สรุปน้ำหนักรวม

&#x20;                └─ ExecuteRmBalWithdraw      หักยอด RM\_BAL

```



\### 4.2 การป้อนวัตถุดิบอัตโนมัติ (auto-feed)



```

โหมด auto ตรวจว่าเป็นน้ำมัน (รหัสขึ้นต้น O) หรือคาร์บอน (ขึ้นต้น B)

&#x20; └─ อ่านรายการ SiloApprove ของคัมบังนั้น

&#x20;      ├─ ถ้าว่าง → เตือนแล้วปิดฟอร์ม input barcode (ไม่ค้างเงียบ)

&#x20;      └─ ต่อบาร์โค้ดแต่ละตัวเข้ากับ RM\_BAL

&#x20;           ├─ ไม่พบใน RM\_BAL → เตือน หยุดลูป แต่ฟอร์มหลักต้องยังเปิดอยู่

&#x20;           └─ พบ → SendParamOpenDropdoor (address ID70-74)

&#x20;                └─ PLC ป้อนวัตถุดิบ

&#x20;                     └─ อ่านน้ำหนักกลับ รอให้นิ่ง 1 วินาที

&#x20;                          └─ อัปเดตสถานะเป็นเขียว 'S' แล้วปิดฟอร์ม

```



> น้ำหนักที่ส่งให้ PLC ต้อง \*\*ตัดทศนิยม (truncate) ไม่ใช่ปัดเศษ\*\* — คูณ 100 แล้ว floor

> เพื่อให้ตรงกับ `Trunc` ของ Delphi เดิม



\### 4.3 การซิงก์ข้อมูลหลักกับ D365



```

Innovation.MasterData  --OData-->  IGT\_ProfileServiceGroup/IGT\_<Entity>Service

&#x20;                                    (Customer, Vendor, Inventory/Item)

&#x20;                      <--ADFS/OAuth--  ClientId + AuthenURL

```



\### 4.4 การคำนวณสต๊อก FIFO เบื้องหลัง



งานเบื้องหลังที่รอบเวลาและสถานะเปิด/ปิดอ่านจากตาราง `ServiceDataMst` ในฐาน `DBCenter`

(`ServiceDataId = 53`) คำนวณสต๊อกคงเหลือใหม่ห้าส่วน: วัตถุดิบของบริษัท · วัตถุดิบของลูกค้า ·

กึ่งสำเร็จรูปของบริษัท · กึ่งสำเร็จรูปของลูกค้า · สต๊อกคลังและพนักงานผลิต



\---



\## 5. บทเรียนด้านความปลอดภัย



ทุกข้อยืนยันจากซอร์สจริง เขียนแบบไม่ระบุค่าจริง กรอบคือ "เจออะไร และถ้าทำใหม่จะทำอย่างไร"



\### 5.1 รหัสผ่านเก็บเป็น plaintext และเทียบข้างใน SQL query



```csharp

// Innovation.Repositories/.../OldRepositories/SILORepository.cs

public UsrWt CheckPasswordWeighing(string user, string pass, string programID, string prgname)

{

&#x20;   return (from x in siloContext.UsrWt

&#x20;           where x.LoginName == user

&#x20;           \&\& x.Password == pass          // ← plaintext เทียบใน query

&#x20;           \&\& x.PrgId == programID

&#x20;           \&\& x.PrgName == prgname

&#x20;           select x).FirstOrDefault();

}

```



\*\*ผลกระทบ\*\*: ใครอ่านฐานข้อมูลได้ก็ได้รหัสผ่านทุกคน และการเทียบใน `where` ทำให้รหัสผ่าน

ไปโผล่ใน query plan / SQL trace / log ได้



\*\*น่าประหลาดตรงที่\*\* โซลูชันเดียวกันมี `Innovation.Authentication/Infrastructure/AESThenHMAC.cs`

(AES-then-HMAC-SHA256) อยู่แล้ว แต่ไม่ถูกใช้กับเส้นทางรหัสผ่านเลย



\*\*ถ้าทำใหม่\*\*: `PasswordHasher<TUser>` (PBKDF2) หรือ BCrypt/Argon2 · hash พร้อม salt รายผู้ใช้ ·

เทียบในชั้นแอปพลิเคชันไม่ใช่ใน LINQ predicate · ย้ายข้อมูลเดิมด้วยวิธี rehash เมื่อผู้ใช้ล็อกอินสำเร็จครั้งถัดไป



\### 5.2 รหัสผ่านฝังตายในโค้ดฝั่ง client



`admin` สำหรับรีเซ็ตพารามิเตอร์ PLC และรีเซ็ตรหัสผ่าน · `123` สำหรับตั้งค่า MAX/MIN และล็อกอินไลน์

เคยมีบั๊กที่ประตูตรวจ\*\*กลับด้าน\*\* — ใส่รหัสผิดแล้วรีเซ็ตพารามิเตอร์ PLC ได้ ซึ่งบันทึกไว้ใน

`RUNTIME\_TEST\_CHECKLIST.md` §E ว่าเป็น critical bug



\*\*ถ้าทำใหม่\*\*: ใช้การตรวจสิทธิ์ตามบทบาทจริง ไม่มีรหัสในโค้ด



\### 5.3 ข้อมูลลับอยู่ในไฟล์ที่ commit เข้ามา



รหัสผ่านฐานข้อมูลปรากฏใน `appsettings.json` และไฟล์ config รายไซต์ \*\*รวม 22 ไฟล์\*\*

พร้อมชื่อเซิร์ฟเวอร์ภายใน IP ภายใน และโดเมน AD



\*\*ถ้าทำใหม่\*\*: user-secrets ตอนพัฒนา · environment variable หรือ managed identity ตอน deploy ·

ไม่มีค่าเหล่านี้ใน repository เลย



\### 5.4 API เปิดกว้างโดยไม่มีการยืนยันตัวตน



```csharp

builder.Services.AddCors(o => o.AddPolicy("AllowAllOrigins",

&#x20;   b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

...

app.UseAuthorization();        // ← ไม่มีการลงทะเบียน authentication scheme ใดๆ

```



ไม่มี `\[Authorize]` บน controller ตัวใดเลย `UseAuthorization()` จึงไม่ได้ป้องกันอะไรจริง

และ CORS เปิดให้ทุก origin บน API ที่ให้บริการข้อมูลการผลิตจริง



\*\*ถ้าทำใหม่\*\*: JWT bearer หรือ Windows auth · CORS ระบุ origin เท่าที่จำเป็น · `\[Authorize]` เป็นค่าเริ่มต้น



\### 5.5 ข้อมูล exception รั่วออกไปหา client



```csharp

catch (Exception ex) { return StatusCode(500, ex); }    // ส่ง exception object ดิบกลับไป

catch (Exception ex) { throw ex; }                       // ทำลาย stack trace เดิม

```



`ErrorsController` ยังคืน HTTP 500 เสมอไม่ว่า `ErrorType` จะเป็น `Info`, `Werning` หรือ `Error`



\*\*ถ้าทำใหม่\*\*: RFC 7807 `ProblemDetails` · แมป error type เป็น status code ที่ถูกต้อง · ใช้ `throw;` เสมอ



\---



\## 6. บทเรียนด้านประสิทธิภาพ



\### 6.1 สร้าง AutoMapper configuration ใหม่ทุกครั้งที่แปลงข้อมูล



```csharp

// Innovation.UtilityCore/Innovation.UtilityCore.Helper/DataMappingHelper.cs

public static TDestination GetSimpleDataMap<TSource, TDestination>(TSource sourceData)

{

&#x20;   var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());

&#x20;   var mapper = new Mapper(config);

&#x20;   return mapper.Map<TDestination>(sourceData);

}

```



`MapperConfiguration` \*\*คอมไพล์ expression tree ตอนสร้าง\*\* จึงถูกออกแบบให้สร้างครั้งเดียวต่อ process

แต่ helper ตัวนี้สร้างใหม่ทุกครั้งที่เรียก และถูกเรียก \*\*1,716 จุด\*\* ทั่วชั้น service

รวมถึงเรียกข้างใน projection ราย row:



```csharp

.Select(x => DataMappingHelper.GetSimpleDataMap<OnHand, OnHandDto>(x))

```



\*\*ผลกระทบ\*\*: query ที่คืน 1,000 แถว จะคอมไพล์ mapper 1,000 ตัว นี่คือข้อบกพร่องที่วัดได้ชัดที่สุด

ในโค้ดเบสนี้ และน่าจะเป็นคำตอบของคำถาม "ทำไมระบบช้า"



\*\*ถ้าทำใหม่\*\*: `MapperConfiguration` แบบ static ตัวเดียว หรือ mapping ที่ generate ตอนคอมไพล์

(เช่น Mapperly) หรือเขียน projection เอง



\### 6.2 Transaction ข้ามฐานข้อมูลโดยไม่มีตัวประสาน



```csharp

\_dbTransectionUnitOfWork.BeginTransaction();

\_dbMasterUnitOfWork.BeginTransaction();

// ...

//    \_dbMasterUnitOfWork.CommitTransaction();     ← ถูกคอมเมนต์ทิ้ง

\_dbTransectionUnitOfWork.CommitTransaction();

```



สอง UnitOfWork = สองการเชื่อมต่อคนละฐาน ไม่มี distributed transaction

ถ้าล้มเหลวคาบเกี่ยวระหว่างสอง commit ข้อมูลจะไม่สอดคล้องกัน



\*\*ถ้าทำใหม่\*\*: ออกแบบขอบเขต transaction ให้อยู่ในฐานเดียว หรือใช้รูปแบบ outbox / saga ถ้าเลี่ยงไม่ได้



\### 6.3 ปัญหาเชิงทดสอบที่เป็นรากของทั้งหมด



```csharp

public InnovationSiloApproveService()          // constructor ไม่รับอะไรเลย

{

&#x20;   \_dbTransactionunitOfWork = UnitOfWorkFactory.GetDBTransectionUnitOfWork();   // static factory

&#x20;   ...

}

```



controller ได้ service มาจาก DI container แต่ \*\*service `new` UnitOfWork เองจาก static factory\*\*

ผลคือ service ทุกตัวที่แตะฐานข้อมูลจะ mock ไม่ได้ เขียน unit test ไม่ได้

และเปลี่ยนไปใช้ฐานข้อมูลจำลองก็ไม่ได้ \*\*นี่คือสิ่งที่ Phase 1 ของแผนสร้างใหม่แก้เป็นอันดับแรก\*\*



\---



\## 7. ของเดิม vs ของใหม่



\*\*กฎ\*\*: เอกสารพิมพ์เขียวทั้งสองไฟล์บรรยายของเดิมตามที่เป็น รวมคำสะกดผิด — ห้ามแก้ให้สวยขึ้น

เพราะคนที่เอาเอกสารไปเทียบกับซอร์สจริงต้องเห็นตรงกัน ส่วนโค้ดที่สร้างใหม่แก้ทั้งหมด



| ประเด็น | ของเดิม (บันทึกตามจริง) | ของใหม่ (สะอาด) |

|---|---|---|

| คำสะกดผิดใน public API | `Transection`, `Werning`, `Messenger`, `Confrim`, `Managment`, `Lacation`, `Brife`, `Reposiory`, `Satatus` | `Transaction`, `Warning`, `Message`, `Confirm`, `Management`, `Location`, `Brief`, `Repository`, `Status` |

| Routing | RPC — `api/\[controller]/\[action]` | REST resource + verb และ status code ที่ถูกต้อง |

| Envelope ของ response | `MyDataAPI<T>` (`Success`/`StatusCode`/`Data`/`Messenger`) | `Result<T>` + `ProblemDetails` สำหรับ error |

| การ route ตามไซต์ | `int siteId` เป็นพารามิเตอร์แรกทุกเมธอด + HTTP header `SiteID` | scoped context resolve ครั้งเดียวต่อ request |

| การได้มาซึ่ง UnitOfWork | static `UnitOfWorkFactory` เรียกใน constructor | inject `IUnitOfWorkFactory` |

| รหัสผ่าน | plaintext เทียบใน LINQ predicate | PBKDF2/BCrypt เทียบในชั้นแอป |

| ข้อความหน้าจอ | ไทย ฝังตายในฟอร์มและ presenter | resource file \*\*ไทย + อังกฤษ สลับได้ตอนรัน\*\* |

| การแตกไฟล์ partial | `Partial01/02`, suffix ชื่อคน | แยกตามฟีเจอร์จริง |

| การจัดการ error | `catch (ex) { throw ex; }`, `StatusCode(500, ex)` | exception ที่มีชนิด, `throw;`, ไม่รั่วข้อมูล |

| แปลง entity → Dto | สร้าง `MapperConfiguration` ใหม่ทุกครั้ง (1,716 จุด) | configuration เดียวแบบ static หรือ mapping ตอนคอมไพล์ |

| \*\*รูปแบบ UI\*\* | \*\*MVP\*\* | \*\*คง MVP ไว้\*\* — รูปแบบดี แต่ท่อประปาเสีย |

| การผูก view↔presenter | `IView.Presenter { set; }` แล้ว presenter ยัดตัวเองกลับ = เป็นวง | \*\*constructor injection ทางเดียว\*\* ไม่ต้อง cross-wire ด้วยมือ |

| การนำทาง | `IApplicationController` ลิสต์แบน \~40 เมธอด และคืนค่าผ่าน view | `INavigationService` แบบ generic พารามิเตอร์และผลลัพธ์มีชนิด |

| Message loop | \*\*ไม่เรียก `Application.Run()`\*\* ใช้ `ShowDialog` ซ้อนกัน | `Application.Run(mainForm)` ปกติ · dialog เป็น dialog จริง |

| การอัปเดตหน้าจอ | presenter ยัดค่าเข้า property ทีละตัว | `BindingSource` + `INotifyPropertyChanged` |

| ผลลัพธ์จาก service | ธง `bool` \~30 ตัว และ `null` เป็นผลลัพธ์เงียบ | `Result<T>` + error union แบบมีชนิด |

| รายงาน vs ปิดฟอร์ม | `NotFound()` ทำสองอย่างพร้อมกัน | แยก `Report(...)` ออกจาก `Close(...)` เด็ดขาด |

| การส่งพารามิเตอร์ให้ dialog | เขียนลง property ของ view แล้วอ่านกลับ | พารามิเตอร์เข้า–ผลลัพธ์ออก มีชนิด view เป็น private |

| การจัดการ error แบบ async | `RunSafeAsync` + `static bool \_caseActive` | tracing context แบบ `AsyncLocal` ไม่มี static state |

| ขอบเขตคลาสฐาน | `BaseForm` รู้เรื่อง PLC step parameter | คลาสฐาน UI ทำเรื่อง UI เท่านั้น |



\---



\## 8. แผนสร้างใหม่ให้รันได้จริง



\### 8.1 ทำไมต้องสร้างใหม่



ระบบจริง \*\*เปิดให้ใครดูไม่ได้\*\* เพราะต้องมี: เซิร์ฟเวอร์ SQL ของโรงงาน · PLC Mitsubishi ตัวจริง ·

ตาชั่งต่อพอร์ตอนุกรม · license DevExpress



เวอร์ชัน portfolio จึงตั้งเป้าให้ \*\*clone มาแล้วกด F5 รันได้เลย\*\*



| ด้าน | ระบบจริง | เวอร์ชัน portfolio |

|---|---|---|

| .NET | Framework 4.6.2 / 4.7.2 | \*\*.NET 8\*\* |

| UI | DevExpress v21.2 (เสียเงิน) | \*\*WinForms มาตรฐาน ไม่มี dependency ที่ต้องซื้อ\*\* |

| ฐานข้อมูล | SQL Server 16 ฐาน routing ตามไซต์ | \*\*SQLite + seed script\*\* สลับกลับเป็น SQL Server ได้ด้วย connection string |

| PLC | Mitsubishi MX Component (COM) | \*\*ตัวจำลอง\*\* — พร้อมเอกสารวิธีต่อของจริง |

| ตาชั่ง | `SerialPort` RS-232 | \*\*ตัวจำลอง\*\* |

| API | process แยก | \*\*process แยกเหมือนเดิม\*\* |



\### 8.2 ขอบเขต — เส้นเดียวแต่ครบทุกชั้น



เลือก \*\*`Innovation.TotalWeight\_PLC`\*\* เพราะเป็นแอปที่โดดเด่นที่สุด: มีทั้ง async, `HttpClient`,

`IViewBase`, PLC, ตาชั่ง, บาร์โค้ด และมีคู่ฝั่งเซิร์ฟเวอร์ชัดเจน



ทำครบตั้งแต่เดสก์ท็อป → API → ฐานข้อมูล แต่\*\*เฉพาะเส้นนี้เส้นเดียว\*\* ไม่ทำครบ 165 หน้าจอ



\### 8.3 จุดที่ต้องใส่ interface (seam)



ทั้งสามจุดนี้ยืนยันจากซอร์สแล้วว่าปัจจุบัน\*\*ไม่มี interface คั่นเลย\*\* — นี่คืองานจริงของแผนนี้



| จุด | ของจริง | ตัวจำลอง | อุปสรรคในโค้ดปัจจุบัน |

|---|---|---|---|

| ฐานข้อมูล | SQL Server 16 ฐาน | SQLite + seed | service `new` UoW จาก \*\*static `UnitOfWorkFactory`\*\* |

| PLC | MX Component `ActUtlTypeLib` | `SimulatedPlcDevice` | `ActUtlType` เป็น \*\*COM type รูปธรรมบน interface ของ view\*\* (`IView\_PLCTest.ActFXCPU1`) และถูก `new` ในฟอร์ม |

| ตาชั่ง | `SerialPort` | `SimulatedScaleReader` | อ่านตรงในฟอร์ม `frmMain.cs` |



\*\*กฎ\*\*: interface เดียว สอง implementation เลือกด้วย config — build ของ portfolio กับ build ของโรงงาน

ต่างกันแค่บรรทัดลงทะเบียนใน DI



> \*\*ข่าวดีที่ตรวจแล้ว\*\*: ทั้ง backend มี raw SQL แค่ 12 จุด (`GetDynamicBySql`) กับ 2 จุด (`FromSqlRaw`)

> และ \*\*`spRMBAL\_WITHDRAW` ไม่ใช่การเรียก stored procedure\*\* — เป็นเพียงชื่อเมธอดที่สืบชื่อ

> stored procedure สมัย Delphi มา ตัว implementation เป็น C#/EF ปกติ

> \*\*จึงไม่มี stored procedure ตัวใดขวางการย้ายไป SQLite\*\*



\### 8.4 เฟสการทำงาน



\*\*TDD ตลอดทุกเฟส — เขียนเทสต์ก่อนโค้ดเสมอ\*\*

ชุดเครื่องมือ: xUnit + FluentAssertions + NSubstitute, SQLite in-memory สำหรับเทสต์ระดับ repository



\---



\#### Phase 0 — วางฐาน + ซ่อม MVP



| | |

|---|---|

| \*\*เป้าหมาย\*\* | โครงโซลูชัน .NET 8 พร้อมสัญญา MVP ที่ซ่อมแล้ว และ CI ที่ทำ red-green ได้ตั้งแต่ commit แรก |

| \*\*ผลลัพธ์\*\* | โครงโฟลเดอร์ · สัญญา MVP ในไลบรารีกลาง \*\*ไลบรารีเดียว ไม่คัดลอก\*\* · DI · logging · โปรเจกต์เทสต์ |

| \*\*ขึ้นกับ\*\* | — |

| \*\*เทสต์ที่เขียนก่อน\*\* | สัญญาของ `INavigationService` · lifecycle ของ presenter |

| \*\*เสร็จเมื่อ\*\* | แอปเปล่าเปิดขึ้นด้วย `Application.Run(mainForm)` และ CI เขียวจากเทสต์จริง |



สี่จุดที่ซ่อมจากของเดิม พร้อมเหตุผล — ส่วนนี้คือเนื้อหาที่ดีที่สุดสำหรับเล่าในสัมภาษณ์:



1\. \*\*ตัด `Presenter { set; }` ทิ้ง ใช้ constructor injection ทางเดียว\*\* — ของเดิม presenter ยัดตัวเอง

&#x20;  กลับเข้า view (`\_view.Presenter = this`) ทำให้เป็นวง จน `TotalWeight\_PLC` ต้องผูกฟอร์ม

&#x20;  singleton สองตัวเข้าหากันด้วยมือหลัง `BuildServiceProvider()`

2\. \*\*`INavigationService` แทน `IApplicationController`\*\* — ของเดิมเป็นลิสต์แบน \~40 เมธอด

&#x20;  และคืนค่าผลลัพธ์ผ่าน public property ของ view

3\. \*\*`Application.Run(mainForm)` จริง\*\* — ของเดิมไม่เคยเรียก ใช้ `ShowDialog` ซ้อนกันแทน message loop

4\. \*\*`BindingSource` + `INotifyPropertyChanged`\*\* — ของเดิม presenter ยัดค่าเข้า property ทีละตัว



\---



\#### Phase 1 — ชั้นข้อมูล + การแก้เชิงสถาปัตยกรรมที่สำคัญที่สุด



| | |

|---|---|

| \*\*เป้าหมาย\*\* | ฐานข้อมูล SQLite ที่ทำงานเหมือนของจริง และทำให้ทุกอย่าง\*\*ทดสอบได้\*\* |

| \*\*ผลลัพธ์\*\* | schema เฉพาะตารางที่เส้นชั่งใช้ · EF Core 8 · `IRepository<T>` + `IUnitOfWork` · \*\*`IUnitOfWorkFactory` แบบ inject ได้\*\* · seed script ข้อมูลสมจริง |

| \*\*ขึ้นกับ\*\* | Phase 0 |

| \*\*เทสต์ที่เขียนก่อน\*\* | contract test ของ repository บน SQLite in-memory |

| \*\*เสร็จเมื่อ\*\* | repository test เขียวทั้งหมด และสลับไป SQL Server ได้ด้วยการเปลี่ยน connection string |



> \*\*การกลับด้าน `UnitOfWorkFactory` จาก static เป็น injected คือสิ่งที่มีค่าที่สุดที่จะโชว์\*\*

> เพราะมันคือสิ่งเดียวที่ทำให้ TDD เป็นไปได้ และทำให้สลับฐานข้อมูลจริง/จำลองได้



ตารางที่ต้องมีใน SQLite (ดู ER ใน Backend ROADMAP §8.4): จากฐาน `SILO` — `KbTogether`, `Weighting`,

`TotalWeight`, `TwAcceptWeightHis`, `SendStepParameter`, `Station`, `UsrWt`, `TrayPlan`, `TrayWeight`,

`TrayBarcode`, `TypeTray` · จากฐานอื่น — `RM\_BAL`, `SiloApprove`, `OnHand`, `PRODSTD\_MIXTEMP`,

`Application\_Setting`



\---



\#### Phase 2 — API



| | |

|---|---|

| \*\*เป้าหมาย\*\* | ASP.NET Core 8 \*\*เป็น process แยก\*\* REST จริง มี authentication พร้อม hash รหัสผ่าน |

| \*\*ผลลัพธ์\*\* | endpoint แบ่งเป็นชั้น T1/T2/T3 · `Result<T>` + `ProblemDetails` · Swagger |

| \*\*ขึ้นกับ\*\* | Phase 1 |

| \*\*เทสต์ที่เขียนก่อน\*\* | endpoint test ผ่าน `WebApplicationFactory` |

| \*\*เสร็จเมื่อ\*\* | T1 + T2 ผ่านเทสต์ครบ และ Swagger เปิดดูได้ |



\*\*นี่คืองาน port ไม่ใช่งานออกแบบใหม่\*\* — ฝั่งเซิร์ฟเวอร์มีอยู่แล้วเป็น `TotalWeightPlcController`

(358 บรรทัด \*\*52 action\*\*) + `ITotalWeightPlcService` + `DtoModel/TotalWeightPlc/`

และจัดกลุ่มตามฟอร์มที่เรียกอยู่แล้ว



`Service\_TotalWeightPlc` เรียก \*\*57 endpoint\*\* ผ่านเมธอด \*\*68 ตัว\*\* ซึ่งไม่ใช่จำนวนน้อย จึงต้องแบ่งชั้น:



| ชั้น | ขอบเขต | ตัวอย่าง endpoint |

|---|---|---|

| \*\*T1\*\* | เส้นชั่งปกติ | `GetUsrWt`, `GetKanban`, `SelectKanban`, `CheckBarcode`, `GetPlanDataByBarcode`, `GetMaxStepByBarcode`, `SaveTotalWeight`, `InsertTotalWeight`, `AcceptNonManual`, `IsTotalWeightExists` |

| \*\*T2\*\* | auto-feed | `CheckBarcodeAuto`, `GetRmBal`, `GET\_BAL`, `ExecuteRmBalWithdraw`, `GetDropDoorSteps`, `ChkMixingPattern`, `GetProdstdMixtempByPlanId` |

| \*\*T3\*\* | ที่เหลือ | ถาด · โหมด manual · ยกเลิก/ผ่านคัมบัง · cleaning · HF-mixing — \*\*stub ไว้\*\* คืน `NotImplemented` |



\*\*demo ถือว่าครบเมื่อทำ T1 + T2 ได้\*\* ส่วน T3 คือส่วนที่ตัดได้ถ้าเวลาไม่พอ



\---



\#### Phase 3 — แยกฮาร์ดแวร์ออกเป็น interface



| | |

|---|---|

| \*\*เป้าหมาย\*\* | รันได้โดยไม่ต้องมี PLC และตาชั่งจริง แต่ไม่ทิ้งเส้นทางของจริง |

| \*\*ผลลัพธ์\*\* | `IPlcDevice` · `IScaleReader` · `IBarcodeSource` แต่ละตัวมี implementation จริงและจำลอง |

| \*\*ขึ้นกับ\*\* | Phase 0 |

| \*\*เทสต์ที่เขียนก่อน\*\* | สถานการณ์จำลองแต่ละแบบ \*\*คือ\*\* test fixture ในตัว |

| \*\*เสร็จเมื่อ\*\* | เดินครบทุกสถานการณ์ด้านล่างได้โดยไม่ต้องต่อฮาร์ดแวร์ |



สถานการณ์ที่ตัวจำลองต้องทำได้ อ่านตรงจาก `RUNTIME\_TEST\_CHECKLIST.md` ของโปรเจกต์เดิม:



\- ชั่งปกติจนจบ

\- น้ำหนักนอกช่วง \[ต่ำสุด, สูงสุด]

\- PLC ต่อไม่ติด / timeout

\- ไม่พบบาร์โค้ดใน `RM\_BAL` → เตือนแต่\*\*ฟอร์มต้องไม่ปิด\*\*

\- ไม่ได้ตั้งค่า Feeddoor Step → ข้ามการเขียนประตู \*\*ฟอร์มต้องไม่ปิด\*\*

\- `PRODSTD\_MIXTEMP` ไม่มีแถว → ชั่งต่อได้ \*\*ฟอร์มต้องไม่ปิด\*\*

\- เขียนฐานข้อมูลไม่สำเร็จระหว่าง auto-feed → เตือนแบบไม่ปิดฟอร์ม



> สังเกตว่าสี่ข้อหลังคือกรณี "ต้องไม่ปิดฟอร์ม" ทั้งหมด — เพราะของเดิมผูก `NotFound()`

> ไว้กับการปิดฟอร์ม ทำให้แอปเด้งออกกลางการทำงาน ตัวจำลองจึงต้องพิสูจน์ว่าของใหม่แยกสองเรื่องนี้แล้วจริง



เอกสารเส้นทางของจริงที่ต้องไม่หาย: การลงทะเบียน COM ของ MX Component · หมายเลข logical station ·

แผนที่ address จาก `SEND\_STEP\_PARAMETER` · การตั้งค่าพอร์ตอนุกรมของตาชั่ง



\---



\#### Phase 4 — หน้าจอ



| | |

|---|---|

| \*\*เป้าหมาย\*\* | หน้าจอหลักบน WinForms มาตรฐาน ทำงานได้จริงบน MVP ที่ซ่อมแล้ว |

| \*\*ผลลัพธ์\*\* | `frmTotalWeight` (843×608 164 control) เป็น main form ของ `Application.Run` แล้วตามด้วย dialog ที่มันเรียก |

| \*\*ขึ้นกับ\*\* | Phase 1, 2, 3 |

| \*\*เทสต์ที่เขียนก่อน\*\* | presenter test กับ view ที่ mock ไว้ — ทำได้เพราะการผูกเป็นทางเดียวแล้ว |

| \*\*เสร็จเมื่อ\*\* | เดินเส้นชั่งครบตั้งแต่ยิงบาร์โค้ดจนบันทึกได้ |



\- \*\*i18n ตั้งแต่บรรทัดแรก\*\* — ห้ามมี string ในฟอร์ม ใช้ `.resx` ไทย/อังกฤษ + ปุ่มสลับภาษาตอนรัน

&#x20; (ย้ายข้อความเดิมแบบ 1:1 ก่อน แล้วค่อยเพิ่มภาษาอังกฤษ)

\- แทน DevExpress ด้วย control มาตรฐานตามตารางใน Frontend ROADMAP §8.4

&#x20; (`GridControl` → `DataGridView`, `RibbonControl` → `MenuStrip`+`ToolStrip`, ฯลฯ)

\- คง \*\*hotkey บนปุ่ม\*\* ไว้ (`\[F5]`, `\[F2]`, `\[Esc]`) เพราะผู้ใช้หน้าโรงงานใส่ถุงมือ ใช้คีย์บอร์ดเป็นหลัก



\---



\#### Phase 5 — Integration และ E2E



| | |

|---|---|

| \*\*เป้าหมาย\*\* | พิสูจน์ว่าทั้งสามชั้นทำงานร่วมกันจริง |

| \*\*ผลลัพธ์\*\* | เทสต์ full-stack เดสก์ท็อป→API→SQLite · เครื่องมือ tracing แทน `CallTracer` · รายงาน coverage |

| \*\*ขึ้นกับ\*\* | Phase 4 |

| \*\*เสร็จเมื่อ\*\* | เส้นชั่งปกติ + สถานการณ์ล้มเหลวทั้งเจ็ดแบบผ่านครบ |



\---



\#### Phase 6 — เก็บงานสำหรับ portfolio



| | |

|---|---|

| \*\*ผลลัพธ์\*\* | ภาพหน้าจอ (ทำได้หลัง Phase 4) · สคริปต์เดินชม demo · launch profile ให้สอง process เปิดพร้อมกัน · หัวข้อ "ถ้าย้อนกลับไปจะทำต่างจากนี้อย่างไร" |



\---



\### 8.5 ลำดับความสำคัญถ้าเวลาไม่พอ



| ตัดได้ | ตัดไม่ได้ |

|---|---|

| T3 endpoint (ถาด manual cancel cleaning) | การกลับด้าน `UnitOfWorkFactory` (Phase 1) |

| หน้าจอรองทั้งหมด | `frmTotalWeight` + dialog ที่จำเป็น |

| ภาษาอังกฤษ (ทำ resource file ไว้ก่อนแต่ยังไม่แปลครบ) | โครงสร้าง i18n |

| coverage report | เทสต์เส้นชั่งปกติ |

| Phase 6 ทั้งหมด | Phase 0–3 |



\---



\## 9. เอกสารในชุดนี้



| ไฟล์ | เนื้อหา | ความยาว |

|---|---|---|

| `README.md` (ไฟล์นี้) | ภาพรวม โดเมน บทเรียน และแผนสร้างใหม่ | — |

| \[Backend (The Server-Side)/ROADMAP.md](Backend%20\\(The%20Server-Side\\)/ROADMAP.md) | โครงสร้าง 519 โฟลเดอร์ · interface contract · DI · ฐานข้อมูล/ER · auth · D365 · ระบบรุ่นก่อน | \~1,640 บรรทัด |

| \[Frontend (The Client-Side)/ROADMAP.md](Frontend%20\\(The%20Client-Side\\)/ROADMAP.md) | โครงสร้าง 397 โฟลเดอร์ · สัญญา MVP · คลังหน้าจอ 165 ฟอร์ม · wireframe · PLC/ฮาร์ดแวร์ | \~1,900 บรรทัด |







