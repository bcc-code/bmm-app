using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy;

public class HvheHeaderPO : BasePO
{
    public HvheHeaderPO(string leftItemLabel, string rightItemLabel)
    {
        LeftItemLabel = leftItemLabel;
        RightItemLabel = rightItemLabel;
    }
    
    public string LeftItemLabel { get; }
    public string RightItemLabel { get; }
}