namespace PixelByProxy.Asus.Router.Extensions;

internal static class StringExtensions
{
    public static TEnum ToEnum<TEnum>(this string? value)
        where TEnum : struct
    {
        if (Enum.TryParse(value, true, out TEnum result))
            return result;

        return default;
    }
}