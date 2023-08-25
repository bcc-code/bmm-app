using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyRulesHeaderPO : IBasePO
{
    string HeaderText { get; }
}