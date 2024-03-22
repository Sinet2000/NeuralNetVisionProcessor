using System.ComponentModel;
using System.Reflection;
using EnumsNET;

namespace NetVisionProc.Common.Extensions
{
    public static class EnumExt
    {
        public static string GetDescription(this Enum? enumeration)
        {
            if (enumeration == null)
            {
                return string.Empty;
            }

            var value = enumeration.ToString();
            var type = enumeration.GetType();
            var descAttribute =
                type.GetField(value)?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return descAttribute is { Length: > 0 } ? descAttribute[0].Description : value;
        }

        public static string? GetDisplayName(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            MemberInfo[] memberInfo = enumType.GetMember(enumValue.ToString());

            if (memberInfo is { Length: > 0 })
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attributes is { Length: > 0 })
                {
                    return ((DisplayAttribute)attributes[0]).Name;
                }
            }

            return enumValue.ToString();
        }

        public static string? GetShortName(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            MemberInfo[] memberInfo = enumType.GetMember(enumValue.ToString());

            if (memberInfo is { Length: > 0 })
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attributes is { Length: > 0 })
                {
                    return ((DisplayAttribute)attributes[0]).ShortName;
                }
            }

            return enumValue.ToString();
        }

        public static TEnum ToEnum<TEnum>(this string value)
            where TEnum : struct, Enum
        {
            if (Enum.TryParse(value, true, out TEnum result))
            {
                return result;
            }

            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Could not parse '{value}' to the enum: {typeof(TEnum).Name}");
        }

        public static string Code<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            return value.AsString();
        }

        public static string Name<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            string? displayName = value.AsString(EnumFormat.DisplayName);

            Guard.Against.Null(displayName, nameof(displayName));
            return displayName;
        }

        public static string Description<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            string? description = value.AsString(EnumFormat.Description);

            Guard.Against.Null(description, nameof(description));
            return description;
        }
    }
}