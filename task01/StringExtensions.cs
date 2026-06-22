namespace task01;

public static class StringExtensions
{
    static bool IsPalindrome(this string s)
    {
        int l = 0; int r = s.Length - 1;
        if (r < 0) { return false; }
        while (l < r)
        {
            if (s[l] != s[r]) { return false; }
            do { l++; } while (Char.IsPunctuation(s[l]) || Char.IsWhiteSpace(s[l]));
            do { r--; } while (Char.IsPunctuation(s[r]) || Char.IsWhiteSpace(s[r]));
        }
        return true;
    }
}
