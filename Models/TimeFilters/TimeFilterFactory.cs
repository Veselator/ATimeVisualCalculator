namespace ATimeVisualCalculator.Models.TimeFilters
{
    public static class TimeFilterFactory
    {
        public static ITimeFilter Create(TimeFilterType type) => type switch
        {
            TimeFilterType.And => new IntersectTimeFilter(),
            TimeFilterType.Or => new UnionTimeFilter(),
            TimeFilterType.Xor => new XorTimeFilter(),
            _ => new IntersectTimeFilter()
        };
    }
}
