namespace Innovation.TotalWeight_PLC.ViewModel;

public record KanbanSummary(int KbTogetherId, string Barcode, string Status);

public record SelectKbRequest(List<KanbanSummary> Candidates);
public record SelectKbResult(KanbanSummary? Selected);

public record AutoFeedRequest(string Barcode, int LineId, int PlanId);
public record AutoFeedResult(bool Success);
