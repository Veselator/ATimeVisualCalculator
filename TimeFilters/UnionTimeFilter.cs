namespace ATimeVisualCalculator.TimeFilters
{
    internal class UnionTimeFilter : ITimeFilter
    {
        public TimeFilterType Type => TimeFilterType.Or;

        public bool Apply(bool a, bool b)
        {
            return a || b;
        }
    }
}
