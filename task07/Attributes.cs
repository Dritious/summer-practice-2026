namespace task07;

using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}


[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod()
    {
    }
}


public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        // DisplayName 
        var displayAttr = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayAttr != null)
        {
            Console.WriteLine($"Class DisplayName: {displayAttr.DisplayName}");
        }

        // Version
        var versionAttr = type.GetCustomAttribute<VersionAttribute>();
        if (versionAttr != null)
        {
            Console.WriteLine($"Version: {versionAttr.Major}.{versionAttr.Minor}");
        }

        Console.WriteLine("Methods:");
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var attr = method.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                Console.WriteLine($"{method.Name}: {attr.DisplayName}");
            }
            else
            {
                Console.WriteLine($"{method.Name}");
            }
        }

        Console.WriteLine("Properties:");
        foreach (var prop in type.GetProperties())
        {
            var attr = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                Console.WriteLine($"{prop.Name}: {attr.DisplayName}");
            }
            else
            {
                Console.WriteLine($"{prop.Name}");
            }
        }
    }
}