using System.Reflection;

namespace task08tests;


public class FileSystemCommandsTests
{
    // вспомогательная функция на поиск корня решения
    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (directory != null)
        {
            if (directory.GetFiles("*.sln").Any())
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        throw new Exception("There is no solution file(.sln)");
    }

    private static Assembly LoadPlugins()
    {
        var solutionDir = FindSolutionRoot();
        var dllPath = Path.Combine(solutionDir, "plugins", "FileSystemCommands.dll");
        return Assembly.LoadFrom(dllPath);
    }

    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    { 
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var assembly = LoadPlugins();
        var type = assembly.GetType("DirectorySizeCommand");
        dynamic command = Activator.CreateInstance(type, testDir);

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

        var assembly = LoadPlugins();
        var type = assembly.GetType("FindFilesCommand");
        dynamic command = Activator.CreateInstance(type, testDir, "*.txt");

        var sw = new StringWriter();
        Console.SetOut(sw);
        command.Execute();
        var output = sw.ToString();
        Console.SetOut(Console.Out);

        var lines = output.Split("\n"); 
        Assert.Single(lines);
        Assert.Contains("file1.txt", lines[0]);

        Directory.Delete(testDir, true);
    }
}