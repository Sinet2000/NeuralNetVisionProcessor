using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace NetVisionProc.Common.Extensions
{
    public static class TypeExt
    {
        public static string GetName(this Type type, out string? description)
        {
            var displayAttribute = type.GetCustomAttribute<DisplayAttribute>();
            Guard.Against.Null(displayAttribute);
            Guard.Against.NullOrEmpty(displayAttribute.Name);
            description = displayAttribute.Description;

            return displayAttribute.Name;
        }

        public static T GetAttributeByType<T>(this Type type) where T: Attribute
        {
            var attributeValue = type.GetCustomAttribute<T>();
            Guard.Against.Null(attributeValue);

            return attributeValue;
        }

        public static string GetTableName(this Type type)
        {
            var tableAttributeValue = type.GetCustomAttribute<TableAttribute>();

            return tableAttributeValue is null ? type.Name : tableAttributeValue.Name;
        }
        
        public static bool IsPrimitiveProperty(this PropertyInfo pi)
        {
            return pi.PropertyType.IsPrimitive
                   || pi.PropertyType == typeof(string)
                   || pi.PropertyType == typeof(double)
                   || pi.PropertyType == typeof(Enum)
                   || pi.PropertyType == typeof(int)
                   || pi.PropertyType == typeof(DateTime);
        }
        
        public static bool IsListProperty(this PropertyInfo pi)
        {
            return !IsPrimitiveProperty(pi)
                   && pi.PropertyType.IsAssignableTo(typeof(IEnumerable));
        }
    }
}