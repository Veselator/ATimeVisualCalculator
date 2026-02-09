using ATimeVisualCalculator.Models.TimeFilters;
using System.Collections.Generic;

namespace ATimeVisualCalculator.Models
{
    /// <summary>
    /// Root save data object containing all calculation tabs
    /// </summary>
    public class SaveData
    {
        public int Version { get; set; } = 1;
        public List<CalculationSaveData> Calculations { get; set; } = new();
    }

    /// <summary>
    /// Save data for a single TimeManager (one tab/calculation)
    /// </summary>
    public class CalculationSaveData
    {
        public string Name { get; set; } = "New Calculation";
        public List<TimelineEntrySaveData> Timelines { get; set; } = new();
    }

    /// <summary>
    /// Save data for a single TimelineEntry (one layer within a calculation)
    /// </summary>
    public class TimelineEntrySaveData
    {
        public string Name { get; set; } = "New timeline";
        public TimeFilterType FilterType { get; set; } = TimeFilterType.And;
        public bool[] Entries { get; set; } = new bool[48];
    }
}
