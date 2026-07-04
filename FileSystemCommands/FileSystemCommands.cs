namespace FileSystemCommands;

using CommandLib;
using System.Text.RegularExpressions;

public class DirectorySizeCommand: ICommand
{
    private readonly string _directoryPath;

    public DirectorySizeCommand(string directoryPath)
    { 
        _directoryPath = directoryPath; 
    }
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

public class FindFilesCommand: ICommand
{
    private readonly string _directoryPath;
    private readonly string _pattern;

    public FindFilesCommand(string directoryPath, string pattern)
    {
        _directoryPath = directoryPath;
        _pattern = pattern;
    }

    public void Execute()
    {
        var regex = new Regex(_pattern);
        int foundCount = FindFilesRecursive(_directoryPath, regex);
    }
    private int FindFilesRecursive(string path, Regex regex)
    {
        int count = 0;
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (regex.IsMatch(fileName))
            {
                Console.WriteLine($"Find: {file}");
                count++;
            }
        }

        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            count += FindFilesRecursive(directory, regex);

        }
   

        return count;
    }

}
