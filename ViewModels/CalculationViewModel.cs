using ATimeVisualCalculator.Models;
using ATimeVisualCalculator.Models.TimeFilters;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ATimeVisualCalculator.ViewModels
{
    public class CalculationViewModel : ViewModelBase
    {
        private readonly TimeManager _manager;
        private string _name;

        public ObservableCollection<TimelineLayerViewModel> Layers { get; } = new();
        public ObservableCollection<TimelineCellViewModel> ResultCells { get; } = new();

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                    _manager.Name = value;
            }
        }

        public double ResultGreenHours => ResultCells.Count(c => c.IsActive) * 0.5;
        public double ResultRedHours => ResultCells.Count(c => !c.IsActive) * 0.5;

        public TimeManager Manager => _manager;

        public ICommand AddTimelineCommand { get; }

        public CalculationViewModel(TimeManager manager)
        {
            _manager = manager;
            _name = manager.Name;

            // Build result cells (read-only)
            for (int i = 0; i < 48; i++)
            {
                ResultCells.Add(new TimelineCellViewModel
                {
                    Index = i,
                    IsActive = manager.ResultingTimeLine.GetEntry(i),
                    IsReadOnly = true
                });
            }

            // Build layers
            foreach (var entry in manager.TimeLines)
            {
                AddLayerVM(entry);
            }

            AddTimelineCommand = new RelayCommand(_ => AddNewTimeline());
        }

        private void AddLayerVM(TimelineEntry entry)
        {
            var layerVm = new TimelineLayerViewModel(entry, OnLayerChanged, OnDeleteLayer);
            Layers.Add(layerVm);
        }

        private void AddNewTimeline()
        {
            var timeline = new Timeline { Name = $"Timeline {Layers.Count + 1}" };
            var filter = new IntersectTimeFilter();
            var entry = new TimelineEntry { Timeline = timeline, Filter = filter };
            _manager.TimeLines.Add(entry);
            AddLayerVM(entry);
            _manager.Recalculate();
            RefreshResult();
        }

        private void OnDeleteLayer(TimelineLayerViewModel layer)
        {
            int idx = Layers.IndexOf(layer);
            if (idx < 0) return;
            Layers.RemoveAt(idx);
            _manager.RemoveTimeline(idx);
            RefreshResult();
        }

        private void OnLayerChanged()
        {
            _manager.Recalculate();
            RefreshResult();
        }

        public void RefreshResult()
        {
            for (int i = 0; i < 48; i++)
                ResultCells[i].IsActive = _manager.ResultingTimeLine.GetEntry(i);

            OnPropertyChanged(nameof(ResultGreenHours));
            OnPropertyChanged(nameof(ResultRedHours));
        }

        public void MoveLayer(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= Layers.Count) return;
            if (toIndex < 0 || toIndex >= Layers.Count) return;
            if (fromIndex == toIndex) return;

            Layers.Move(fromIndex, toIndex);
            _manager.MoveTimeline(fromIndex, toIndex);
            RefreshResult();
        }
    }
}
