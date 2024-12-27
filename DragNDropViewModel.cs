using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public MyCommand SelectLayoutSettingCommand =>
            _selectLayoutSettingCommand ?? new MyCommand(
                (obj) =>
                {
                    if (obj == null || obj is not LayoutSetting layoutSetting)
                        return;
                    _selectedLayoutSetting = layoutSetting;
                    OnPropertyChanged(nameof(SelectedLayoutSetting));
                });
    }
}
