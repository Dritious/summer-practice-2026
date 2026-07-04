using Microsoft.CSharp;
using System.CodeDom.Compiler;

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
        using var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters();
        // подключение базовой библиотеки
        parameters.ReferencedAssemblies.Add("System.dll");

        var results = provider.CompileAssemblyFromSource(parameters, ClassDefinition);

        var type = results.CompiledAssembly.GetType("Calculator");
        return Activator.CreateInstance(type);
    }
}

