namespace BMM.Core.Extensions;

public static class StringExtensions
{
    public static Uri ToUri(this string link) => new(link);
}