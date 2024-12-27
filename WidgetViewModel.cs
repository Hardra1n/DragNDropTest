﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragNDropTask
{
    internal class WidgetViewModel : ViewModelBase
    {
        private int _posIndex;
        private object _content;

        public int PosIndex
        {
            get => _posIndex;
            set
            {
                if (value == _posIndex) return;
                _posIndex = value;
                OnPropertyChanged();
            }
        }

        public object Content
        {
            get => _content;
            set
            {
                if (Equals(value, _content)) return;
                _content = value;
                OnPropertyChanged();
            }
        }
    }
}