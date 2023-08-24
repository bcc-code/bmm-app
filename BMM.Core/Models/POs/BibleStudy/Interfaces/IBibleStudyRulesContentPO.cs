using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyRulesContentPO : IBasePO
{
    string Title { get; }
    string Text { get; }
}