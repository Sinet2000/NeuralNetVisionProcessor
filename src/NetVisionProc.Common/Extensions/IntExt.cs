namespace NetVisionProc.Common.Extensions
{
    /// <summary>
    /// Author: Nikita Nikitins .
    /// Email: nikitinsn6@gmail.com .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class IntExt
    {
        public const int MinNaturalValue = 1;

        public static bool HasValue([NotNullWhen(true)] this int? value)
        {
            return value is > 0;
        }

        public static bool HasValue(this int value)
        {
            return value > 0;
        }
    }
}