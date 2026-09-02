namespace Innovation.Services.Contracts;

public record LoginRequestDto(string Username, string Password);
public record LoginResultDto(int UserId, string Username, string FullName);

public record KanbanStepDto(int StepNo, string RawMaterialCode, decimal Target, decimal Min, decimal Max, decimal? Actual, bool Accepted);
public record KanbanDetailDto(int KbTogetherId, string Barcode, int PlanId, int Number, string Status, List<KanbanStepDto> Steps);
public record KanbanSummaryDto(int KbTogetherId, string Barcode, string Status);

public record StepWeightDto(int StepNo, decimal ActualWeight);
public record SaveTotalWeightRequestDto(int KbTogetherId, List<StepWeightDto> Steps);
public record SaveTotalWeightResultDto(int TotalWeightId, decimal TotalActualWeight);

public record AcceptStepRequestDto(int KbTogetherId, int StepNo);

public record RmBalDto(string Barcode, decimal Balance);
public record FeeddoorStepDto(int StepNo, string PlcAddress, string Description);
public record MixTempDto(int PlanId, string MixPattern, decimal Temperature);
