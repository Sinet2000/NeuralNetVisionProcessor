using NetVisionProc.Common.Helpers;

namespace NetVisionProc.Common.Extensions
{
    /// <summary>
    /// Author: Nikita Nikitins .
    /// Email: nikitinsn6@gmail.com .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ObjectExt
    {
        /// <summary>
        /// Serializes the data to an indented JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the data to serialize.</typeparam>
        /// <param name="data">The data to serialize.</param>
        /// <param name="name">Optional name or identifier for the serialized data (e.g., variable name).</param>
        /// <returns>An indented JSON string representing the serialized data.</returns>
        public static string SerializeToIndentedJson<T>(this T data, [CallerArgumentExpression("data")] string? name = null)
        {
            string prefix = "serialize: " + typeof(T).Name;

            if (name.HasValue())
            {
                prefix += $"[{name}]";
            }

            prefix += Environment.NewLine;

            return prefix + JsonHelper.SerializeIndented(data);
        }
    }
}