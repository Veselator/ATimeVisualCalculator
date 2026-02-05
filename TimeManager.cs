using ATimeVisualCalculator.TimeFilters;
using System;
using System.Reflection;

namespace ATimeVisualCalculator
{
    public class TimeManager
    {
        // Менеджер времени - хранит информацию о всех временных шкалах
        // при необходимости применяет фильтры

        private Timeline _resultingTimeLine = new();

        private List<TimelineEntry> _timeLines = new();

        private Action OnNewTimelineAdded;
        private Action OnTimelineRemoved;
        private Action OnTimelineOrderChanged;

        public TimeManager()
        {
            OnNewTimelineAdded += RecalculateAllTimeline;
            OnTimelineRemoved += RecalculateAllTimeline;
            OnTimelineOrderChanged += RecalculateAllTimeline;

            foreach (var entry in _timeLines)
            {
                entry.timeline.OnTimelineChanged += RecalculateTimeline;
            }
        }

        public TimeManager(List<TimelineEntry> timeLines)
        {
            _timeLines = timeLines;
        }

        ~TimeManager()
        {
            OnNewTimelineAdded -= RecalculateAllTimeline;
            OnTimelineRemoved -= RecalculateAllTimeline;
            OnTimelineOrderChanged -= RecalculateAllTimeline;

            foreach (var entry in _timeLines)
            {
                entry.timeline.OnTimelineChanged -= RecalculateTimeline;
            }
        }

        private void RecalculateTimeline(string _, int id)
        {
            RecalculateSpecificPoint(id);
        }

        private void RecalculateSpecificPoint(int index)
        {
            _resultingTimeLine.SetEntry(index, true);

            foreach (var entry in _timeLines)
            {
                ITimeFilter filter = entry.Filter;
                _resultingTimeLine.SetEntry(index, filter.Apply(_resultingTimeLine[index], entry.timeline[index]));
            }
        }

        private void RecalculateAllTimeline()
        {
            _resultingTimeLine.Reset();

            foreach (var entry in _timeLines)
            {
                ITimeFilter filter = entry.Filter;
                for (int i = 0; i < entry.timeline.Length; i++)
                {
                    _resultingTimeLine.SetEntry(i, filter.Apply(_resultingTimeLine[i], entry.timeline[i]));
                }
            }
        }

        public void AddTimeline(Timeline timeline, ITimeFilter filter)
        {
            _timeLines.Add(new TimelineEntry { timeline = timeline, Filter = filter });
            OnNewTimelineAdded?.Invoke();
        }

        public void RemoveTimeline(int index)
        {
            if(index < 0 || index >= _timeLines.Count)
            {
                return;
            }

            _timeLines.RemoveAt(index);
            OnTimelineRemoved?.Invoke();
        }

        public void ReplaceTimeline(int from, int to)
        {
            if (from < 0 || from >= _timeLines.Count)
            {
                return;
            }

            if (to < 0 || to >= _timeLines.Count)
            {
                return;
            }

            TimelineEntry tempEntry = _timeLines[from];

            _timeLines.RemoveAt(from);
            _timeLines.Insert(to, tempEntry);

            OnTimelineOrderChanged?.Invoke();
        }

        public Timeline? GetTimeline(int index)
        {
            if(index < 0 || index >= _timeLines.Count)
            {
                return null;
            }

            return _timeLines[index].timeline;
        }

        public override string ToString()
        {
            return _resultingTimeLine.ToString();
        }
    }

    public struct TimelineEntry
    {
        public ITimeFilter Filter;
        public Timeline timeline;
    }
}