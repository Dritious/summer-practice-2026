using System.Reflection;

class Program
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
        var pluginsDir = Path.Combine(solutionDir, "plugins");
        var dllPath = Path.Combine(solutionDir, "plugins", "FileSystemCommands.dll");

        if (!Directory.Exists(pluginsDir))
        {
            throw new DirectoryNotFoundException($"Plugins directory not found at: {pluginsDir}");
        }

        // подгружаем все плагины(в том числе интерфейс комманды)
        foreach (var dll in Directory.GetFiles(pluginsDir, "*.dll"))
        {
            try
            {
                Assembly.LoadFrom(dll);
            }
            catch (Exception ex) when (ex is FileLoadException)
            {
                throw new InvalidOperationException($"Failed to load plugin: {dll}", ex);
            }
        }

        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException($"Plugin not found: {dllPath}");
        }

        try
        {
            return Assembly.LoadFrom(dllPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error loading target DLL: {dllPath}", ex);
        }
    }

    // взял пример из тестов
    public static void Main(string[] args)
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var assembly = LoadPlugins();
        var dirSizeType = assembly.GetType("FileSystemCommands.DirectorySizeCommand")
            ?? throw new TypeLoadException("Type 'FileSystemCommands.DirectorySizeCommand' not found."); 
        dynamic dirSizeCommand = Activator.CreateInstance(dirSizeType, testDir);
        dirSizeCommand.Execute();

        var findFilesType = assembly.GetType("FileSystemCommands.FindFilesCommand")
            ?? throw new TypeLoadException("Type FileSystemCommands.FindFilesCommand' not found."); 
        dynamic findFilesCommand = Activator.CreateInstance(findFilesType, testDir, "*.txt");
        findFilesCommand.Execute();
    }
}