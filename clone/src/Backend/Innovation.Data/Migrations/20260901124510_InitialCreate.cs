using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Innovation.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KbTogether",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    FormulationId = table.Column<int>(type: "INTEGER", nullable: false),
                    LineId = table.Column<int>(type: "INTEGER", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KbTogether", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnHand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemCode = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnHand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdstdMixtemp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    MixPattern = table.Column<string>(type: "TEXT", nullable: false),
                    Temperature = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdstdMixtemp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RmBal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RawMaterialBarcode = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmBal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendStepParameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StepNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PlcAddress = table.Column<string>(type: "TEXT", nullable: false),
                    LineId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendStepParameter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiloApprove",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    LineId = table.Column<int>(type: "INTEGER", nullable: false),
                    Approved = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiloApprove", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LineId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalWeight",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KbTogetherId = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalActualWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    SavedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SavedByUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalWeight", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrayBarcode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    TypeTrayId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrayBarcode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrayPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KbTogetherId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrayBarcodeId = table.Column<int>(type: "INTEGER", nullable: true),
                    PlannedWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrayPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrayWeight",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrayPlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActualWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    WeighedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrayWeight", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwAcceptWeightHis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TotalWeightId = table.Column<int>(type: "INTEGER", nullable: false),
                    StepNo = table.Column<int>(type: "INTEGER", nullable: false),
                    AcceptedWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcceptedByUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwAcceptWeightHis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeTray",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MaxCapacity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeTray", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsrWt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoginName = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    ProgramId = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsrWt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weighting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KbTogetherId = table.Column<int>(type: "INTEGER", nullable: false),
                    StepNo = table.Column<int>(type: "INTEGER", nullable: false),
                    RawMaterialCode = table.Column<string>(type: "TEXT", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    ActualWeight = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    Accepted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weighting", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KbTogether_Barcode",
                table: "KbTogether",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_ProdstdMixtemp_PlanId",
                table: "ProdstdMixtemp",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RmBal_RawMaterialBarcode",
                table: "RmBal",
                column: "RawMaterialBarcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiloApprove_Barcode",
                table: "SiloApprove",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_TotalWeight_KbTogetherId",
                table: "TotalWeight",
                column: "KbTogetherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrayBarcode_Barcode",
                table: "TrayBarcode",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsrWt_LoginName",
                table: "UsrWt",
                column: "LoginName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSetting");

            migrationBuilder.DropTable(
                name: "KbTogether");

            migrationBuilder.DropTable(
                name: "OnHand");

            migrationBuilder.DropTable(
                name: "ProdstdMixtemp");

            migrationBuilder.DropTable(
                name: "RmBal");

            migrationBuilder.DropTable(
                name: "SendStepParameter");

            migrationBuilder.DropTable(
                name: "SiloApprove");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "TotalWeight");

            migrationBuilder.DropTable(
                name: "TrayBarcode");

            migrationBuilder.DropTable(
                name: "TrayPlan");

            migrationBuilder.DropTable(
                name: "TrayWeight");

            migrationBuilder.DropTable(
                name: "TwAcceptWeightHis");

            migrationBuilder.DropTable(
                name: "TypeTray");

            migrationBuilder.DropTable(
                name: "UsrWt");

            migrationBuilder.DropTable(
                name: "Weighting");
        }
    }
}
