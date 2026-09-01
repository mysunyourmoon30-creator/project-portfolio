using Innovation.Services.Contracts;

namespace Innovation.TotalWeight_PLC.Service;

// Desktop-side HTTP client contract, backed by a typed HttpClient
// (IHttpClientFactory) rather than the original's `static readonly
// HttpClient` (Frontend ROADMAP §6b). DTOs are shared directly with the API
// project (both live in this one solution) instead of duplicated as
// parallel VM classes purely to avoid ~10 records' worth of boilerplate -
// see docs/RETROSPECTIVE.md for why this one convention was NOT preserved.
public interface IApiClient
{
    Task<LoginResultAndToken> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<KanbanDetailDto> GetKanbanAsync(string barcode, CancellationToken ct = default);
    Task<SaveTotalWeightResultDto> SaveTotalWeightAsync(SaveTotalWeightRequestDto request, CancellationToken ct = default);
    Task AcceptAsync(AcceptStepRequestDto request, CancellationToken ct = default);
    Task<RmBalDto> GetRmBalAsync(string barcode, CancellationToken ct = default);
    Task WithdrawRmBalAsync(string barcode, decimal amount, CancellationToken ct = default);
    Task<FeeddoorStepDto> GetFeeddoorStepAsync(int lineId, CancellationToken ct = default);
    Task<MixTempDto?> GetMixTempAsync(int planId, CancellationToken ct = default);
}

public record LoginResultAndToken(string Token, string Username, string FullName);
