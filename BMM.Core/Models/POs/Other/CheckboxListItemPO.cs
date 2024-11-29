using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other
{
    public class CheckboxListItemPO : BasePO, ICheckboxListItemPO
    {
        public CheckboxListItemPO()
        {
            OnSelected = new MvxCommand(() =>
            {
                if (IsEnabled)
                    IsChecked = !IsChecked;
            });
            IsEnabled = true;
        }

        public string Key { get; set; }
        
        public string Title { get; set; }

        public string Text { get; set; }

        private bool _isChecked;
        private bool _isEnabled;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (!SetProperty(ref _isChecked, value))
                    return;
                
                RaisePropertyChanged();
                OnChanged?.Invoke(this);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public IMvxCommand OnSelected { get; set; }

        public Action<CheckboxListItemPO> OnChanged { get; set; }
    }
}