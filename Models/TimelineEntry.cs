using ATimeVisualCalculator.Models.TimeFilters;

namespace ATimeVisualCalculator.Models
{
    public class TimelineEntry
    {
        public ITimeFilter Filter { get; set; } = new IntersectTimeFilter();
        public Timeline Timeline { get; set; } = new();

        public TimeFilterType FilterType
        {
            get => Filter.Type;
            set => Filter = TimeFilterFactory.Create(value);
        }
    }
}
