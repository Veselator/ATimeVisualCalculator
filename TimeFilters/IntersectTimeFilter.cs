namespace ATimeVisualCalculator.TimeFilters
{
    internal class IntersectTimeFilter : ITimeFilter
    {
        public TimeFilterType Type => TimeFilterType.And;

        public bool Apply(bool a, bool b)
        {
            return a && b;
        }
    }
}
