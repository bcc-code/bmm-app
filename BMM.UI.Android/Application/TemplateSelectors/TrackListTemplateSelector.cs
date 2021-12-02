namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class TrackListTemplateSelector : DocumentTemplateSelector
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == ViewTypes.Header)
                return Resource.Layout.listitem_tracklist_header;
            if (fromViewType == ViewTypes.SharedTrackCollectionHeader)
                return Resource.Layout.listitem_shared_trackcollection_header;
            if (fromViewType == ViewTypes.ChapterHeader)
                return Resource.Layout.listitem_chapter_header;

            return base.GetItemLayoutId(fromViewType);
        }
    }
}