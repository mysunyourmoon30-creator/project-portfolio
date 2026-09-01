namespace Innovation.Core.Entities;

public class KbTogether
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public int FormulationId { get; set; }
    public int LineId { get; set; }

    // Count of kanbans bundled together for simultaneous weighing.
    // WeightToleranceCalculator branches on this value for steps 2/3.
    public int Number { get; set; }

    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

public class Weighting
{
    public int Id { get; set; }
    public int KbTogetherId { get; set; }
    public int StepNo { get; set; }
    public string RawMaterialCode { get; set; } = string.Empty;
    public decimal TargetWeight { get; set; }
    public decimal? ActualWeight { get; set; }
    public bool Accepted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TotalWeight
{
    public int Id { get; set; }
    public int KbTogetherId { get; set; }
    public decimal TotalActualWeight { get; set; }
    public DateTime SavedAt { get; set; }
    public int SavedByUserId { get; set; }
}

public class TwAcceptWeightHis
{
    public int Id { get; set; }
    public int TotalWeightId { get; set; }
    public int StepNo { get; set; }
    public decimal AcceptedWeight { get; set; }
    public DateTime AcceptedAt { get; set; }
    public int AcceptedByUserId { get; set; }
}

public class SendStepParameter
{
    public int Id { get; set; }
    public int StepNo { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PlcAddress { get; set; } = string.Empty;
    public int? LineId { get; set; }
}
