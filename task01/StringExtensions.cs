namespace task01;

public static class StringExtensions
{
    static bool IsPalindrome(this string input)
    {
        input = input.ToLower();
        string cleanInput = new string(input.Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c)).ToArray());
        if (cleanInput.Length > 0 && cleanInput == cleanInput.Reverse()) { return true; }
        else { return false; }
    }
}
