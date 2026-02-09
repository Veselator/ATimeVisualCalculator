using System.Windows.Input;

namespace ATimeVisualCalculator.ViewModels
{
    public class TimelineCellViewModel : ViewModelBase
    {
        private bool _isActive;
        private int _index;
        private bool _isReadOnly;

        public bool IsActive
        {
            get => _isActive;
            set { SetProperty(ref _isActive, value); OnPropertyChanged(nameof(IsActive)); }
        }

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        /// <summary>
        /// Time label like "0:00", "0:30", "1:00", etc.
        /// </summary>
        public string TimeLabel
        {
            get
            {
                int hours = _index / 2;
                int minutes = (_index % 2) * 30;
                return $"{hours}:{minutes:D2}";
            }
        }

        public ICommand? ToggleCommand { get; set; }
    }
}
