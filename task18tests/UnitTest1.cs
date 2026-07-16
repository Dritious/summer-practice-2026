namespace task18tests;

using task17;
using Xunit;

public class SchedulerTests
{
    private class LongRunningCommand : ICommand
    {
        private readonly IScheduler _scheduler;
        private readonly Action _stepAction;
        private int _remainingSteps;
        public LongRunningCommand(IScheduler scheduler, int steps, Action stepAction)
        {
            _scheduler = scheduler;
            _remainingSteps = steps;
            _stepAction = stepAction;
        }

        public void Execute()
        {
            _stepAction();
            _remainingSteps--;

            if (_remainingSteps > 0)
            {
                _scheduler.Add(this);
            }
        }
    }
    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action;
        public void Execute() => _action();
    }
    [Fact]
    public void LongRunningCommand_ShouldExecuteAllSteps()
    {
        var scheduler = new DefualtScheduler();
        var server = new ServerThread(scheduler);
        int executionCount = 0;

        var longTask = new LongRunningCommand(scheduler, 5, () => executionCount++);

        server.Enqueue(longTask);
        server.Enqueue(new SoftStopCommand(server));
        server.Start();
        server.Join(1000);

        Assert.Equal(5, executionCount);
    }

    [Fact]
    public void RoundRobinScheduler_ShouldInterleaveCommands()
    {
        var scheduler = new DefualtScheduler();
        var server = new ServerThread(scheduler);

        var executionOrder = new List<string>();

        var taskA = new LongRunningCommand(scheduler, 3, () => executionOrder.Add("A"));

        var taskB = new LongRunningCommand(scheduler, 3, () => executionOrder.Add("B"));

        server.Enqueue(taskA);
        server.Enqueue(taskB);
        server.Enqueue(new SoftStopCommand(server));

        server.Start();
        server.Join(1000);

        var expectedOrder = new List<string> { "A", "B", "A", "B", "A", "B" };
        Assert.Equal(expectedOrder, executionOrder);
    }
}
