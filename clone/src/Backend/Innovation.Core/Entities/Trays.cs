namespace Innovation.Core.Entities;

// Tray-related tables are T3 scope (README §8.5, cuttable) - modeled for
// schema completeness but not exercised by the T1/T2 API surface.
public class TrayPlan
{
    public int Id { get; set; }
    public int KbTogetherId { get; set; }
    public int? TrayBarcodeId { get; set; }
    public decimal PlannedWeight { get; set; }
}

public class TrayWeight
{
    public int Id { get; set; }
    public int TrayPlanId { get; set; }
    public decimal ActualWeight { get; set; }
    public DateTime WeighedAt { get; set; }
}

public class TrayBarcode
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int TypeTrayId { get; set; }
}

public class TypeTray
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MaxCapacity { get; set; }
}
