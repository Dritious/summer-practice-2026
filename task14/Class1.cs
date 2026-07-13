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

        //int completedCount = 0;

        Thread[] threads = new Thread[threadsnumber];

        for (int i = 0; i < threadsnumber; i++)
        {
            int threadIndex = i;

            threads[i] = new Thread(() =>
            {
                try
                {
                    double segmentLength = (b - a) / threadsnumber;
                    double leftBorder = a + threadIndex * segmentLength;
                    double rightBorder = a + (threadIndex + 1) * segmentLength;

                    double localSum = ComputeSegment(leftBorder, rightBorder, function, step);

                    InterlockedAdd(ref totalSum, localSum);
                }
                finally
                {
                    barrier.SignalAndWait();
                }
            });

            threads[i].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        return totalSum;
    }

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