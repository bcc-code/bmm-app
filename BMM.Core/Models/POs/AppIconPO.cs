using BMM.Core.Models.Enums;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs;

public class AppIconPO : BasePO
{
    private bool _isSelected;

    public AppIconPO(AppIconType appIconType, string name, string imagePath)
    {
        AppIconType = appIconType;
        Name = name;
        ImagePath = imagePath;
    }
    
    public AppIconType AppIconType { get; }
    public string Name { get; }
    public string ImagePath { get; }
    
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}