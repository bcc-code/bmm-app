using System.Linq;

namespace BMM.Core.Extensions
{
    public static class IntExtensions
    {
        public static bool IsOneOf(this int value, params int[] values) => values.Contains(value);
        public static bool IsNoneOf(this int value, params int[] values) => !value.IsOneOf(values);
    }
}