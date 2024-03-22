using System.Reflection;

namespace NetVisionProc.Common
{
    public abstract class StringEnum : IComparable
    {
        protected StringEnum(string code, string name)
        {
            (Code, Name) = (code, name);
        }

        public string Name { get; private set; }

        public string Code { get; private set; }

        public static bool operator ==(StringEnum left, StringEnum right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(StringEnum left, StringEnum right)
        {
            return !(left == right);
        }

        public static bool operator <(StringEnum left, StringEnum right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(StringEnum left, StringEnum right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(StringEnum left, StringEnum right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(StringEnum left, StringEnum right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        public static IEnumerable<T> GetAll<T>()
        where T : StringEnum
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
        }

        public static T FromValue<T>(string code)
        where T : StringEnum
        {
            var matchingItem = Parse<T, string>(code, "code", item => string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase));
            return matchingItem;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not StringEnum otherValue)
            {
                return false;
            }

            bool typeMatches = GetType().Equals(obj.GetType());
            bool valueMatches = Code.Equals(otherValue.Code, StringComparison.OrdinalIgnoreCase);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is not StringEnum otherValue)
            {
                return 1;
            }

            return string.Compare(Code, otherValue!.Code, StringComparison.OrdinalIgnoreCase);
        }

        private static T Parse<T, TFrom>(TFrom value, string description, Func<T, bool> predicate)
        where T : StringEnum
        {
            var found = GetAll<T>().FirstOrDefault(predicate);

            if (found is null)
            {
                throw new InvalidOperationException($"{nameof(Parse)} - '{value}' in {typeof(T)}");
            }

            return found;
        }
    }
}