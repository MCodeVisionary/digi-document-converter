using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigiDocumentConverter.Core;

public class ConversionFactory : IConversionFactory
{
    private readonly Dictionary<TargetFormat, IConverter> _converters;
    private readonly ILogger<ConversionFactory> _logger;

    public ConversionFactory(ILogger<ConversionFactory>? logger = null)
    {
        _logger = logger ?? NullLogger<ConversionFactory>.Instance;
        _converters = new IConverter[] { new PdfConverter(), new WordConverter(), new JsonConverter() }
            .ToDictionary(c => c.Format);
        _logger.LogDebug("ConversionFactory initialised with formats: {Formats}",
            string.Join(", ", _converters.Keys));
    }

    public IConverter For(TargetFormat format)
    {
        if (_converters.TryGetValue(format, out var c))
            return c;
        throw new NotSupportedException($"No converter registered for format '{format}'.");
    }

    public ConversionResult Convert(ConversionInput input, TargetFormat format)
    {
        _logger.LogDebug("Converting {Filename} to {Format}", input.Filename, format);
        try
        {
            var result = For(format).Convert(input);
            _logger.LogDebug("Converted {Filename} → {Output} ({Bytes} bytes)",
                input.Filename, result.Filename, result.Content.Length);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Converter for {Format} threw while processing {Filename}",
                format, input.Filename);
            throw;
        }
    }
}
