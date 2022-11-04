using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class PodcastContextHeaderRecyclerAdapter : MvxRecyclerAdapter
    {
        private readonly RecyclerView.RecycledViewPool _commonRecyclerViewPool;

        public PodcastContextHeaderRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
            _commonRecyclerViewPool = new RecyclerView.RecycledViewPool();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);

            switch (viewType)
            {
                case Resource.Layout.listitem_covers_carousel_collection:
                {
                    var coversCarouselCollectionViewHolder = new CoversCarouselCollectionViewHolder(view, itemBindingContext)
                    {
                        RecycledViewPool = _commonRecyclerViewPool
                    };
                    return coversCarouselCollectionViewHolder;
                }
                case Resource.Layout.listitem_continue_listening_collection:
                    return new ContinueListeningCarouselCollectionViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_info_message:
                    return new InfoMessageViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_year_in_review_teaser_collapsed:
                case Resource.Layout.listitem_year_in_review_teaser_expanded:
                    return new YearInReviewTeaserViewHolder(view, itemBindingContext, this);
                default:
                    return new MvxRecyclerViewHolder(view, itemBindingContext);
            }
        }
    }
}