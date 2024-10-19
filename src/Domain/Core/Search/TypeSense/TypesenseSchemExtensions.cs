namespace Anyding.Search;

public static class TypesenseSchemExtensions
{
    public static string ToId(this Guid id)
    {
        return id.ToString("N");
    }

    public static long? ToUnixTimeSeconds(this DateOnly? date)
    {
        if (date is { } d)
        {
            return (int)new DateTime(d.Year, d.Month, d.Day).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        return default;
    }

    public static DateIndex ToDateIndex(this DateTimeOffset? date)
    {
        if (date is { } d)
        {
            new DateIndex()
            {
                Year = d.Year,
                Month = d.Month,
                Day = d.Day,
                YearMonth = $"{d.Year}-{d.Month}",
                Date = $"{d.Year}-{d.Month}-{d.Day}",
                Timestamp = d.ToUnixTimeSeconds()
            };
        }

        return default;
    }
}
