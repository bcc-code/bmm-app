using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Other.Interfaces;

namespace BMM.Core.Models.POs.Other;

public class SectionHeaderPO : BasePO, ISectionHeaderPO, IListItem
{
    public SectionHeaderPO(string title, bool showDivider = true)
    {
        ShowDivider = showDivider;
        Title = title;
    }
    
    public bool ShowDivider { get; }
    public string Title { get; set; }
}