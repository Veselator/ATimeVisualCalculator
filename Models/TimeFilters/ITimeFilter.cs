namespace ATimeVisualCalculator.Models.TimeFilters
{
    public interface ITimeFilter
    {
        TimeFilterType Type { get; }
        bool Apply(bool a, bool b);
    }

    public enum TimeFilterType
    {
        And,
        Or,
        Xor
    }
}
