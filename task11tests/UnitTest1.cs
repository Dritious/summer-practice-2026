namespace task11tests;

public class CalculatorTests
{
    [Fact]
    public void CreateCalculator_ShouldReturnDynamicObject()
    {
        dynamic calculator = DynamicClassCompiler.CreateCalculator();
        Assert.NotNull(calculator);
    }

    [Fact]
    public void Add_ShouldReturnCorrectSum()
    {
        dynamic calculator = DynamicClassCompiler.CreateCalculator();
        int result = calculator.Add(5, 3);
        Assert.Equal(8, result);
    }

    [Fact]
    public void Minus_ShouldReturnCorrectDifference()
    {
        dynamic calculator = DynamicClassCompiler.CreateCalculator();
        int result = calculator.Minus(10, 3);
        Assert.Equal(7, result);
    }

    [Fact]
    public void Mul_ShouldReturnCorrectProduct()
    {
        dynamic calculator = DynamicClassCompiler.CreateCalculator();
        int result = calculator.Mul(5, 3);
        Assert.Equal(15, result);
    }
}