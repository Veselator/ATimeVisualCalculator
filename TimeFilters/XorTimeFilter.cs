namespace ATimeVisualCalculator.TimeFilters
{
    internal class XorTimeFilter : ITimeFilter
    {
        public TimeFilterType Type => TimeFilterType.Xor;
        public bool Apply(bool a, bool b)
        {
            return a ^ b;
        }
    }
}
