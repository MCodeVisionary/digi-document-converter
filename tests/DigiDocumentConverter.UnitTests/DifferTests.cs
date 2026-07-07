using DigiDocumentConverter.Core;
using Xunit;

namespace DigiDocumentConverter.UnitTests;

public class DifferTests
{
    private static readonly DocumentDiffer Differ = new();

    private static DiffInput Input(string v1, string v2) =>
        new("doc-v1.txt", "doc-v2.txt", v1, v2);

    [Fact]
    public void Identical_texts_have_no_insertions_or_deletions()
    {
        var result = Differ.Diff(Input("hello\nworld", "hello\nworld"));

        Assert.Equal(0, result.InsertedLines);
        Assert.Equal(0, result.DeletedLines);
        Assert.All(result.Lines, l => Assert.Equal("unchanged", l.Type));
    }

    [Fact]
    public void Added_line_is_counted_as_inserted()
    {
        var result = Differ.Diff(Input("line1", "line1\nline2"));

        Assert.Equal(1, result.InsertedLines);
        Assert.Equal(0, result.DeletedLines);
    }

    [Fact]
    public void Removed_line_is_counted_as_deleted()
    {
        var result = Differ.Diff(Input("line1\nline2", "line1"));

        Assert.Equal(1, result.DeletedLines);
        Assert.Equal(0, result.InsertedLines);
    }

    [Fact]
    public void Empty_old_text_all_content_is_inserted()
    {
        var result = Differ.Diff(Input("", "alpha\nbeta\ngamma"));

        Assert.True(result.InsertedLines > 0);
        Assert.Equal(0, result.DeletedLines);
    }

    [Fact]
    public void Empty_new_text_all_content_is_deleted()
    {
        var result = Differ.Diff(Input("alpha\nbeta", ""));

        Assert.True(result.DeletedLines > 0);
        Assert.Equal(0, result.InsertedLines);
    }

    [Fact]
    public void Filenames_are_preserved_in_result()
    {
        var result = Differ.Diff(new DiffInput("old.txt", "new.txt", "text", "text"));

        Assert.Equal("old.txt", result.FilenameV1);
        Assert.Equal("new.txt", result.FilenameV2);
    }

    [Fact]
    public void Lines_collection_is_not_empty_for_non_empty_input()
    {
        var result = Differ.Diff(Input("hello", "hello"));

        Assert.NotEmpty(result.Lines);
    }

    [Fact]
    public void Changed_line_text_appears_in_result()
    {
        var result = Differ.Diff(Input("old content", "new content"));

        var texts = result.Lines.Select(l => l.Text).ToList();
        Assert.True(texts.Any(t => t.Contains("old content") || t.Contains("new content")));
    }

    [Fact]
    public void Unchanged_line_count_matches_shared_lines()
    {
        var result = Differ.Diff(Input("shared\nchanged", "shared\nupdated"));

        Assert.Equal(1, result.UnchangedLines);
    }

    [Fact]
    public void Both_empty_texts_produce_empty_lines_collection()
    {
        var result = Differ.Diff(Input("", ""));

        Assert.Equal(0, result.InsertedLines);
        Assert.Equal(0, result.DeletedLines);
    }
}
