namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class TrackCollectionTemplateSelector : DocumentTemplateSelector
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == ViewTypes.Header)
            {
                return Resource.Layout.listitem_trackcollection_header;
            }

            return base.GetItemLayoutId(fromViewType);
        }
    }
}

