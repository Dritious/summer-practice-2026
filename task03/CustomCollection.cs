namespace task03;

using System.Collections;

public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Шаг 5
    public IEnumerable<T> GetReverseEnumerator()
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    // Шаг 6
    public static IEnumerable<int> GenerateSequence(int start, int count)
    {
        for (int i = start; i < count; i++)
        {
            yield return i;
        }
    }

    // Шаг 7
    public IEnumerable<T> FilterAndSort(Func<T, bool> predicate, Func<T, IComparable> keySelector)
    {
        return _items.Where(predicate).OrderBy(keySelector);
    }
}