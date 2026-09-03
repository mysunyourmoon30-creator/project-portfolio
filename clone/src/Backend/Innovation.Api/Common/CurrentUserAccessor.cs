using System.Security.Claims;
using Innovation.Services.CurrentUser;

namespace Innovation.Api.Common;

// Lives here rather than Innovation.Services because it needs
// IHttpContextAccessor, which requires the ASP.NET Core framework reference
// that Innovation.Services (plain net8.0) deliberately doesn't carry - same
// reason JwtTokenIssuer lives here instead of Innovation.Services.
public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public int UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("No authenticated user on the current request.");
            return int.Parse(claim.Value);
        }
    }
}
