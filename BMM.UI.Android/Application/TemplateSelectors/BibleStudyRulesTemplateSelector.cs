using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors;

public class BibleStudyRulesTemplateSelector : MvxTemplateSelector<IBasePO>
{
    public override int GetItemLayoutId(int fromViewType)
    {
        return fromViewType;
    }

    protected override int SelectItemViewType(IBasePO forItemObject)
    {
        switch (forItemObject)
        {
            case IBibleStudyRulesHeaderPO:
                return Resource.Layout.listitem_bible_study_rules_header;
            case IBibleStudyRulesContentPO:
                return Resource.Layout.listitem_bible_study_rules_content;
            default:
                return default;
        }
    }
}