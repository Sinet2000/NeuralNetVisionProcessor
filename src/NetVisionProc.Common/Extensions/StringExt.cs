namespace NetVisionProc.Common.Extensions
{
    /// <summary>
    /// String extension methods for common operations.
    /// Author: Nikita Nikitins .
    /// Email: nikitinsn6@gmail.com .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class StringExt
    {
        /// <summary>
        /// Tries to parse the specified string into a boolean value.
        /// </summary>
        /// <param name="value">A nullable string representing the boolean value.</param>
        /// <param name="defaultValue">The default value to return if the conversion is not successful.</param>
        /// <returns>
        /// The boolean value parsed from the input string, or the specified default value
        /// if the conversion is not successful.
        /// </returns>
        public static bool ToBoolSafe(this string? value, bool defaultValue = false)
        {
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// Tries to convert the specified string to an integer.
        /// </summary>
        /// <param name="value">Nullable string that represents the integer value.</param>
        /// <returns>The integer value if conversion is successful.</returns>
        /// <exception cref="FormatException">Thrown if the conversion is not successful.</exception>
        public static int ToIntSafe(this string? value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            throw new FormatException($"Failed to parse '{value}' as an integer.");
        }

        /// <summary>
        /// Tries to convert the specified string to an integer.
        /// </summary>
        /// <param name="value">Nullable string that represents the integer value.</param>
        /// <returns>The integer value if conversion is successful.</returns>
        /// <exception cref="FormatException">Thrown if the conversion is not successful.</exception>
        public static double ToDoubleSafe(this string? value)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }

            throw new FormatException($"Failed to parse '{value}' as an double.");
        }

        /// <summary>
        /// Checks if the string is not null and not whitespace.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the string is not null and not whitespace; otherwise, false.</returns>
        public static bool HasValue([NotNullWhen(true)] this string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Checks if the string is null or whitespace.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the string is null or whitespace; otherwise, false.</returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string TrimToLength(this string value, int length)
        {
            value = value.Trim();

            return value.Length > length ? value[..length].Trim() : value.Trim();
        }

        public static string AppendWithSeperator(this string value, string? part, char seperator = ' ')
        {
            if (part.HasValue())
            {
                if (value.Length > 0)
                {
                    string delim = seperator.ToString();
                    if (seperator != ' ')
                    {
                        delim += ' ';
                    }

                    return string.Concat(value, delim, part.Trim());
                }

                return part.Trim();
            }

            return value;
        }

        /// <summary>
        /// Ensures that a string ends with the specified character.
        /// </summary>
        /// <param name="input">The input string to be checked and modified if necessary.</param>
        /// <param name="character">The character to ensure is at the end of the string.</param>
        /// <returns>
        /// If the input string is not empty and already ends with the specified character returns the original string.
        /// Otherwise, appends the character to the end of the string.
        /// If the input string is empty, returns it as is.
        /// </returns>
        public static string EnsureEndsWithCharacter(this string input, char character)
        {
            if (input.IsNullOrEmpty())
            {
                return input;
            }

            if (input.EndsWith(character))
            {
                return input;
            }

            return input + character;
        }

        public static string FirstLetter(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str.Trim()[..1];
        }

        public static string GetUniqueRtCombIdentifier()
        {
            var rtComb = RT.Comb.Provider.Sql.Create();
            return RT.Comb.Provider.Sql.GetTimestamp(rtComb).ToString("o");
        }
    }
}