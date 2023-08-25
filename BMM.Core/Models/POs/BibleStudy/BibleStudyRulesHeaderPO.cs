using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyRulesHeaderPO : BasePO, IBibleStudyRulesHeaderPO
{
    public BibleStudyRulesHeaderPO(string headerText)
    {
        HeaderText = headerText;
    }
    
    public string HeaderText { get; }
}