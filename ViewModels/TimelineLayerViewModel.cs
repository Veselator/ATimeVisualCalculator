using ATimeVisualCalculator.Models;
using ATimeVisualCalculator.Models.TimeFilters;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ATimeVisualCalculator.ViewModels
{
    public class TimelineLayerViewModel : ViewModelBase
    {
        private readonly TimelineEntry _entry;
        private readonly Action _onChanged;
        private string _name;
        private TimeFilterType _selectedFilter;

        public ObservableCollection<TimelineCellViewModel> Cells { get; } = new();

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    _entry.Timeline.Name = value;
                }
            }
        }

        public TimeFilterType SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    _entry.FilterType = value;
                    _onChanged?.Invoke();
                }
            }
        }

        public Array FilterTypes => Enum.GetValues(typeof(TimeFilterType));

        public double GreenHours => Cells.Count(c => c.IsActive) * 0.5;
        public double RedHours => Cells.Count(c => !c.IsActive) * 0.5;

        public ICommand DeleteCommand { get; }
        public ICommand ShiftLeftCommand { get; }
        public ICommand ShiftRightCommand { get; }
        public ICommand ShiftLeftDoubleCommand { get; }
        public ICommand ShiftRightDoubleCommand { get; }

        public TimelineEntry Entry => _entry;

        public TimelineLayerViewModel(TimelineEntry entry, Action onChanged, Action<TimelineLayerViewModel> onDelete)
        {
            _entry = entry;
            _onChanged = onChanged;
            _name = entry.Timeline.Name;
            _selectedFilter = entry.Filter.Type;

            // Build cells
            for (int i = 0; i < 48; i++)
            {
                int idx = i;
                var cell = new TimelineCellViewModel
                {
                    Index = i,
                    IsActive = entry.Timeline.GetEntry(i),
                    IsReadOnly = false
                };
                cell.ToggleCommand = new RelayCommand(_ =>
                {
                    entry.Timeline.ToggleEntry(idx);
                    cell.IsActive = entry.Timeline.GetEntry(idx);
                    UpdateStats();
                    _onChanged?.Invoke();
                });
                Cells.Add(cell);
            }

            DeleteCommand = new RelayCommand(_ => onDelete(this));
            ShiftLeftCommand = new RelayCommand(_ => Shift(-1));
            ShiftRightCommand = new RelayCommand(_ => Shift(1));
            ShiftLeftDoubleCommand = new RelayCommand(_ => Shift(-2));
            ShiftRightDoubleCommand = new RelayCommand(_ => Shift(2));
        }

        private void Shift(int offset)
        {
            _entry.Timeline.ShiftTimeline(offset);
            for (int i = 0; i < 48; i++)
                Cells[i].IsActive = _entry.Timeline.GetEntry(i);
            UpdateStats();
            _onChanged?.Invoke();
        }

        private void UpdateStats()
        {
            OnPropertyChanged(nameof(GreenHours));
            OnPropertyChanged(nameof(RedHours));
        }

        public void RefreshCells()
        {
            for (int i = 0; i < 48; i++)
                Cells[i].IsActive = _entry.Timeline.GetEntry(i);
            UpdateStats();
        }
    }
}
