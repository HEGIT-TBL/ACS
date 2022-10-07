using System;
using System.ComponentModel;

namespace ACS.Helpers
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            var fi = enumValue.GetType().GetField(enumValue.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes != null && attributes.Length > 0
                ? attributes[0].Description
                : enumValue.ToString();
        }
    }

    public class EnumExtensionWrapper
    {
        public static string GetDescription(Enum enumValue)
        {
            return enumValue.GetDescription();
        }
    }
}
