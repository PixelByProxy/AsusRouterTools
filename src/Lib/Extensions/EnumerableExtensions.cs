namespace PixelByProxy.Asus.Router.Extensions;

internal static class EnumerableExtensions
{
    public static T? IndexAtOrDefault<T>(this IEnumerable<T> enumerable, int index)
    {
        var list = enumerable.ToArray();

        if (list.Length > index)
            return list[index];

        return default;
    }
}