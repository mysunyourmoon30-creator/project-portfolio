using Innovation.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Innovation.Services.Security;

// PBKDF2 via Microsoft.AspNetCore.Identity.PasswordHasher<UsrWt>, replacing
// the original's plaintext-compared-in-a-LINQ-predicate password column
// (Backend ROADMAP §9.1). A password never travels through a SQL/LINQ
// predicate here - it is only ever compared via VerifyHashedPassword.
public sealed class UsrWtPasswordHasher
{
    private readonly PasswordHasher<UsrWt> _hasher = new();

    public string Hash(UsrWt user, string password) => _hasher.HashPassword(user, password);

    public bool Verify(UsrWt user, string password) =>
        _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
}
