using System.Text;
using System.Text.Json;

namespace DigiDocumentConverter.Core;

public class JsonConverter : IConverter
{
    public TargetFormat Format => TargetFormat.Json;

    public ConversionResult Convert(ConversionInput input)
    {
        var payload = new
        {
            filename = input.Filename,
            sourceType = input.SourceType,
            text = input.ExtractedText,
            metadata = input.Metadata ?? new Dictionary<string, string>(),
            convertedAt = DateTime.UtcNow,
        };
        var json = JsonSerializer.SerializeToUtf8Bytes(
            payload, new JsonSerializerOptions { WriteIndented = true });
        return new ConversionResult(
            Path.ChangeExtension(input.Filename, ".json"), "application/json", json);
    }
}
