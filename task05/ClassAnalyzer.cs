namespace task05;

using System.Reflection;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type ?? throw new ArgumentNullException(nameof(type)); ;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods()
                    .Where(m => m.IsPublic)
                    .Select(m => m.Name);
    }

    public IEnumerable<string> GetMethodParams(string methodName)
    {
        var method = _type.GetMethods()
                         .FirstOrDefault(m => m.IsPublic && m.Name == methodName);

        if (method == null)
            return Enumerable.Empty<string>();

        var parameters = method.GetParameters()
                              .Select(p => p.Name);

        if (method.ReturnType != typeof(void))
            parameters = parameters.Append(method.ReturnType.Name);

        return parameters;
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Select(f => f.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.IsDefined(typeof(T), true);
    }
}