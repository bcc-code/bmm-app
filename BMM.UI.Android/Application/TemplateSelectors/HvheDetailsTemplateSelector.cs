using _Microsoft.Android.Resource.Designer;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors;

public class HvheDetailsTemplateSelector : MvxTemplateSelector<IBasePO>
{
    public override int GetItemLayoutId(int fromViewType)
    {
        return fromViewType;
    }

    protected override int SelectItemViewType(IBasePO forItemObject)
    {
        return forItemObject switch
        {
            HvheBoysVsGirlsPO => ResourceConstant.Layout.listitem_boys_vs_girls,
            HvheChurchesSelectorPO => ResourceConstant.Layout.listitem_churches_selector,
            HvheHeaderPO => ResourceConstant.Layout.listitem_hvhe_header,
            HvheChurchPO hvheChurchPO when hvheChurchPO.Church.IsHighlighted => ResourceConstant.Layout.listitem_highlighted_church,
            HvheChurchPO => ResourceConstant.Layout.listitem_standard_church,
            _ => default
        };
    }
}