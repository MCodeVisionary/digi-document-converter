namespace DigiDocumentConverter.Core;

public class ConversionFactory : IConversionFactory
{
    private readonly Dictionary<TargetFormat, IConverter> _converters;

    public ConversionFactory()
    {
        _converters = new IConverter[] { new PdfConverter(), new WordConverter(), new JsonConverter() }
            .ToDictionary(c => c.Format);
    }

    public IConverter For(TargetFormat format) =>
        _converters.TryGetValue(format, out var c)
            ? c : throw new NotSupportedException($"No converter for {format}");

    public ConversionResult Convert(ConversionInput input, TargetFormat format) =>
        For(format).Convert(input);
}
