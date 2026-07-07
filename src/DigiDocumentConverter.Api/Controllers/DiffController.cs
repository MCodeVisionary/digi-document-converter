using DigiDocumentConverter.Core;
using Microsoft.AspNetCore.Mvc;

namespace DigiDocumentConverter.Api.Controllers;

public record DiffRequest(
    string FilenameV1,
    string FilenameV2,
    string TextV1,
    string TextV2);

[ApiController]
[Route("api/diff")]
public class DiffController : ControllerBase
{
    private readonly IDocumentDiffer _differ;
    private readonly ILogger<DiffController> _logger;

    public DiffController(IDocumentDiffer differ, ILogger<DiffController> logger)
    {
        _differ = differ;
        _logger  = logger;
    }

    [HttpPost]
    public IActionResult Diff([FromBody] DiffRequest req)
    {
        _logger.LogInformation("Diff request: {V1} vs {V2}", req.FilenameV1, req.FilenameV2);

        var input  = new DiffInput(req.FilenameV1, req.FilenameV2, req.TextV1 ?? "", req.TextV2 ?? "");
        var result = _differ.Diff(input);

        _logger.LogInformation("Diff complete: +{Ins} -{Del} lines",
            result.InsertedLines, result.DeletedLines);

        return Ok(result);
    }
}
