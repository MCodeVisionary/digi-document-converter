using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DigiDocumentConverter.Core;

public class WordConverter : IConverter
{
    public TargetFormat Format => TargetFormat.Word;

    public ConversionResult Convert(ConversionInput input)
    {
        using var ms = new MemoryStream();
        using (var word = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
        {
            var main = word.AddMainDocumentPart();
            main.Document = new Document();
            var body = main.Document.AppendChild(new Body());

            body.AppendChild(new Paragraph(new Run(
                new RunProperties(new Bold()),
                new Text(input.Filename))));

            foreach (var line in input.ExtractedText.Split('\n'))
                body.AppendChild(new Paragraph(new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve })));

            main.Document.Save();
        }
        return new ConversionResult(
            Path.ChangeExtension(input.Filename, ".docx"),
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ms.ToArray());
    }
}
