namespace DigiDocumentConverter.Core;

public enum TargetFormat { Pdf, Word, Json }

/// <summary>Normalized representation of an inputted document to be converted.</summary>
public record ConversionInput(
    string Filename,
    string SourceType,          // image | transcript | handwritten_note | pdf
    string ExtractedText,
    IReadOnlyDictionary<string, string>? Metadata = null);

public record ConversionResult(
    string Filename,
    string ContentType,
    byte[] Content);
