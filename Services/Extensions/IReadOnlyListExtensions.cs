namespace Application.Extensions;

public static class IReadOnlyListExtensions
{
    public static int FindIndex<T>(this IReadOnlyList<T> self, Predicate<T> match)
    {
        for (var i = 0; i < self.Count; i++)
        {
            if (match(self[i])) return i;
        }

        return -1;
    }

    public static int FindLastIndex<T>(this IReadOnlyList<T> self, Predicate<T> match)
    {
        for (var i = self.Count - 1; i >= 0; i--)
        {
            if (match(self[i])) return i;
        }

        return -1;
    }
}