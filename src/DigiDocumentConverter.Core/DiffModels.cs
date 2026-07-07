namespace DigiDocumentConverter.Core;

public record DiffInput(
    string FilenameV1,
    string FilenameV2,
    string TextV1,
    string TextV2);

public record DiffLine(string Type, string Text, int? LineNumber);

public record DiffResult(
    string FilenameV1,
    string FilenameV2,
    int InsertedLines,
    int DeletedLines,
    int UnchangedLines,
    IReadOnlyList<DiffLine> Lines);
