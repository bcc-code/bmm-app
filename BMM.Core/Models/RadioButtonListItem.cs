using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.Models
{
    public class RadioButtonListItem : MvxNotifyPropertyChanged, IListContentItem
    {
        private bool _isSelected;

        public RadioButtonListItem()
        {
            OnSelected = new MvxCommand(() => IsSelected = !IsSelected);
        }

        public string Title { get; set; }

        public string Text { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }

        public IMvxCommand OnSelected { get; set; }
    }
}
