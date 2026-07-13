using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{

    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);

        var output = new StringWriter();
        Console.SetOut(output);

        command.Execute(); // Проверяем, что не возникает исключений

        Assert.Contains("Size of directory:", output.ToString());

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");

        var sw = new StringWriter();
        Console.SetOut(sw);

        command.Execute();

        var output = sw.ToString();
        Console.SetOut(Console.Out);
        var lines = output.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
        Assert.Contains("file1.txt", lines[0]);

        Directory.Delete(testDir, true);
    }
}