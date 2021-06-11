using System;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.Models
{
    public class CheckboxListItem: MvxNotifyPropertyChanged, ICheckboxListItem
    {
        public CheckboxListItem()
        {
            OnSelected = new MvxCommand(() => IsChecked = !IsChecked);
        }

        public string Title { get; set; }

        public string Text { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
                OnChanged?.Invoke(value);
            }
        }

        public IMvxCommand OnSelected { get; set; }

        public Action<bool> OnChanged { get; set; }
    }
}