using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DigiDocumentConverter.Core;

public class PdfConverter : IConverter
{
    static PdfConverter() => QuestPDF.Settings.License = LicenseType.Community;

    public TargetFormat Format => TargetFormat.Pdf;

    public ConversionResult Convert(ConversionInput input)
    {
        var bytes = Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.Header().Text(input.Filename).Bold().FontSize(16);
                page.Content().PaddingVertical(10).Text(input.ExtractedText);
                page.Footer().AlignRight().Text($"Source: {input.SourceType}");
            });
        }).GeneratePdf();

        return new ConversionResult(
            Path.ChangeExtension(input.Filename, ".pdf"), "application/pdf", bytes);
    }
}
