namespace task10;

using PluginAttribute;
using System.Reflection;

public class PluginLoader
{
    private readonly Dictionary<string, Type> _plugins = new Dictionary<string, Type>();

    public void LoadPlugins(string directoryPath)
    {
        var dllFiles = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);

        foreach (var dllPath in dllFiles)
        {
            var assembly = Assembly.LoadFrom(dllPath);
            var pluginTypes = assembly.GetTypes()
                .Where(t => t.IsClass && t.GetCustomAttribute<PluginLoadAttribute>() != null);

            foreach (var pluginType in pluginTypes)
            {
                var attribute = pluginType.GetCustomAttribute<PluginLoadAttribute>();
                _plugins[pluginType.FullName] = pluginType;
            }
        }

        var loadOrder = new List<string>();
        var visited = new HashSet<string>();

        foreach (var pluginName in _plugins.Keys)
        {
            if (!visited.Contains(pluginName))
            {
                DFS(pluginName, visited, loadOrder, new HashSet<string>());
            }
        }

        foreach (var pluginName in loadOrder)
        {
            var pluginType = _plugins[pluginName];
            dynamic instance = Activator.CreateInstance(pluginType);
            instance.Execute();
        }
    }

    private void DFS(string pluginName, HashSet<string> visited, List<string> loadOrder, HashSet<string> recursionStack)
    {
        if (recursionStack.Contains(pluginName))
        {
            throw new InvalidOperationException($"Cycle detected: {pluginName}");
        }

        if (visited.Contains(pluginName))
        {
            return;
        }

        recursionStack.Add(pluginName);

        var pluginType = _plugins[pluginName];
        var attribute = pluginType.GetCustomAttribute<PluginLoadAttribute>();

        foreach (var dependency in attribute.Dependencies)
        {
            DFS(dependency, visited, loadOrder, recursionStack);
        }

        recursionStack.Remove(pluginName);
        visited.Add(pluginName);
        loadOrder.Add(pluginName);
    }
}
