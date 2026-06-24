using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DigiDocumentConverter.FunctionalTests;

public class ConvertApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public ConvertApiTests(WebApplicationFactory<Program> f) => _client = f.CreateClient();

    [Fact]
    public async Task Health_ok() =>
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/health")).StatusCode);

    [Theory]
    [InlineData("json", "application/json")]
    [InlineData("pdf", "application/pdf")]
    public async Task Convert_returns_requested_format(string target, string contentType)
    {
        var resp = await _client.PostAsJsonAsync("/api/convert", new
        {
            filename = "doc.txt", sourceType = "transcript", text = "content", target
        });
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.Equal(contentType, resp.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Convert_rejects_unknown_target()
    {
        var resp = await _client.PostAsJsonAsync("/api/convert", new
        {
            filename = "doc.txt", sourceType = "pdf", text = "x", target = "xml"
        });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, resp.StatusCode);
    }
}
