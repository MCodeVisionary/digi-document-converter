namespace DigiDocumentConverter.Core;

public interface IConverter
{
    TargetFormat Format { get; }
    ConversionResult Convert(ConversionInput input);
}

public interface IConversionFactory
{
    IConverter For(TargetFormat format);
    ConversionResult Convert(ConversionInput input, TargetFormat format);
}
