using System;

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
}
