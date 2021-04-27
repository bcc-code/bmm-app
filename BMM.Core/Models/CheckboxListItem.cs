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
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        public IMvxCommand OnSelected { get; set; }
    }
}