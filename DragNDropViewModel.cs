using System.Collections.ObjectModel;

namespace DragNDropTask
{
    internal class DragNDropViewModel : ViewModelBase
    {
        private LayoutSetting? _selectedLayoutSetting;
        private MyCommand? _selectLayoutSettingCommand;

        public DragNDropViewModel()
        {
            LayoutSettings = Extensions.CreateDefaultLayouts();
            ItemsSource = Extensions.CreateDefaultItemsSource();
        }

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
    }
}
