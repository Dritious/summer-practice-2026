namespace CommandLib;

public interface IPlugin
{
    string Name { get; }
    void Execute();
}
