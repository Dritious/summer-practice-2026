namespace task10;

using PluginAttribute;
using System.Reflection;

public class PluginLoader
{
    private readonly Dictionary<string, Type> _plugins = new Dictionary<string, Type>();

    public void LoadPlugins(string directoryPath)
    {
        if (directoryPath == null) throw new ArgumentNullException(nameof(directoryPath));

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Plugin directory not found: {directoryPath}");
        }

        string[] dllFiles;
        try
        {
            dllFiles = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to read files.", ex);
        }

        foreach (var dllPath in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && t.GetCustomAttribute<PluginLoadAttribute>() != null);

                foreach (var pluginType in pluginTypes)
                {
                    _plugins[pluginType.FullName] = pluginType;
                }
            }
            catch (Exception ex) when (ex is FileLoadException)
            {
                throw new InvalidOperationException($"Error loading plugin: {dllPath}", ex);
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
            try
            {
                dynamic instance = Activator.CreateInstance(pluginType)
                    ?? throw new InvalidOperationException($"Could not create instance of {pluginName}");

                var method = pluginType.GetMethod("Execute");
                if (method == null)
                {
                    throw new TypeLoadException($"Plugin {pluginName} does not contain an 'Execute' method.");
                }

                method.Invoke(instance, null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error executing plugin: {pluginName}", ex);
            }
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
