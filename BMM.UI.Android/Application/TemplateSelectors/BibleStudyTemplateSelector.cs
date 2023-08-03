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
        switch (forItemObject)
        {
            case IBibleStudyHeaderPO:
                return Resource.Layout.listitem_bible_study_header;
            case IBibleStudyProgressPO:
                return Resource.Layout.listitem_bible_study_progress;
            case ISectionHeaderPO:
                return Resource.Layout.listitem_header;
            case IExternalRelationListItemPO:
                return Resource.Layout.listitem_trackrelation_external;
            case ISelectableListContentItemPO:
                return Resource.Layout.listitem;
            case IBibleStudyExternalRelationPO bibleStudyExternalRelationPO:
            {
                if (bibleStudyExternalRelationPO.WillPlayTrack)
                    return Resource.Layout.listitem_extrenal_relations_play_button;
                
                return Resource.Layout.listitem_extrenal_relations_open_button;
            }
            default:
                return default;
        }
    }
}