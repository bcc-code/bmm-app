using System;
using System.Linq;

namespace BMM.Core.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsOneOf<T>(this T @enum, params T[] values) where T : Enum =>
            values.Contains(@enum);

        public static bool IsNoneOf<T>(this T @enum, params T[] values) where T : Enum =>
            !@enum.IsOneOf(values);
    }
}