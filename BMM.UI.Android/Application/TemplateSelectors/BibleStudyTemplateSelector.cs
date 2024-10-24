using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;
using Microsoft.IdentityModel.Tokens;
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
                if (bibleStudyExternalRelationPO.HasQuestion)
                    return Resource.Layout.listitem_extrenal_relations_quiz_button;

                if (bibleStudyExternalRelationPO.WillPlayTrack)
                {
                    return bibleStudyExternalRelationPO.Subtitle.IsNullOrEmpty()
                        ? Resource.Layout.listitem_extrenal_relations_play_button
                        : Resource.Layout.listitem_extrenal_relations_play_button_with_subtitle;
                }

                return bibleStudyExternalRelationPO.Subtitle.IsNullOrEmpty()
                    ? Resource.Layout.listitem_extrenal_relations_open_button
                    : Resource.Layout.listitem_extrenal_relations_open_button_with_subtitle;
            }
            default:
                return default;
        }
    }
}