using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tiles;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class TilesCollectionTemplateSelector : MvxTemplateSelector<DocumentPO>
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        protected override int SelectItemViewType(DocumentPO forItemObject)
        {
            return forItemObject switch
            {
                ContinueListeningTilePO => Resource.Layout.listitem_continue_listening_tile,
                MessageTilePO => Resource.Layout.listitem_message_tile,
                VideoTilePO => Resource.Layout.listitem_video_tile,
                _ => default
            };
        }
    }
}