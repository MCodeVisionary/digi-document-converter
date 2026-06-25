using DigiDocumentConverter.Core;
using Microsoft.AspNetCore.Mvc;

namespace DigiDocumentConverter.Api.Controllers;

public record ConvertRequest(string Filename, string SourceType, string Text, string Target);

[ApiController]
[Route("api/convert")]
public class ConvertController : ControllerBase
{
    private readonly IConversionFactory _factory;
    private readonly ILogger<ConvertController> _logger;

    public ConvertController(IConversionFactory factory, ILogger<ConvertController> logger)
    {
        _factory = factory;
        _logger  = logger;
    }

    [HttpPost]
    public IActionResult Convert([FromBody] ConvertRequest req)
    {
        _logger.LogInformation("Convert request: filename={Filename} sourceType={SourceType} target={Target}",
            req.Filename, req.SourceType, req.Target);

        if (!Enum.TryParse<TargetFormat>(req.Target, ignoreCase: true, out var target))
        {
            _logger.LogWarning("Invalid target format {Target} for {Filename}", req.Target, req.Filename);
            return UnprocessableEntity($"target must be one of: pdf, word, json");
        }

        try
        {
            var input  = new ConversionInput(req.Filename, req.SourceType, req.Text ?? "");
            var result = _factory.Convert(input, target);
            _logger.LogInformation("Conversion succeeded: {OutputFilename} ({Bytes} bytes)",
                result.Filename, result.Content.Length);
            return File(result.Content, result.ContentType, result.Filename);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex, "Unsupported conversion requested: {Target}", req.Target);
            return UnprocessableEntity(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Conversion failed for {Filename} → {Target}", req.Filename, req.Target);
            return StatusCode(500, "Conversion failed");
        }
    }

    [HttpGet("/health")]
    public IActionResult Health()
    {
        _logger.LogDebug("Health check called");
        return Ok(new { status = "ok", service = "digi-document-converter" });
    }
}
