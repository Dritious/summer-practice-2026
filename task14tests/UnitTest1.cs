namespace task14tests;

using task14;

public class DefiniteIntegralTests
{
    [Fact]
    public void TestIntegral()
    {
        var X = (double x) => x;
        var SIN = (double x) => Math.Sin(x);

        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);
        Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, X, 1e-6, 8), 1e-5); // в тесте из вики неправильное значение
    }
}