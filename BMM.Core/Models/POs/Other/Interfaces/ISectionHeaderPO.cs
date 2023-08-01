using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Other.Interfaces;

public interface ISectionHeaderPO : IBasePO
{ 
    bool ShowDivider { get; }
    string Title { get; }
}