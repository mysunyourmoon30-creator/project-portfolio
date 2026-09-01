using Innovation.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Innovation.Data.Seed;

// Application_Setting values below are INVENTED, not extracted from the real
// system (those are confidential - see README.md's plan-approval notes).
// Seeds exactly enough data for one full T1+T2 weighing walkthrough:
// one station, one operator, one ready-to-weigh kanban with a single step,
// one RM_BAL row it will withdraw from, and one PRODSTD_MIXTEMP row.
public static class DemoDataSeeder
{
    public static void Seed(SiloDbContext db)
    {
        if (db.Station.Any())
        {
            return; // already seeded
        }

        var station = new Station { Name = "Station-1", LineId = 1 };
        db.Station.Add(station);

        var hasher = new PasswordHasher<UsrWt>();
        var operatorUser = new UsrWt
        {
            LoginName = "operator1",
            ProgramId = "ERP098",
            FullName = "Demo Operator",
        };
        operatorUser.PasswordHash = hasher.HashPassword(operatorUser, "Password123!");
        db.UsrWt.Add(operatorUser);

        db.ApplicationSetting.AddRange(
            new ApplicationSetting { Id = 4, Value = "0.5", Description = "Weight tolerance - min offset (invented default)" },
            new ApplicationSetting { Id = 5, Value = "0.5", Description = "Weight tolerance - max offset (invented default)" },
            new ApplicationSetting { Id = 23, Value = "1", Description = "EnabledWeightInput (invented default)" },
            new ApplicationSetting { Id = 24, Value = "1", Description = "CheckMixingFinished (invented default)" });

        db.SendStepParameter.Add(new SendStepParameter
        {
            StepNo = 2,
            Description = "Feeddoor Step",
            PlcAddress = "D70",
            LineId = 1,
        });

        var kanban = new KbTogether
        {
            Barcode = "KB0000001",
            PlanId = 1,
            FormulationId = 1,
            LineId = 1,
            Number = 1,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };
        db.KbTogether.Add(kanban);
        db.SaveChanges();

        db.Weighting.Add(new Weighting
        {
            KbTogetherId = kanban.Id,
            StepNo = 1,
            RawMaterialCode = "RM001",
            TargetWeight = 10.00m,
            CreatedAt = DateTime.UtcNow,
        });

        db.RmBal.Add(new RmBal
        {
            RawMaterialBarcode = "RM001",
            Balance = 500.00m,
            UpdatedAt = DateTime.UtcNow,
        });

        db.ProdstdMixtemp.Add(new ProdstdMixtemp
        {
            PlanId = 1,
            MixPattern = "Standard",
            Temperature = 65.0m,
        });

        db.SaveChanges();
    }
}
