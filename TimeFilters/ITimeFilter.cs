namespace ATimeVisualCalculator.TimeFilters
{
    public interface ITimeFilter
    {
        public TimeFilterType Type { get; }
        public bool Apply(bool a, bool b); 
    }

    public enum TimeFilterType
    {
        And,
        Or,
        Xor
    }
}
