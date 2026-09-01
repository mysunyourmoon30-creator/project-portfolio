using System.Net.Http.Headers;

namespace Innovation.TotalWeight_PLC.Infrastructure;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthSession _session;

    public AuthHeaderHandler(IAuthSession session) => _session = session;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_session.Token is { } token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
