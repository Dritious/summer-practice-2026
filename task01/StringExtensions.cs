namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        input = input.ToLower();
        var cleanInput = input.Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c)).ToArray();

        if (cleanInput.Length == 0)
            return false;

        return cleanInput.SequenceEqual(cleanInput.Reverse());
    }
}