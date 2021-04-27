namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class PodcastTemplateSelector : DocumentTemplateSelector
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            switch (fromViewType)
            {
                case ViewTypes.Track:
                    return Resource.Layout.listitem_track;

                case ViewTypes.Header:
                    return Resource.Layout.listitem_podcast_header;

                case ViewTypes.ChapterHeader:
                    return Resource.Layout.listitem_chapter_header;

                default:
                    return base.GetItemLayoutId(fromViewType);
            }
        }
    }
}

