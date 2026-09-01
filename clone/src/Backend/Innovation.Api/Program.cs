using System.Text;
using Innovation.Api.Common;
using Innovation.Api.Middleware;
using Innovation.Core.UnitOfWork;
using Innovation.Data;
using Innovation.Services.Contracts;
using Innovation.Services.CurrentSite;
using Innovation.Services.Implementations;
using Innovation.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Silo") ?? "Data Source=totalweight.db";
builder.Services.AddDbContextFactory<SiloDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IUnitOfWorkFactory, Innovation.Repositories.UnitOfWorkFactory>();
builder.Services.AddSingleton<UsrWtPasswordHasher>();
builder.Services.AddSingleton<ICurrentSiteAccessor, CurrentSiteAccessor>();
builder.Services.AddScoped<ITotalWeightPlcService, TotalWeightPlcService>();
builder.Services.AddSingleton<JwtTokenIssuer>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "demo-only-signing-key-not-for-production-use-32bytes!";
builder.Configuration["Jwt:Key"] = jwtKey;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

// [Authorize] is the default for every endpoint, fixing the original's
// UseAuthorization() call with no authentication scheme registered at all
// (Backend ROADMAP §12) - anonymous access must now be opted into
// explicitly with [AllowAnonymous], as AuthController does.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string corsPolicy = "DesktopClient";
builder.Services.AddCors(o => o.AddPolicy(corsPolicy, b => b
    .WithOrigins("http://localhost", "https://localhost")
    .AllowAnyMethod()
    .AllowAnyHeader()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SiloDbContext>>();
    using var db = dbFactory.CreateDbContext();
    db.Database.Migrate();
    Innovation.Data.Seed.DemoDataSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ProblemDetailsExceptionMapper.Register(app);

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { } // exposed for WebApplicationFactory<Program> in tests
