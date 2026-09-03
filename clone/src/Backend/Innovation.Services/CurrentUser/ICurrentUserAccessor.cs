namespace Innovation.Services.CurrentUser;

// Resolved once per request from the authenticated JWT claims (see
// Innovation.Api.Common.CurrentUserAccessor). Mirrors ICurrentSiteAccessor's
// "resolve once via DI instead of threading a parameter everywhere" pattern.
public interface ICurrentUserAccessor
{
    int UserId { get; }
}
