using System.Text;

var path = GetCurrentExecutablePath();
PrintStartupInfo(path);
var files = Directory.EnumerateFiles(path, "*", new EnumerationOptions() { RecurseSubdirectories = true });
List<ScanFileInfo> countFileAndLinesResult = CountFilesAndLines(files);
var fileTypeGroups = countFileAndLinesResult
    .GroupBy(x => x.FileExtension)
    .OrderByDescending(x => x.Count());
PrintScanFileInfoMarkdownTable(fileTypeGroups);
Console.ReadLine();

static void PrintStartupInfo(string path)
{
    Console.WriteLine($"Counting Files in path:");
    Console.WriteLine($"\t {path}");
}

static string GetCurrentExecutablePath()
{
    var processPath = Environment.ProcessPath;
    if (processPath == null)
    {
        throw new Exception("Unexpected Error, the process path should not be null here.");
    }
    return Directory.GetParent(processPath)!.ToString();
}

static List<ScanFileInfo> CountFilesAndLines(IEnumerable<string> files)
{
    List<ScanFileInfo> result = new();
    foreach (var file in files)
    {
        var lineCount = File.ReadLines(file).Count();
        FileInfo fileInfo = new(file);
        ScanFileInfo scanFileInfo = new() { FileExtension = fileInfo.Extension.ToLowerInvariant(), LineCount = lineCount, SizeInBytes = fileInfo.Length };
        result.Add(scanFileInfo);
    }
    return result;
}

static void PrintScanFileInfoMarkdownTable(IOrderedEnumerable<IGrouping<string?, ScanFileInfo>> fileTypeGroups)
{
    Console.WriteLine($"|Extension   |Line Count  |Size        |");
    Console.WriteLine($"|------------|------------|------------|");

    foreach (var fileType in fileTypeGroups)
    {
        const int columnWidth = 12;
        var key = fileType.Key is not null ? fileType.Key.ToString() : string.Empty;
        StringBuilder stringBuilder = new();
        stringBuilder.Append('|');
        stringBuilder.Append(key.PadRight(columnWidth, ' '));
        stringBuilder.Append('|');
        stringBuilder.Append(fileType.Count().ToString().PadRight(columnWidth, ' '));
        stringBuilder.Append('|');
        stringBuilder.Append(fileType.Sum(x => x.SizeInBytes).ToString().PadRight(columnWidth, ' '));
        stringBuilder.Append('|');
        Console.WriteLine(stringBuilder.ToString());
    }
}