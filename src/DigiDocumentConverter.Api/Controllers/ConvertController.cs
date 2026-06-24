using DigiDocumentConverter.Core;
using Microsoft.AspNetCore.Mvc;

namespace DigiDocumentConverter.Api.Controllers;

public record ConvertRequest(string Filename, string SourceType, string Text, string Target);

[ApiController]
[Route("api/convert")]
public class ConvertController : ControllerBase
{
    private readonly IConversionFactory _factory;
    public ConvertController(IConversionFactory factory) => _factory = factory;

    [HttpPost]
    public IActionResult Convert([FromBody] ConvertRequest req)
    {
        if (!Enum.TryParse<TargetFormat>(req.Target, ignoreCase: true, out var target))
            return UnprocessableEntity($"target must be one of: pdf, word, json");

        var input = new ConversionInput(req.Filename, req.SourceType, req.Text ?? "");
        var result = _factory.Convert(input, target);
        return File(result.Content, result.ContentType, result.Filename);
    }

    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "ok", service = "digi-document-converter" });
}
