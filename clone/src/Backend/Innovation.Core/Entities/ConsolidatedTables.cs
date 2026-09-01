namespace Innovation.Core.Entities;

// In the real system these five tables live in separate SQL Server
// databases (Backend ROADMAP §8.1). This clone deliberately consolidates
// them into the same SiloDbContext as the native SILO tables, applying
// README §6.2's own recommendation ("keep the transaction boundary inside
// one database") instead of reproducing the cross-database transaction bug
// documented there.
public class RmBal
{
    public int Id { get; set; }
    public string RawMaterialBarcode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SiloApprove
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int LineId { get; set; }
    public bool Approved { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class OnHand
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public int LocationId { get; set; }
}

public class ProdstdMixtemp
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public string MixPattern { get; set; } = string.Empty;
    public decimal Temperature { get; set; }
}

// Setting_ID 4/5 = weight tolerance min/max offsets, 23 = EnabledWeightInput,
// 24 = CheckMixingFinished (Backend ROADMAP §8.3). Real values are
// confidential; DemoDataSeeder invents reasonable defaults.
public class ApplicationSetting
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
