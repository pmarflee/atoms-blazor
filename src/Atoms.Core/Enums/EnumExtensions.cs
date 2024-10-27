using System.ComponentModel;

namespace Atoms.Core.Enums;

public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());

        if (field == null) return enumValue.ToString();

        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            return attribute.Description;
        }

        return enumValue.ToString();
    }

    public static IEnumerable<KeyValuePair<TEnum, string>> GetValuesDescriptions<TEnum>()
        where TEnum : struct, Enum 
    {
        return Enum.GetValues<TEnum>()
            .Select(x => new KeyValuePair<TEnum, string>(x, x.GetDescription()))
            .ToList();
    }
}
