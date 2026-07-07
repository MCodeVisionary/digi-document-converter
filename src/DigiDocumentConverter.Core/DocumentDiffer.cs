using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace DigiDocumentConverter.Core;

public interface IDocumentDiffer
{
    DiffResult Diff(DiffInput input);
}

public class DocumentDiffer : IDocumentDiffer
{
    public DiffResult Diff(DiffInput input)
    {
        var model = InlineDiffBuilder.Diff(input.TextV1, input.TextV2);

        var lines = model.Lines
            .Select(l => new DiffLine(
                l.Type switch
                {
                    ChangeType.Inserted  => "inserted",
                    ChangeType.Deleted   => "deleted",
                    ChangeType.Modified  => "modified",
                    ChangeType.Imaginary => "imaginary",
                    _                    => "unchanged"
                },
                l.Text ?? "",
                l.Position))
            .ToList();

        return new DiffResult(
            input.FilenameV1,
            input.FilenameV2,
            lines.Count(l => l.Type is "inserted"),
            lines.Count(l => l.Type is "deleted"),
            lines.Count(l => l.Type is "unchanged"),
            lines);
    }
}
