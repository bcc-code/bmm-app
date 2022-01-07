using BMM.Core.Models.POs;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class ChangeTrackLanguageTemplateSelector : IMvxTemplateSelector
    {
        public int GetItemViewType(object forItemObject) => forItemObject switch
        {
            HeaderPO _ => Resource.Layout.listitem_standard_header,
            StandardSelectablePO _ => Resource.Layout.listitem_standard_selectable,
            _ => int.MinValue
        };

        public int GetItemLayoutId(int fromViewType) => fromViewType;

        public int ItemTemplateId { get; set; }
    }
}