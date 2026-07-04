using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

public static class DynamicClassCompiler
{
    private const string ClassDefinition = @"
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    public static dynamic CreateCalculator()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(ClassDefinition);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            "MyAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var path = "calculator.dll";
        var result = compilation.Emit(path);
        var assembly = Assembly.LoadFrom(path);
        var type = assembly.GetType("Calculator");

        return Activator.CreateInstance(type);
    }
}