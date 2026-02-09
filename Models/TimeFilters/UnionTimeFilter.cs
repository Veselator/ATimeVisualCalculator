namespace ATimeVisualCalculator.Models.TimeFilters
{
    public class UnionTimeFilter : ITimeFilter
    {
        public TimeFilterType Type => TimeFilterType.Or;
        public bool Apply(bool a, bool b) => a || b;
    }
}
