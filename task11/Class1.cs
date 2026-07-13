using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

public interface ICalculator
{
    int Add(int a, int b);
    int Minus(int a, int b);
    int Mul(int a, int b);
    int Div(int a, int b);
}

public static class DynamicClassCompiler
{
    private const string ClassDefinition = @"
public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    public static ICalculator CreateCalculator()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(ClassDefinition);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
        };

        // чтобы при параллельном запуске(например тестов) ошибок не возникало
        string uniqueId = Guid.NewGuid().ToString();
        string assemblyName = $"MyAssembly_{uniqueId}";
        string path = $"calculator_{uniqueId}.dll";

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var result = compilation.Emit(path);

        if (!result.Success)
        {
            var failures = result.Diagnostics
                .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error) // берем критические ошибки
                .Select(diagnostic => $"{diagnostic.Id}: {diagnostic.GetMessage()}");

            var errorLog = string.Join(Environment.NewLine, failures);

            throw new InvalidOperationException($"Compilation error:{Environment.NewLine}{errorLog}");
        }

        var assembly = Assembly.LoadFrom(path);
        var type = assembly.GetType("Calculator")
            ?? throw new TypeLoadException("Type 'Calculator' not found.");

        var instance = Activator.CreateInstance(type)
            ?? throw new InvalidOperationException("Cannot create Calculator instance.");

        return (ICalculator)instance;
    }
}