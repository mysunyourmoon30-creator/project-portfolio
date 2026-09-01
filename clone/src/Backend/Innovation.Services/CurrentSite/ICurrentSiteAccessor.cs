namespace Innovation.Services.CurrentSite;

// Replaces `int siteId` as the first parameter of every original method plus
// the `SiteID` HTTP header (Backend ROADMAP §4.4/§9). Resolved once per
// request instead. Hardcoded to 1 for this single-site demo slice (see
// README §8.2 - only one vertical slice is built).
public interface ICurrentSiteAccessor
{
    int SiteId { get; }
}

public sealed class CurrentSiteAccessor : ICurrentSiteAccessor
{
    public int SiteId => 1;
}
