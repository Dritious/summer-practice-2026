using ScottPlot;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using task14;

class PerformanceResearch
{
    static int measurments = 5;
    static double a = -100, b = 100;
    static readonly Func<double, double> func = Math.Sin;

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    static void Main()
    {
        // Определение оптимального шага
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        int stepIndex = 0;

        for (int i = 0; i < steps.Length; i++)
        {
            double time = MeasureTime(steps[i], 4);
            double error = Math.Abs(DefiniteIntegral.Solve(a, b, func, steps[i], 1));

            if (error < 1e-4 && (stepIndex == 0 || time < MeasureTime(steps[stepIndex], 4)))
                stepIndex = i;
        }

        if (stepIndex == 0)
        {
            throw new Exception("МЫ ВСЕ ПРОЕБАЛИ");
        }

        double optimalStep = steps[stepIndex];


        // Бинарный поиск оптимального числа потоков
        var results = new Dictionary<int, double>();

        Console.WriteLine("Бинарный поиск оптимального числа потоков:");
        Console.WriteLine("Потоки | Время (мс)");
        Console.WriteLine("-------|-----------");

        // Замеряем для 1 потока
        results[1] = MeasureTime(optimalStep, 1);
        Console.WriteLine($"{1,6} | {results[1],9:F12}");

        // Бинарный поиск
        for (int i=2; i<=16;i++)
            {
                results[i] = MeasureTime(optimalStep, i);
                Console.WriteLine($"{i,6} | {results[i],9:F1}");
            }

        // Результаты
        var best = results.OrderBy(x => x.Value).First();
        double single = results[1];
        double multi = best.Value;
        double gain = (1 - multi / single) * 100;

        // Сохранение и график
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

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
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