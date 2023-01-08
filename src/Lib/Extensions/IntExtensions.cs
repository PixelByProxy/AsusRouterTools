namespace PixelByProxy.Asus.Router.Extensions;

internal static class IntExtensions
{
    public static bool IsInRange(this int value, int start, int end)
    {
        return value >= start && value <= end;
    }
}