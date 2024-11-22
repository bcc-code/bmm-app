using System;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Commands;
using Newtonsoft.Json.Serialization;

namespace BMM.Core.Models.POs.Other
{
    public class CheckboxListItemPO : BasePO, ICheckboxListItemPO
    {
        public CheckboxListItemPO()
        {
            OnSelected = new MvxCommand(() => IsChecked = !IsChecked);
        }

        public string Key { get; set; }
        
        public string Title { get; set; }

        public string Text { get; set; }

        private bool _isChecked;

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

        public IMvxCommand OnSelected { get; set; }

        public Action<CheckboxListItemPO> OnChanged { get; set; }
    }
}