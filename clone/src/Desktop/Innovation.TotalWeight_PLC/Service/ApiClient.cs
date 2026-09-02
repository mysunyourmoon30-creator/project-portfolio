using System.Net;
using System.Net.Http.Json;
using Innovation.Services.Contracts;
using Innovation.Services.Errors;

namespace Innovation.TotalWeight_PLC.Service;

public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResultAndToken> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequestDto(username, password), ct);
        await ThrowIfError(response, ct);
        var body = await response.Content.ReadFromJsonAsync<LoginResponseBody>(cancellationToken: ct);
        return new LoginResultAndToken(body!.Token, body.Username, body.FullName);
    }

    public async Task<List<KanbanSummaryDto>> GetPendingKanbansAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync("api/kanbans", ct);
        await ThrowIfError(response, ct);
        return (await response.Content.ReadFromJsonAsync<List<KanbanSummaryDto>>(cancellationToken: ct))!;
    }

    public async Task<KanbanDetailDto> GetKanbanAsync(string barcode, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/kanban/{Uri.EscapeDataString(barcode)}", ct);
        await ThrowIfError(response, ct);
        return (await response.Content.ReadFromJsonAsync<KanbanDetailDto>(cancellationToken: ct))!;
    }

    public async Task<SaveTotalWeightResultDto> SaveTotalWeightAsync(SaveTotalWeightRequestDto request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/totalweight", request, ct);
        await ThrowIfError(response, ct);
        return (await response.Content.ReadFromJsonAsync<SaveTotalWeightResultDto>(cancellationToken: ct))!;
    }

    public async Task AcceptAsync(AcceptStepRequestDto request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/totalweight/accept", request, ct);
        await ThrowIfError(response, ct);
    }

    public async Task<RmBalDto> GetRmBalAsync(string barcode, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/rm-bal/{Uri.EscapeDataString(barcode)}", ct);
        await ThrowIfError(response, ct);
        return (await response.Content.ReadFromJsonAsync<RmBalDto>(cancellationToken: ct))!;
    }

    public async Task WithdrawRmBalAsync(string barcode, decimal amount, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/rm-bal/{Uri.EscapeDataString(barcode)}/withdraw", new { amount }, ct);
        await ThrowIfError(response, ct);
    }

    public async Task<FeeddoorStepDto> GetFeeddoorStepAsync(int lineId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/lines/{lineId}/feeddoor-step", ct);
        await ThrowIfError(response, ct);
        return (await response.Content.ReadFromJsonAsync<FeeddoorStepDto>(cancellationToken: ct))!;
    }

    public async Task<MixTempDto?> GetMixTempAsync(int planId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/plans/{planId}/mix-temp", ct);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }

        await ThrowIfError(response, ct);
        return await response.Content.ReadFromJsonAsync<MixTempDto?>(cancellationToken: ct);
    }

    // Translates the API's RFC 7807 ProblemDetails responses back into the
    // same typed exceptions the API itself throws (Innovation.Services.Errors)
    // - shared directly since both ends live in this one solution, instead
    // of maintaining a parallel exception hierarchy purely for the sake of
    // physical separation.
    private static async Task ThrowIfError(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(ct);

        throw response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new InvalidCredentialsException(),
            HttpStatusCode.NotFound when body.Contains("barcode-not-found") => new BarcodeNotFoundException(""),
            HttpStatusCode.NotFound when body.Contains("rm-bal-not-found") => new RmBalNotFoundException(""),
            HttpStatusCode.NotFound when body.Contains("setting-not-found") => new SettingNotFoundException(""),
            HttpStatusCode.Conflict when body.Contains("total-weight-exists") => new TotalWeightAlreadyExistsException(0),
            HttpStatusCode.Conflict when body.Contains("step-not-accepted") => new StepNotAcceptedException(0),
            _ => new HttpRequestException($"API call failed: {(int)response.StatusCode} {body}"),
        };
    }

    private sealed record LoginResponseBody(string Token, string Username, string FullName);
}
