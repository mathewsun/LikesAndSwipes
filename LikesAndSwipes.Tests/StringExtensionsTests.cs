using LikesAndSwipes.Extensions;
using Xunit;

namespace LikesAndSwipes.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("Привет, мир!", "Privet, mir!")]
    [InlineData("Щука", "Shchuka")]
    [InlineData("Юлия", "Yuliya")]
    [InlineData("Тест-123", "Test-123")]
    [InlineData("", "")]
    public void ConvertToLatin_TransliteratesCyrillicToLatin(string source, string expected)
    {
        var actual = source.ConvertToLatin();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertToLatin_ReturnsEmptyString_ForNullInput()
    {
        string? source = null;

        var actual = source.ConvertToLatin();

        Assert.Equal(string.Empty, actual);
    }
}
