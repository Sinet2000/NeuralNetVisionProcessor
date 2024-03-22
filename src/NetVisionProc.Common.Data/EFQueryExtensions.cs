using Microsoft.EntityFrameworkCore.Query;

namespace NetVisionProc.Common.Data
{
    public static class EFQueryExtensions
    {
        public static bool Contains(string matchExpression, [NotParameterized] string pattern)
        {
            return matchExpression?.Contains(pattern, StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool StartsWith(string matchExpression, [NotParameterized] string pattern)
        {
            return matchExpression?.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}