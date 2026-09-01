using Microsoft.AspNetCore.Mvc;

namespace Innovation.Api.Controllers;

// T3 = everything cut from the demo slice (trays, manual mode, cancel,
// cleaning, HF-mixing - README §8.5). Every action returns 501 with a
// ProblemDetails body pointing back at the scope decision, instead of
// silently 404ing or being omitted (an omitted route is indistinguishable
// from a typo; an explicit 501 documents "deliberately not built").
public static class T3NotImplemented
{
    public static ObjectResult Result(HttpRequest request, string endpointName) => new(new ProblemDetails
    {
        Status = StatusCodes.Status501NotImplemented,
        Title = "Not Implemented in Portfolio Clone",
        Type = "https://errors.innovation-mes.local/not-implemented",
        Detail = $"{endpointName} is out of scope for the demo slice (T3). See README §8.5.",
        Instance = request.Path,
    })
    { StatusCode = StatusCodes.Status501NotImplemented };
}

[ApiController]
[Route("api/trays")]
public class TraysController : ControllerBase
{
    [HttpGet] public IActionResult GetAll() => T3NotImplemented.Result(Request, nameof(GetAll));
    [HttpPost] public IActionResult Create() => T3NotImplemented.Result(Request, nameof(Create));
}

[ApiController]
[Route("api/manual-mode")]
public class ManualModeController : ControllerBase
{
    [HttpPost("enable")] public IActionResult Enable() => T3NotImplemented.Result(Request, nameof(Enable));
    [HttpPost("cancel-kanban")] public IActionResult CancelKanban() => T3NotImplemented.Result(Request, nameof(CancelKanban));
}

[ApiController]
[Route("api/cleaning")]
public class CleaningController : ControllerBase
{
    [HttpPost("start")] public IActionResult Start() => T3NotImplemented.Result(Request, nameof(Start));
}
