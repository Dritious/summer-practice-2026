using ScottPlot;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using task17;

var stopwatch = new Stopwatch();
var history = new ConcurrentBag<(long time, int id, int val)>();
var scheduler = new DefualtScheduler();
var server = new ServerThread(scheduler);

stopwatch.Start();

for (int i = 1; i <= 3; i++)
    server.Enqueue(new TestCommand(i, scheduler, history, stopwatch));

server.Start();
server.Join(100);
server.HardStop();

var plt = new Plot();

foreach (var group in history.GroupBy(x => x.id).OrderBy(g => g.Key))
{
    double[] times = group.Select(x => (double)x.time).ToArray();
    double[] counters = group.Select(x => (double)x.val).ToArray();

    var scatter = plt.Add.Scatter(times, counters);
    scatter.LegendText = $"Команда {group.Key}";

    scatter.MarkerSize = 7;
}

plt.XLabel("Время выполнения (мс)");
plt.YLabel("Значение счетчика");
plt.ShowLegend();
plt.SavePng("commands.png", 400, 300);

public class TestCommand(int id, IScheduler scheduler,
    ConcurrentBag<(long time, int id, int val)> history,
    Stopwatch stopwatch) : ICommand
{
    int counter = 0;
    public void Execute()
    {
        Console.WriteLine($"Поток {id} вызов {++counter}");
        scheduler.Add(this);
        history.Add((stopwatch.ElapsedMilliseconds, id, counter));
    }
}