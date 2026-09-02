using Innovation.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Innovation.Api.Controllers;

// T1 - the normal weighing path (README §8, Backend ROADMAP §7c).
[ApiController]
[Route("api")]
public class KanbanController : ControllerBase
{
    private readonly ITotalWeightPlcService _service;

    public KanbanController(ITotalWeightPlcService service) => _service = service;

    [HttpGet("kanban/{barcode}")]
    public ActionResult<KanbanDetailDto> GetKanban(string barcode) => Ok(_service.GetKanban(barcode));

    [HttpGet("kanbans")]
    public ActionResult<List<KanbanSummaryDto>> GetPendingKanbans() => Ok(_service.GetPendingKanbans());

    [HttpGet("totalweight/{kbTogetherId:int}/exists")]
    public ActionResult<bool> Exists(int kbTogetherId) => Ok(_service.TotalWeightExists(kbTogetherId));

    [HttpPost("totalweight")]
    public ActionResult<SaveTotalWeightResultDto> SaveTotalWeight([FromBody] SaveTotalWeightRequestDto request) =>
        Ok(_service.SaveTotalWeight(request));

    [HttpPost("totalweight/accept")]
    public IActionResult Accept([FromBody] AcceptStepRequestDto request)
    {
        _service.Accept(request);
        return NoContent();
    }
}
