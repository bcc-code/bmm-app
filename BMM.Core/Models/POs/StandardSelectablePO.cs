using BMM.Core.Models.POs.Base;
using BMM.Core.Models.Themes;

namespace BMM.Core.Models.POs
{
    public class StandardSelectablePO : BasePO
    {
        private bool _isSelected;

        public StandardSelectablePO(string label, string value = null)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; }
        public string Value { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}