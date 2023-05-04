using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.Tracks;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class HighlightedTextTemplateSelector : MvxTemplateSelector<DocumentPO>
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        protected override int SelectItemViewType(DocumentPO forItemObject)
        {
            return forItemObject switch
            {
                HighlightedTextPO => Resource.Layout.listitem_highlighted_text,
                HighlightedTextHeaderPO => Resource.Layout.listitem_highlighted_text_header,
                _ => default
            };
        }
    }
}