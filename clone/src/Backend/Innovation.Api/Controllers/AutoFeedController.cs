using Innovation.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Innovation.Api.Controllers;

// T2 - auto-feed (README §8, Backend ROADMAP §7c).
[ApiController]
[Route("api")]
public class AutoFeedController : ControllerBase
{
    private readonly ITotalWeightPlcService _service;

    public AutoFeedController(ITotalWeightPlcService service) => _service = service;

    [HttpGet("rm-bal/{barcode}")]
    public ActionResult<RmBalDto> GetRmBal(string barcode) => Ok(_service.GetRmBal(barcode));

    public record WithdrawRequest(decimal Amount);

    [HttpPost("rm-bal/{barcode}/withdraw")]
    public IActionResult Withdraw(string barcode, [FromBody] WithdrawRequest request)
    {
        _service.ExecuteRmBalWithdraw(barcode, request.Amount);
        return NoContent();
    }

    [HttpGet("lines/{lineId:int}/feeddoor-step")]
    public ActionResult<FeeddoorStepDto> GetFeeddoorStep(int lineId) => Ok(_service.GetFeeddoorStep(lineId));

    [HttpGet("plans/{planId:int}/mix-temp")]
    public ActionResult<MixTempDto?> GetMixTemp(int planId) => Ok(_service.GetMixTemp(planId));
}
