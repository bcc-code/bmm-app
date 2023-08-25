using BMM.Core.Models.POs.Base;
using BMM.Core.Models.Themes;

namespace BMM.Core.Models.POs
{
    public class ColorThemePO : BasePO
    {
        private bool _isSelected;

        public ColorThemePO(ColorTheme colorTheme, string label)
        {
            ColorTheme = colorTheme;
            Label = label;
        }

        public ColorTheme ColorTheme { get; }

        public string Label { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}