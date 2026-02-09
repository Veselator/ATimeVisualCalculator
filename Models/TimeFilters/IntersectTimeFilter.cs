namespace ATimeVisualCalculator.Models.TimeFilters
{
    public class IntersectTimeFilter : ITimeFilter
    {
        public TimeFilterType Type => TimeFilterType.And;
        public bool Apply(bool a, bool b) => a && b;
    }
}
