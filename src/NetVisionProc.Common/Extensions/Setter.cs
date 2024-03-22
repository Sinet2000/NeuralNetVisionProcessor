namespace NetVisionProc.Common.Extensions
{
    /// <summary>
    /// Extension methods for setting and guarding values using Ardalis Guard principles.
    /// Author: Nikita Nikitins
    /// Email: nikitinsn6@gmail.com .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Setter
    {
        /// <summary>
        /// Guards against NullOrWhiteSpace and trims whitespace.
        /// </summary>
        /// <param name="value">The input string to guard.</param>
        /// <param name="parameterName">The name of the parameter (automatically filled by the compiler).</param>
        /// <returns>The trimmed and guarded string.</returns>
        public static string SetAndGuard([NotNull] string? value, [CallerArgumentExpression("value")] string? parameterName = null)
        {
            Guard.Against.NullOrWhiteSpace(value, parameterName);
            return value.Trim();
        }

        /// <summary>
        /// Guards against NegativeOrZero for an integer value.
        /// </summary>
        /// <param name="value">The input integer to guard.</param>
        /// <param name="parameterName">The name of the parameter (automatically filled by the compiler).</param>
        /// <returns>The guarded integer value.</returns>
        public static int SetAndGuard([NotNull] int value, [CallerArgumentExpression("value")] string? parameterName = null)
        {
            return Guard.Against.NegativeOrZero(value, parameterName);
        }

        /// <summary>
        /// Sets the value ensuring it falls within the specified range.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="minValue">The minimum allowed value (inclusive).</param>
        /// <param name="maxValue">The maximum allowed value (inclusive).</param>
        /// <param name="parameterName">The name of the parameter (automatically filled by the compiler).</param>
        /// <returns>The value within the specified range.</returns>
        public static int SetAndGuardInRange(int value, int minValue, int maxValue, [CallerArgumentExpression("value")] string? parameterName = null)
        {
            if (value < minValue || value > maxValue)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"The value must be between {minValue} and {maxValue} (inclusive).");
            }

            return value;
        }

        /// <summary>
        /// Guards against Negative for a decimal value.
        /// </summary>
        /// <param name="value">The input decimal to guard.</param>
        /// <param name="parameterName">The name of the parameter (automatically filled by the compiler).</param>
        /// <returns>The guarded decimal value.</returns>
        public static decimal SetAndGuard([NotNull] decimal value, [CallerArgumentExpression("value")] string? parameterName = null)
        {
            return Guard.Against.Negative(value, parameterName);
        }

        /// <summary>
        /// Guards against NegativeOrZero for an integer value if it is not null.
        /// </summary>
        /// <param name="value">The nullable integer value to guard.</param>
        /// <param name="parameterName">The name of the parameter (automatically filled by the compiler).</param>
        /// <returns>The guarded nullable integer value.</returns>
        public static int? SetAndGuard(int? value, [CallerArgumentExpression("value")] string? parameterName = null)
        {
            if (value is not null)
            {
                Guard.Against.NegativeOrZero(value.Value, parameterName);
            }

            return value;
        }

        /// <summary>
        /// Returns null if the input string is IsNullOrWhiteSpace, trims whitespace otherwise.
        /// </summary>
        /// <param name="value">The input string to set or guard.</param>
        /// <returns>The trimmed and guarded string or null.</returns>
        public static string? Set(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }
    }
}