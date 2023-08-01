using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors;

public class BibleStudyTemplateSelector : MvxTemplateSelector<IBasePO>
{
    public override int GetItemLayoutId(int fromViewType)
    {
        return fromViewType;
    }

    protected override int SelectItemViewType(IBasePO forItemObject)
    {
        return forItemObject switch
        {
            IBibleStudyHeaderPO => Resource.Layout.listitem_bible_study_header,
            IBibleStudyProgressPO => Resource.Layout.listitem_bible_study_progress,
            ISectionHeaderPO => Resource.Layout.listitem_header,
            IExternalRelationListItemPO =>  Resource.Layout.listitem_trackrelation_external,
            ISelectableListContentItemPO => Resource.Layout.listitem,
            _ => default
        };
    }
}