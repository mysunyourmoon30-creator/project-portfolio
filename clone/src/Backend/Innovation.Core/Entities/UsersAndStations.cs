namespace Innovation.Core.Entities;

public class Station
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LineId { get; set; }
}

public class UsrWt
{
    public int Id { get; set; }
    public string LoginName { get; set; } = string.Empty;

    // PBKDF2 hash via Microsoft.AspNetCore.Identity.PasswordHasher<UsrWt>
    // (Phase 2) - the original stored and compared this column as plaintext
    // inside a LINQ predicate (Backend ROADMAP §9.1). Never store plaintext
    // here.
    public string PasswordHash { get; set; } = string.Empty;

    public string ProgramId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
