using System.Text;
using System.Text.Json;
using DigiDocumentConverter.Core;
using Xunit;

namespace DigiDocumentConverter.UnitTests;

public class ConverterTests
{
    private static ConversionInput Sample() =>
        new("notes.txt", "transcript", "Hello DocPortal\nSecond line.");

    [Fact]
    public void Json_conversion_produces_parseable_json_with_text()
    {
        var res = new ConversionFactory().Convert(Sample(), TargetFormat.Json);
        Assert.Equal("application/json", res.ContentType);
        using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(res.Content));
        Assert.Equal("transcript", doc.RootElement.GetProperty("sourceType").GetString());
        Assert.Contains("Hello DocPortal", doc.RootElement.GetProperty("text").GetString());
    }

    [Fact]
    public void Pdf_conversion_produces_pdf_magic_bytes()
    {
        var res = new ConversionFactory().Convert(Sample(), TargetFormat.Pdf);
        Assert.Equal("application/pdf", res.ContentType);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(res.Content, 0, 4));
    }

    [Fact]
    public void Word_conversion_produces_docx_zip_magic_bytes()
    {
        var res = new ConversionFactory().Convert(Sample(), TargetFormat.Word);
        Assert.EndsWith(".docx", res.Filename);
        Assert.Equal(0x50, res.Content[0]); // 'P' (zip/OOXML container)
        Assert.Equal(0x4B, res.Content[1]); // 'K'
    }

    [Fact]
    public void Unknown_format_throws()
    {
        var f = new ConversionFactory();
        Assert.Throws<NotSupportedException>(() => f.For((TargetFormat)99));
    }
}
