using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.TemplateSelectors;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class PodcastContextHeaderRecyclerAdapter : HeaderRecyclerAdapter
    {
        private readonly ExploreNewestViewModel _viewModel;

        protected override int NumberOfAdditionalItems => 3;

        public PodcastContextHeaderRecyclerAdapter(IMvxAndroidBindingContext bindingContext, ExploreNewestViewModel exploreNewestViewModel) : base(bindingContext)
        {
            _viewModel = exploreNewestViewModel;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);

            switch (viewType)
            {
                case Resource.Layout.listitem_playlists_collection:
                    return new PlaylistsCollectionViewHolder(view, itemBindingContext);
                default:
                    return new MvxRecyclerViewHolder(view, itemBindingContext);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (position == 0)
            {
                var wrapper = new CellWrapperViewModel<Document>(null, _viewModel);
                ((IMvxRecyclerViewHolder)holder).DataContext = wrapper;
            }
            else if (position == 1)
            {
                var wrapper = new CellWrapperViewModel<Document>(null, _viewModel);
                ((IMvxRecyclerViewHolder)holder).DataContext = wrapper;
            }
            else if (position == 2)
            {
                var wrapper = new CellWrapperViewModel<Document>(null, _viewModel);
                ((IMvxRecyclerViewHolder)holder).DataContext = wrapper;
            }
            else
            {
                base.OnBindViewHolder(holder, position);
            }
        }

        public override int GetItemViewType(int position)
        {
            if (position == 0)
                return ItemTemplateSelector.GetItemLayoutId(ViewTypes.LiveRadio);

            if (position == 1)
                return ItemTemplateSelector.GetItemLayoutId(ViewTypes.FraKaareTeaser);

            if (position == 2)
                return ItemTemplateSelector.GetItemLayoutId(ViewTypes.AslaksenTeaser);

            var result = base.GetItemViewType(position);
            return result;
        }
    }
}