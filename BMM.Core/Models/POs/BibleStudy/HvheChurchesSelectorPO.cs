using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy;

public class HvheChurchesSelectorPO : BasePO
{
    private bool _isLeftItemSelected;
    private bool _isRightItemSelected;

    public HvheChurchesSelectorPO(
        string leftItemTitle,
        string rightItemTitle,
        IList<IBasePO> churches)
    {
        LeftItemTitle = leftItemTitle;
        RightItemTitle = rightItemTitle;
        Churches = churches;

        LeftItemSelectedCommand = new MvxCommand(() =>
        {
            IsLeftItemSelected = true;
        });
        
        RightItemSelectedCommand = new MvxCommand(() =>
        {
            IsRightItemSelected = true;
        });

        IsLeftItemSelected = true;
    }

    public bool IsLeftItemSelected
    {
        get => _isLeftItemSelected;
        set
        {
            if (SetProperty(ref _isLeftItemSelected, value) && value)
                IsRightItemSelected = false;
        }
    }
    
    public bool IsRightItemSelected
    {
        get => _isRightItemSelected;
        set
        {
            if (SetProperty(ref _isRightItemSelected, value) && value)
                IsLeftItemSelected = false;
        }
    }

    public string LeftItemTitle { get; }
    public string RightItemTitle { get; }
    public IList<IBasePO> Churches { get; }
    public IMvxCommand LeftItemSelectedCommand { get; }
    public IMvxCommand RightItemSelectedCommand { get; }
}