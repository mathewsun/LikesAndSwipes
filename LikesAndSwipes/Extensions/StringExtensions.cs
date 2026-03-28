namespace LikesAndSwipes.Extensions;

public static class StringExtensions
{
    private static readonly Dictionary<char, string> CyrillicToLatinMap = new()
    {
        ['а'] = "a", ['б'] = "b", ['в'] = "v", ['г'] = "g", ['д'] = "d", ['е'] = "e", ['ё'] = "yo",
        ['ж'] = "zh", ['з'] = "z", ['и'] = "i", ['й'] = "y", ['к'] = "k", ['л'] = "l", ['м'] = "m",
        ['н'] = "n", ['о'] = "o", ['п'] = "p", ['р'] = "r", ['с'] = "s", ['т'] = "t", ['у'] = "u",
        ['ф'] = "f", ['х'] = "kh", ['ц'] = "ts", ['ч'] = "ch", ['ш'] = "sh", ['щ'] = "shch",
        ['ъ'] = "", ['ы'] = "y", ['ь'] = "", ['э'] = "e", ['ю'] = "yu", ['я'] = "ya"
    };

    public static string ConvertToLatin(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value ?? string.Empty;
        }

        var result = new System.Text.StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (!CyrillicToLatinMap.TryGetValue(char.ToLowerInvariant(character), out var latin))
            {
                result.Append(character);
                continue;
            }

            if (string.IsNullOrEmpty(latin))
            {
                continue;
            }

            if (char.IsUpper(character))
            {
                result.Append(char.ToUpperInvariant(latin[0]));
                if (latin.Length > 1)
                {
                    result.Append(latin[1..]);
                }
                continue;
            }

            result.Append(latin);
        }

        return result.ToString();
    }
}
