namespace Innovation.Services.Contracts;

// T1 (normal weighing) + T2 (auto-feed) methods only. T3 (trays, manual
// mode, cleaning, etc.) is out of scope per README §8.5 and stubbed as 501
// directly in the API controllers, not declared here.
public interface ITotalWeightPlcService
{
    // T1
    LoginResultDto Login(LoginRequestDto request);
    KanbanDetailDto GetKanban(string barcode);
    List<KanbanSummaryDto> GetPendingKanbans();
    SaveTotalWeightResultDto SaveTotalWeight(SaveTotalWeightRequestDto request);
    void Accept(AcceptStepRequestDto request);
    bool TotalWeightExists(int kbTogetherId);

    // T2
    RmBalDto GetRmBal(string barcode);
    void ExecuteRmBalWithdraw(string barcode, decimal amount);
    FeeddoorStepDto GetFeeddoorStep(int lineId);
    MixTempDto? GetMixTemp(int planId);
}
