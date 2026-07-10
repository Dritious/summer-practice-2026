namespace task14;
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
        if (threadsnumber <= 0)
            throw new ArgumentException("Число потоков должно быть положительно");

        if (step <= 0)
            throw new ArgumentException("Шаг должен быть положителен");

        if (a >= b)
            throw new ArgumentException("Левая граница должна быть больше правой");

        double totalSum = 0.0;

        Barrier barrier = new Barrier(threadsnumber, (b) => { });

        // создаем и запускаем потоки
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

                    double localSum = 0.0;

                    int stepsCount = (int)Math.Ceiling((rightBorder - leftBorder) / step);
                    double actualStep = (rightBorder - leftBorder) / stepsCount;

                    for (int j = 0; j < stepsCount; j++)
                    {
                        double x1 = leftBorder + j * actualStep;
                        double x2 = leftBorder + (j + 1) * actualStep;

                        double y1 = function(x1);
                        double y2 = function(x2);

                        localSum += (y1 + y2) / 2.0 * actualStep;
                    }

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