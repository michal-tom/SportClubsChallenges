namespace SportClubsChallenges.Utils.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class EnumsHelper
    {
        public static Dictionary<byte, string> GetEnumWithDescriptions<TEnum>() where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            return Enum.GetValues(typeof(TEnum))
                .Cast<object>()
                .Where(p => p != null)
                .ToDictionary(p => (byte) p, p => GetEnumDescription((Enum) p));
        }

        public static string GetEnumDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var name = value.ToString();
            var fieldInfo = value.GetType().GetField(name);
            if (fieldInfo == null)
            {
                return string.Empty;
            }

            var attributes = (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                name = attributes[0].Description;
            }

            return name;
        }

        public static byte GetEnumIdByName<TEnum>(string valueString) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            if (Enum.TryParse<TEnum>(valueString, true, out TEnum enumValue))
            {
                return Convert.ToByte(enumValue);
            }

            return default(byte);
        }
    }
}