namespace SportClubsChallenges.Utils.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class EnumsHelper
    {
        public static Dictionary<byte, string> GetEnumWithDescriptions<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return Enum.GetValues(typeof(T))
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
    }
}