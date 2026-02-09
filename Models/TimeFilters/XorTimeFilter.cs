namespace ATimeVisualCalculator.Models.TimeFilters
{
    public class XorTimeFilter : ITimeFilter
    {
        public TimeFilterType Type => TimeFilterType.Xor;
        public bool Apply(bool a, bool b) => a ^ b;
    }
}
