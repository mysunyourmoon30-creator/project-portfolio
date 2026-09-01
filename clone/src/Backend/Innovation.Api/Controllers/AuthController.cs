using Innovation.Api.Common;
using Innovation.Core.UnitOfWork;
using Innovation.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innovation.Api.Controllers;

public record LoginResponse(string Token, string Username, string FullName);

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly ITotalWeightPlcService _service;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly JwtTokenIssuer _tokenIssuer;

    public AuthController(ITotalWeightPlcService service, IUnitOfWorkFactory unitOfWorkFactory, JwtTokenIssuer tokenIssuer)
    {
        _service = service;
        _unitOfWorkFactory = unitOfWorkFactory;
        _tokenIssuer = tokenIssuer;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequestDto request)
    {
        var result = _service.Login(request);

        using var uow = _unitOfWorkFactory.CreateSiloUnitOfWork();
        var user = uow.UsrWtRepository.Get(result.UserId)!;

        var token = _tokenIssuer.Issue(user);
        return Ok(new LoginResponse(token, result.Username, result.FullName));
    }
}
