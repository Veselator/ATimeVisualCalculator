using ATimeVisualCalculator.Models.TimeFilters;
using System;
using System.Collections.Generic;

namespace ATimeVisualCalculator.Models
{
    public class TimeManager
    {
        private string _name = "New Calculation";
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private Timeline _resultingTimeLine = new();
        public Timeline ResultingTimeLine => _resultingTimeLine;

        private List<TimelineEntry> _timeLines = new();
        public List<TimelineEntry> TimeLines => _timeLines;

        public Action? OnRecalculated;

        public TimeManager() { }

        public TimeManager(string name)
        {
            _name = name;
        }

        public void Recalculate()
        {
            _resultingTimeLine.Reset();

            if (_timeLines.Count == 0)
            {
                _resultingTimeLine.NotifyAllChanged();
                OnRecalculated?.Invoke();
                return;
            }

            // First timeline: copy directly
            for (int i = 0; i < _timeLines[0].Timeline.Length; i++)
            {
                _resultingTimeLine.SetEntry(i, _timeLines[0].Timeline[i]);
            }

            // Apply filters for subsequent timelines
            for (int t = 1; t < _timeLines.Count; t++)
            {
                var entry = _timeLines[t];
                ITimeFilter filter = entry.Filter;
                for (int i = 0; i < entry.Timeline.Length; i++)
                {
                    _resultingTimeLine.SetEntry(i, filter.Apply(_resultingTimeLine[i], entry.Timeline[i]));
                }
            }

            _resultingTimeLine.NotifyAllChanged();
            OnRecalculated?.Invoke();
        }

        public void AddTimeline(Timeline timeline, ITimeFilter filter)
        {
            _timeLines.Add(new TimelineEntry { Timeline = timeline, Filter = filter });
            Recalculate();
        }

        public void RemoveTimeline(int index)
        {
            if (index < 0 || index >= _timeLines.Count) return;
            _timeLines.RemoveAt(index);
            Recalculate();
        }

        public void MoveTimeline(int from, int to)
        {
            if (from < 0 || from >= _timeLines.Count) return;
            if (to < 0 || to >= _timeLines.Count) return;
            if (from == to) return;

            var item = _timeLines[from];
            _timeLines.RemoveAt(from);
            _timeLines.Insert(to, item);
            Recalculate();
        }

        public Timeline? GetTimeline(int index)
        {
            if (index < 0 || index >= _timeLines.Count) return null;
            return _timeLines[index].Timeline;
        }

        public override string ToString() => _resultingTimeLine.ToString();
    }
}
