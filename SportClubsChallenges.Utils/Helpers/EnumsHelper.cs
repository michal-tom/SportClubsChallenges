namespace SportClubsChallenges.Utils.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class EnumsHelper
    {
        public static Dictionary<byte, string> GetEnumValues<TEnum>() where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            return Enum.GetValues(typeof(TEnum))
                .Cast<object>()
                .Where(p => p != null)
                .ToDictionary(p => (byte) p, p => p.ToString());
        }

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

        public static Dictionary<byte, string> GetEnumWithAttribute<TEnum, TAttribute>() where TEnum : struct, IConvertible where TAttribute : Attribute
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            return Enum.GetValues(typeof(TEnum))
                .Cast<object>()
                .Where(p => p != null && GetEnumAttribute<TAttribute>((Enum) p) != null )
                .ToDictionary(p => (byte) p, p => p.ToString());
        }

        public static string GetEnumDescription(Enum value)
        {
            var attribute = GetEnumAttribute<DescriptionAttribute>(value);
            return attribute?.Description ?? string.Empty;
        }

        public static T GetEnumAttribute<T> (Enum value) where T : Attribute
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var name = value.ToString();
            var fieldInfo = value.GetType().GetField(name);
            if (fieldInfo == null)
            {
                return null;
            }

            return (T) fieldInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
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