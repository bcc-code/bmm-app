using _Microsoft.Android.Resource.Designer;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.Adapters.Swipes;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class PodcastContextHeaderRecyclerAdapter : BaseSwipeMenuAdapter
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
                case Resource.Layout.listitem_tiles_collection:
                    return new TilesCollectionViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_info_message:
                    return new InfoMessageViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_year_in_review_teaser_collapsed:
                case Resource.Layout.listitem_year_in_review_teaser_expanded:
                    return new YearInReviewTeaserViewHolder(view, itemBindingContext, this);
                case Resource.Layout.listitem_project_box_collapsed:
                    return new ProjectBoxViewHolder(view, itemBindingContext, this);
                case Resource.Layout.listitem_project_box_expanded:
                    return new ProjectBoxExpandedViewHolder(view, itemBindingContext, this);
                case Resource.Layout.listitem_gibraltar_project_box:
                    return new GibraltarProjectBoxExpandedViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_hvhe_project_box:
                    return new HvheProjectBoxViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_track:
                {
                    view.SetBackgroundColor(parent.Context.GetColorFromResource(ResourceConstant.Color.background_one_color));
                    return new TrackSwipeItemViewHolder(view, itemBindingContext, this);
                }
                default:
                    return new MvxRecyclerViewHolder(view, itemBindingContext);
            }
        }
    }
}