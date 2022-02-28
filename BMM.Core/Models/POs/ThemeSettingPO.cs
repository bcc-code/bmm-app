using BMM.Core.Models.POs.Base;
using BMM.Core.Models.Themes;

namespace BMM.Core.Models.POs
{
    public class ThemeSettingPO : BasePO
    {
        private bool _isSelected;

        public ThemeSettingPO(Theme theme, string label)
        {
            Theme = theme;
            Label = label;
        }

        public Theme Theme { get; }

        public string Label { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}