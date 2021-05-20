namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class TrackCollectionTemplateSelector : DocumentTemplateSelector
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == ViewTypes.Header)
            {
                return Resource.Layout.listitem_tracklist_header;
            }

            return base.GetItemLayoutId(fromViewType);
        }
    }

    public class TrackListTemplateSelector : DocumentTemplateSelector
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == ViewTypes.Header)
            {
                return Resource.Layout.listitem_tracklist_header;
            }

            return base.GetItemLayoutId(fromViewType);
        }
    }
}

