namespace Innovation.TotalWeight_PLC.Infrastructure;

public interface IAuthSession
{
    string? Token { get; set; }
}

public sealed class AuthSession : IAuthSession
{
    public string? Token { get; set; }
}
