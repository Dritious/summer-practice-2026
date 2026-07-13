using System.Reflection;
class Program
{
    public static void Main(string[] args)
    {

        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            throw new ArgumentException("Args must contain ddl path.");
        }

        string dllPath = args[0];

        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException($"File not exist: {dllPath}");
        }

        string? parentDirectory = Path.GetDirectoryName(dllPath);

        if (string.IsNullOrEmpty(parentDirectory) || !Directory.Exists(parentDirectory))
        {
            throw new DirectoryNotFoundException($"Директория плагина не найдена или неверна.");
        }

        try
        {
            foreach (var dll in Directory.GetFiles(parentDirectory, "*.dll"))
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
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error while search plugins", ex);
        }

        // Загрузка целевой сборки
        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFrom(dllPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"File load error: {dllPath}", ex);
        }


        foreach (Type type in assembly.GetTypes().Where(t => t.IsClass))
        {
            Console.WriteLine($"Class:{type.Name}");

            // Атрибуты класса
            foreach (Attribute attr in type.GetCustomAttributes())
                Console.WriteLine($"Attribute:{attr}");

            // Конструкторы
            foreach (ConstructorInfo ctor in type.GetConstructors())
            {
                Console.WriteLine($"Constructor: {ctor.Name}");
                foreach (ParameterInfo param in ctor.GetParameters())
                    Console.WriteLine($"Parameter:{param.Name}:{param.ParameterType.Name}");
            }

            // Свойства
            foreach (PropertyInfo prop in type.GetProperties())
            {
                Console.WriteLine($"Property: {prop.Name}:{prop.PropertyType.Name}");
                foreach (Attribute attr in prop.GetCustomAttributes())
                    Console.WriteLine($"Attribute:{attr}");
            }

            // Методы
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Console.WriteLine($"Method:{method.Name}:{method.ReturnType.Name}");
                foreach (ParameterInfo param in method.GetParameters())
                    Console.WriteLine($"Parameter:{param.Name}:{param.ParameterType.Name}");
                foreach (Attribute attr in method.GetCustomAttributes())
                    Console.WriteLine($"Attribute:{attr}");
            }
        }
    }
}