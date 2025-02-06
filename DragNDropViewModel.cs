using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DragNDropTask
{
    internal class DragNDropViewModel : ViewModelBase
    {
        private LayoutSetting? _selectedLayoutSetting;
        private MyCommand? _selectLayoutSettingCommand;
        private MyCommand? _changePositionIndexesCommand;

        public DragNDropViewModel()
        {
            LayoutSettings = Extensions.CreateDefaultLayouts();
            ItemsSource = Extensions.CreateDefaultItemsSource();
            //ItemsSource.CollectionChanged += ((sender, args) => MessageBox.Show("Resource changed"));
        }

        public StackPanel MyStackPanel { get; set; }
        public ObservableCollection<WidgetViewModel> ItemsSource { get; set; } 
        public ObservableCollection<LayoutSetting> LayoutSettings { get; set; }

        public LayoutSetting? SelectedLayoutSetting
        {
            get => _selectedLayoutSetting;
            set
            {
                if (Equals(value, _selectedLayoutSetting)) return;
                _selectedLayoutSetting = value;
                OnPropertyChanged();
            }
        }

        public MyCommand SelectLayoutSettingCommand
        {
            get
            {

                _selectLayoutSettingCommand ??= new MyCommand(
                    (obj) =>
                    {
                        if (obj is not LayoutSetting layoutSetting)
                        {
                            return;
                        }
                        _selectedLayoutSetting = layoutSetting;
                        OnPropertyChanged(nameof(SelectedLayoutSetting));
                    });
                return _selectLayoutSettingCommand;
            }
        }

        public MyCommand ChangePositionIndexesCommand
        {
            get
            {
                _changePositionIndexesCommand ??= new MyCommand(
                    (obj) =>
                    {
                        if (obj is not PositionIndexesPair pair ||
                            pair.SourcePosition < 0 || pair.SourcePosition >= ItemsSource.Count ||
                            pair.TargetPosition < 0 || pair.TargetPosition >= ItemsSource.Count ||
                            pair.SourcePosition == pair.TargetPosition)
                        {
                            return;
                        }

                        (ItemsSource[pair.SourcePosition].PosIndex, ItemsSource[pair.TargetPosition].PosIndex)
                            = (ItemsSource[pair.TargetPosition].PosIndex, ItemsSource[pair.SourcePosition].PosIndex);
                    }
                    );
                return _changePositionIndexesCommand;
            }
        }

        public MyCommand PositionsSwappedHandlingCommand =>
            new(
                (obj) =>
                {
                    if (obj is PositionsSwappedRoutedEventArgs eventArgs)
                    {
                        //MessageBox.Show(
                        //    $"Dropping with event args {eventArgs.Positions.SourcePosition} and {eventArgs.Positions.TargetPosition}");
                        return;
                    }
                    //MessageBox.Show("Dropping WITHOUT event args");
                });

        public MyCommand AddRectangleToItemsSourceCommand
            => new
            ((obj) =>
            {
                ItemsSource.Add(new WidgetViewModel()
                {
                    PosIndex = ItemsSource.Max(wvm => wvm.PosIndex + 1),
                    Content = Extensions.CreateRandomFilledRectangle()
                });
            });
    }
}
