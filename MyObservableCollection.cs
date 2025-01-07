using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragNDropTask
{
    internal sealed class MyObservableCollection<T> : ObservableCollection<T>
    {
        public MyObservableCollection()
        {
            CollectionChanged += OnItemsSourceCollectionChanged;
        }

        private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged castedItem)
                    {
                        castedItem.PropertyChanged += OnItemPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged castedItem)
                    {
                        castedItem.PropertyChanged -= OnItemPropertyChanged;
                    }
                }
            }
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var ea = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, e.PropertyName);
            OnCollectionChanged(ea);
        }
    }
}
