namespace task10tests;

using task10;

public class PluginLoaderTests
{
    [Fact]
    public void LoadPlugins_ThrowsArgumentNullException_WhenPathIsNull()
    {
        var loader = new PluginLoader();

        Assert.Throws<ArgumentNullException>(() => loader.LoadPlugins(null!));
    }

    [Fact]
    public void LoadPlugins_ThrowsDirectoryNotFoundException_WhenPathDoesNotExist()
    {
        var loader = new PluginLoader();
        var fakePath = Path.Combine(Path.GetTempPath(), "hfdjfhskdfhdjshfdkjshfdjshfdskjhfdksj");

        Assert.Throws<DirectoryNotFoundException>(() => loader.LoadPlugins(fakePath));
    }
}