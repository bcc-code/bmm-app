using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyRulesContentPO : BasePO, IBibleStudyRulesContentPO
{
    public BibleStudyRulesContentPO(string title, string text)
    {
        Title = title;
        Text = text;
    }
    
    public string Title { get; }
    public string Text { get; }
}