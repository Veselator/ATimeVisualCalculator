using ATimeVisualCalculator.Models;
using ATimeVisualCalculator.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ATimeVisualCalculator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private CalculationViewModel? _selectedCalculation;
        private bool _isSidebarOpen;

        public ObservableCollection<CalculationViewModel> Calculations { get; } = new();

        public CalculationViewModel? SelectedCalculation
        {
            get => _selectedCalculation;
            set => SetProperty(ref _selectedCalculation, value);
        }

        public bool IsSidebarOpen
        {
            get => _isSidebarOpen;
            set => SetProperty(ref _isSidebarOpen, value);
        }

        public ICommand ToggleSidebarCommand { get; }
        public ICommand NewCalculationCommand { get; }
        public ICommand DeleteCalculationCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainViewModel()
        {
            ToggleSidebarCommand = new RelayCommand(_ => IsSidebarOpen = !IsSidebarOpen);
            NewCalculationCommand = new RelayCommand(_ => CreateNewCalculation());
            DeleteCalculationCommand = new RelayCommand(param => DeleteCalculation(param as CalculationViewModel));
            SaveCommand = new RelayCommand(_ => Save());
            LoadCommand = new RelayCommand(_ => Load());

            // Try to load saved data
            Load();

            // If no saved data, create a default one
            if (Calculations.Count == 0)
                CreateNewCalculation();
        }

        private void CreateNewCalculation()
        {
            var manager = new TimeManager($"Calculation {Calculations.Count + 1}");
            var vm = new CalculationViewModel(manager);
            Calculations.Add(vm);
            SelectedCalculation = vm;
        }

        private void DeleteCalculation(CalculationViewModel? calc)
        {
            if (calc == null) return;
            int idx = Calculations.IndexOf(calc);
            Calculations.Remove(calc);

            if (SelectedCalculation == calc)
            {
                SelectedCalculation = Calculations.Count > 0
                    ? Calculations[Math.Min(idx, Calculations.Count - 1)]
                    : null;
            }

            // Ensure at least one calculation exists
            if (Calculations.Count == 0)
                CreateNewCalculation();
        }

        public void Save()
        {
            var managers = Calculations.Select(c => c.Manager);
            var saveData = TimelineSaveManager.ConvertToSaveData(managers);
            TimelineSaveManager.Save(saveData);
        }

        private void Load()
        {
            try
            {
                var saveData = TimelineLoadManager.Load();
                if (saveData.Calculations.Count == 0) return;

                var managers = TimelineLoadManager.ConvertFromSaveData(saveData);
                Calculations.Clear();

                foreach (var manager in managers)
                {
                    var vm = new CalculationViewModel(manager);
                    vm.RefreshResult();
                    Calculations.Add(vm);
                }

                SelectedCalculation = Calculations.FirstOrDefault();
            }
            catch
            {
                // If loading fails, silently continue
            }
        }
    }
}
