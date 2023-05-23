using BMM.Core.Models;
using BMM.Core.Models.POs.Base;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class TrackInfoTemplateSelector : MvxTemplateSelector<IListItem>
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            return fromViewType switch
            {
                TrackInfoViewTypes.SectionHeader => Resource.Layout.listitem_header,
                TrackInfoViewTypes.ExternalRelation => Resource.Layout.listitem_trackrelation_external,
                TrackInfoViewTypes.SelectableItem => Resource.Layout.listitem,
                _ => -1
            };
        }

        protected override int SelectItemViewType(IListItem forItemObject)
        {
            return forItemObject switch
            {
                SectionHeader _ => TrackInfoViewTypes.SectionHeader,
                ExternalRelationListItem _ => TrackInfoViewTypes.ExternalRelation,
                SelectableListItem _ => TrackInfoViewTypes.SelectableItem,
                _ => -1
            };
        }

        private static class TrackInfoViewTypes
        {
            public const int SectionHeader = 0;
            public const int ExternalRelation = 1;
            public const int SelectableItem = 2;
        }
    }
}