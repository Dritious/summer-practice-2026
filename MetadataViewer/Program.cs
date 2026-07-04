using System.Reflection;

Console.WriteLine("Введите путь к плагину: ");
string dllPath = Console.ReadLine();
string parentDirectory = Path.GetDirectoryName(dllPath);
foreach (var dll in Directory.GetFiles(parentDirectory, "*.dll"))
{
    Assembly.LoadFrom(dll);
}
Assembly assembly = Assembly.LoadFrom(dllPath);

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