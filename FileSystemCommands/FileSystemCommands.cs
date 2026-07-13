namespace FileSystemCommands;

using CommandLib;
using task07;

[DisplayName("Команда подсчета размера директории")]
[Version(1, 0)]
public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;

    public DirectorySizeCommand(string directoryPath)
    {
        _directoryPath = directoryPath;
    }

    [DisplayName("Выполнить подсчет размера")]
    public void Execute()
    {
        long dirSize = CalculateSize(_directoryPath);
        Console.WriteLine($"Size of directory: {dirSize} bytes");
    }

    private long CalculateSize(string path)
    {
        long totalSize = 0;

        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            totalSize += fileInfo.Length;
        }
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            totalSize += CalculateSize(directory);
        }

        return totalSize;
    }
}

[DisplayName("Команда поиска файлов")]
[Version(1, 1)]
public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _pattern;

    public FindFilesCommand(string directoryPath, string pattern)
    {
        _directoryPath = directoryPath;
        _pattern = pattern;
    }

    [DisplayName("Выполнить поиск файлов")]
    public void Execute()
    {
        var files = Directory.GetFiles(_directoryPath, _pattern, SearchOption.AllDirectories);

        foreach (var file in files)
        {
            Console.WriteLine($"Found: {file}");
        }

    }

}