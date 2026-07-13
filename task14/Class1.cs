namespace task14;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

//
// Вычисление определенного интеграла
//
public class DefiniteIntegral
{
    //
    // a, b - границы отрезка, на котором происходит вычисление опредленного интеграла
    // function - функция, для которой вычисляется определнный интеграл
    // step - размер одного шага разбиения
    // threadsNumber - число потоков, которые используются для вычислений
    //

    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentThread();

    [DllImport("kernel32.dll")]
    static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

    // здесь и далее убираем оптимизацию чтобы программа не пропускала вычисления
    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        if (threadsnumber <= 0 || step <= 0 || a >= b)
            throw new ArgumentException();

        if (threadsnumber == 1)
            return ComputeSegment(a, b, function, step);

        double totalSum = 0.0;
        int processorCount = Environment.ProcessorCount;
        double segmentLength = (b - a) / threadsnumber;

        using var barrier = new Barrier(threadsnumber);

        int completedCount = 0;

        for (int i = 0; i < threadsnumber; i++)
        {
            int idx = i;
            double left = a + idx * segmentLength;
            double right = a + (idx + 1) * segmentLength;

            // через пул более оптимизированно
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // явно прописываем переключения ядер чтобы система не перерасприделяла потоки криво
                    if (idx < processorCount)
                        SetThreadAffinityMask(GetCurrentThread(), (IntPtr)(1 << idx));

                    double localSum = ComputeSegment(left, right, function, step);

                    // атомарно складываем
                    InterlockedAdd(ref totalSum, localSum);
                }
                finally
                {
                    barrier.SignalAndWait();

                    Interlocked.Increment(ref completedCount);
                }
            });
        }

        while (Volatile.Read(ref completedCount) < threadsnumber)
        {
            Thread.Yield();
        }

        return totalSum;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    static double ComputeSegment(double a, double b, Func<double, double> function, double step)
    {
        int stepsCount = (int)((b - a) / step + 0.5);
        if (stepsCount == 0) stepsCount = 1;

        double actualStep = (b - a) / stepsCount;
        double sum = 0.0;

        for (int i = 0; i < stepsCount; i++)
        {
            double x1 = a + i * actualStep;
            double x2 = a + (i + 1) * actualStep;
            sum += (function(x1) + function(x2)) / 2.0 * actualStep;
        }

        return sum;
    }

    // вспомогательный метод атомарного сложения
    private static void InterlockedAdd(ref double location1, double value)
    {
        double newCurrentValue;
        double currentValue;

        do
        {
            currentValue = location1;
            newCurrentValue = currentValue + value;
        }
        while (Interlocked.CompareExchange(ref location1, newCurrentValue, currentValue) != currentValue);
    }
}