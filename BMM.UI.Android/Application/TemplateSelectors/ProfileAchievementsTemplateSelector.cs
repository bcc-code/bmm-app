using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors;

public class ProfileAchievementsTemplateSelector : MvxTemplateSelector<IBasePO>
{
    public override int GetItemLayoutId(int fromViewType) => fromViewType;

    protected override int SelectItemViewType(IBasePO forItemObject)
    {
        switch (forItemObject)
        {
            case ChapterHeaderPO:
                return Resource.Layout.listitem_chapter_header;
            case AchievementPO:
                return Resource.Layout.listitem_profile_achievement;
            default:
                return default;
        }
    }
}