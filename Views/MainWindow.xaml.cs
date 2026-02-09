using ATimeVisualCalculator.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ATimeVisualCalculator.Views
{
    public partial class MainWindow : Window
    {
        private TimelineLayerViewModel? _draggedLayer;
        private Point _dragStartPoint;

        public MainWindow()
        {
            InitializeComponent();

            // Keyboard shortcuts
            InputBindings.Add(new KeyBinding(
                ((MainViewModel)DataContext).SaveCommand,
                Key.S, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(
                ((MainViewModel)DataContext).ToggleSidebarCommand,
                Key.B, ModifierKeys.Control));

            // Subscribe to sidebar changes
            var vm = (MainViewModel)DataContext;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsSidebarOpen))
                    AnimateSidebar(vm.IsSidebarOpen);
            };

            // Autosave on close
            Closing += (s, e) => vm.Save();

            // Autosave when window loses focus (user switches to another app)
            Deactivated += (s, e) => vm.Save();
        }

        private void AnimateSidebar(bool open)
        {
            var animation = new GridLengthAnimation
            {
                From = SidebarColumn.Width,
                To = open ? new GridLength(280) : new GridLength(0),
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
        }

        private void DragHandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is TimelineLayerViewModel layer)
            {
                _draggedLayer = layer;
                _dragStartPoint = e.GetPosition(this);

                var data = new DataObject(typeof(TimelineLayerViewModel), layer);
                DragDrop.DoDragDrop(fe, data, DragDropEffects.Move);

                _draggedLayer = null;
            }
        }

        private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is TimelineCellViewModel cell)
            {
                cell.ToggleCommand?.Execute(null);
            }
        }
    }

    /// <summary>
    /// Custom animation for GridLength (WPF doesn't support this natively)
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(GridLength);

        public GridLength From
        {
            get => (GridLength)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength To
        {
            get => (GridLength)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public IEasingFunction? EasingFunction
        {
            get => (IEasingFunction?)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(GridLengthAnimation));

        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double progress = animationClock.CurrentProgress ?? 0.0;

            if (EasingFunction != null)
                progress = EasingFunction.Ease(progress);

            double from = From.Value;
            double to = To.Value;

            double current = from + (to - from) * progress;
            return new GridLength(Math.Max(0, current));
        }
    }
}
