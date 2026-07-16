namespace task17;

using System.Collections.Concurrent;

public interface ICommand
{
    void Execute();
}
public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class DefualtScheduler : IScheduler
{
    private readonly Queue<ICommand> _queue = new Queue<ICommand>();
    public bool HasCommand() { if (_queue.Count > 0) return true; return false; }
    public ICommand Select() { return _queue.Dequeue(); }
    public void Add(ICommand command) { _queue.Enqueue(command); }
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
    private readonly IScheduler _scheduler;
    private readonly Thread _thread;
    private readonly Action<ICommand, Exception> _exceptionHandler;

    // делегат, исполняющий комманды 
    private Action? _behavior;

    public int ManagedThreadId => _thread.ManagedThreadId;

    public ServerThread(IScheduler? scheduler = null, Action<ICommand, Exception>? exceptionHandler = null)
    {
        // добавляет имя команды в начало сообщения, и пробрасывает дальше
        _exceptionHandler = exceptionHandler ?? ((cmd, ex) =>
        {
            throw new Exception($"[{cmd.GetType().Name}]: {ex.Message}", ex);
        });
        _behavior = DefaultBehavior;
        _thread = new Thread(Run);
        _scheduler = scheduler ?? new DefualtScheduler();
    }

    public void Start() => _thread.Start();

    public void Join(int millisecondsTimeout = Timeout.Infinite) => _thread.Join(millisecondsTimeout);

    public void Enqueue(ICommand cmd)
    {
        try
        {
            _queue.Add(cmd);
        }
        catch (InvalidOperationException ex)
        {
            // ловим системную ошибку закрытой коллекции 
            throw new InvalidOperationException(
                $"[SoftStop in progress] Cannot enqueue command '{cmd.GetType().Name}'. The queue is closed.",
                ex
            );
        }
    }

    private void Run()
    {
        while (_behavior != null)
        {
            _behavior();
        }
    }

    private void DefaultBehavior()
    {
        // если в scheduler есть задачи продолжает работу, если нет засыпает пока нет комманд
        int timeout = _scheduler.HasCommand() ? 0 : Timeout.Infinite;

        // round robin логика
        if (_queue.TryTake(out var cmd, timeout))
        {
            ExecuteCommand(cmd);
        }
        else if (_scheduler.HasCommand())
        {
            ExecuteCommand(_scheduler.Select());
        }
    }

    public void HardStop()
    {
        _behavior = null;
    }

    public void SoftStop()
    {
        // закрываем очередь при остановке
        _queue.CompleteAdding();
        _behavior = () =>
        {
            if (!_queue.IsCompleted)
            {
                if (_queue.TryTake(out var cmd))
                {
                    ExecuteCommand(cmd);
                }
            }
            else if (_scheduler.HasCommand())
            {
                ExecuteCommand(_scheduler.Select());
            }
            else
            {
                _behavior = null;
            }
        };
    }

    public void ExecuteCommand(ICommand cmd)
    {
        try
        {
            cmd.Execute();
        }
        catch (Exception ex)
        {
            _exceptionHandler(cmd, ex);
        }
    }
}


public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public HardStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Thread.CurrentThread.ManagedThreadId != _serverThread.ManagedThreadId)
        {
            throw new InvalidOperationException("Command must execute in server thread.");
        }
        _serverThread.HardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public SoftStopCommand(ServerThread serverThread) => _serverThread = serverThread;

    public void Execute()
    {
        if (Thread.CurrentThread.ManagedThreadId != _serverThread.ManagedThreadId)
        {
            throw new InvalidOperationException("Command must execute in server thread.");
        }
        _serverThread.SoftStop();
    }
}