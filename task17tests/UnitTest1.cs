namespace task17tests;

using task17;
using Xunit;

public class ServerThreadTests
{
    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    private class ThrowingCommand : ICommand
    {
        public void Execute() => throw new ArgumentException("Something went wrong!");
    }

    [Fact]
    public void DefaultExceptionHandler_PrependCommandNameAndRethrows()
    {
        var server = new ServerThread(); 
        var command = new ThrowingCommand();

        // проверяем, что обработчик перевыбрасывает ошибку с нужным префиксом
        var exception = Assert.Throws<Exception>(() => server.ExecuteCommand(command));

        Assert.StartsWith("[ThrowingCommand]", exception.Message);
        Assert.Contains("Something went wrong!", exception.Message);
        Assert.IsType<ArgumentException>(exception.InnerException);
    }

    [Fact]
    public void HardStopCommand_StopsImmediately()
    {
        var server = new ServerThread();
        bool remainingCommandExecuted = false;

        server.Enqueue(new HardStopCommand(server));
        server.Enqueue(new ActionCommand(() => remainingCommandExecuted = true));

        server.Start();
        server.Join(1000); // время на выполнение

        Assert.False(remainingCommandExecuted);
    }

    [Fact]
    public void SoftStopCommand_ProcessesRemainingQueue()
    {
        var server = new ServerThread();
        bool remainingCommandExecuted = false;

        server.Enqueue(new SoftStopCommand(server));
        server.Enqueue(new ActionCommand(() => remainingCommandExecuted = true));

        server.Start();
        server.Join(1000);

        Assert.True(remainingCommandExecuted);
    }

    [Fact]
    public void Enqueue_AfterSoftStop_ThrowsInvalidOperationException()
    {
        var server = new ServerThread();
        server.Start();

        server.Enqueue(new SoftStopCommand(server));
        server.Join(1000);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            server.Enqueue(new ActionCommand(() => { }))
        );
        Assert.Contains("[SoftStop in progress]", exception.Message);
    }

    [Fact]
    public void StopCommands_InWrongThread_ThrowException()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        var softStop = new SoftStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }
}