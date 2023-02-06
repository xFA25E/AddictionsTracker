using System;
using System.Collections.Generic;

namespace AddictionsTracker;

public static class Extensions
{
    static readonly TimeOnly zeroTime = new TimeOnly(0, 0, 0);

    public static int Subtract(this DateOnly a, DateOnly b)
        => (int)(a.ToDateTime() - b.ToDateTime()).TotalDays;

    public static DateTime ToDateTime(this DateOnly d)
        => d.ToDateTime(zeroTime);

    public static DateTimeOffset ToDateTimeOffset(this DateOnly d)
        => new DateTimeOffset(d.ToDateTime());

    public static DateOnly ToDateOnly(this DateTime dt)
        => DateOnly.FromDateTime(dt);

    public static DateOnly ToDateOnly(this DateTimeOffset dto)
        => dto.DateTime.ToDateOnly();

    public static int BinarySearch<T>(this IList<T> source, T item, IComparer<T>? comparer = null)
    {
        comparer = comparer ?? Comparer<T>.Default;

        int lower = 0;
        int upper = source.Count - 1;

        while (lower <= upper)
        {
            int middle = lower + (upper - lower) / 2;
            int comparisonResult = comparer.Compare(item, source[middle]);
            if (comparisonResult == 0)
                return middle;
            else if (comparisonResult < 0)
                upper = middle - 1;
            else
                lower = middle + 1;
        }

        return ~lower;
    }
}
