namespace BMM.Core.Extensions
{
    public static class LongExtensions
    {
        public static long WithPossibleLowestValue(this long value, long lowestPossibleValue)
        {
            if (value < lowestPossibleValue)
                return lowestPossibleValue;

            return value;
        }
    }
}