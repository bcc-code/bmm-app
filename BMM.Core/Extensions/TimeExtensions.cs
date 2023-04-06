namespace BMM.Core.Extensions;

public static class TimeExtensions
{
    public static long ToMilliseconds(this int seconds)
    {
        return seconds * 1000;
    }
}