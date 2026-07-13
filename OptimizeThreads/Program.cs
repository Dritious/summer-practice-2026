using ScottPlot;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using task14;

class PerformanceResearch
{
    static int measurments = 5;
    static double a = -100, b = 100;
    static readonly Func<double, double> func = Math.Sin;

    static void Main()
    {
        // определение оптимального шага
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        int stepIndex = 0;
        int bestTime = int.MaxValue;

        for (int i = 0; i < steps.Length; i++)
        {
            double time = MeasureTime(steps[i], 4);
            // у вашего покороного слуги 4 ядра поэтому проверяем на 4 потоках
            double error = Math.Abs(DefiniteIntegral.Solve(a, b, func, steps[i], 4));

            if (error < 1e-4 && (stepIndex == 0 || time < bestTime))
                stepIndex = i;
                
        }

        double optimalStep = steps[stepIndex];

        var results = new Dictionary<int, double>();

        // замеряем для 1 потока
        results[0] = MeasureTime(optimalStep, 1);
        Console.WriteLine($"{1,6} | {results[0],9:F1}");
        // для нескольких
        for (int i=2; i<=16;i++)
            {
                results[i-1] = MeasureTime(optimalStep, i);
                Console.WriteLine($"{i,6} | {results[i-1],9:F1}");
            }

        // результаты
        var best = results.OrderBy(x => x.Value).First();
        double single = results[0];
        double multi = best.Value;
        double gain = (1 - multi / single) * 100;

        // сохранение и график
        File.WriteAllText("results.txt",
            $"Шаг: {optimalStep:E1}\n" +
            $"Потоков: {best.Key}\n" +
            $"Время: {best.Value:F1} мс\n" +
            $"Ускорение: {single / multi:F2}x\n" +
            $"Улучшение: {gain:F1}%\n" +
            $"Однопоточная: {single:F1} мс"
        );

        DrawChart(results, best);
    }

    static double MeasureTime(double step, int threads)
    {
        var times = new List<long>();

        for (int i = 0; i < measurments; i++)
        {
            var sw = Stopwatch.StartNew();
            DefiniteIntegral.Solve(a, b, func, step, threads);

            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }

        return times.Average();
    }

    static void DrawChart(Dictionary<int, double> data, KeyValuePair<int, double> best)
    {
        var plot = new Plot();

        var x = data.Keys.Select(k => (double)k).ToArray();
        var y = data.Values.ToArray();

        var scatter = plot.Add.Scatter(x, y);
        scatter.LineWidth = 2;
        scatter.MarkerSize = 10;
        scatter.Color = Colors.Blue;

        plot.Title("Время вычисления от количества потоков");
        plot.XLabel("Потоки");
        plot.YLabel("Время (мс)");
        plot.ShowGrid();

        plot.SavePng("graph.png", 800, 600);

    }
}