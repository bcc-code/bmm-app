using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class CoversCarouselRecyclerAdapter : MvxRecyclerAdapter
    {
        private readonly MvxRecyclerView _recyclerView;
        private readonly int _itemsPerRow;
        private readonly int _itemSpacing;

        public CoversCarouselRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public CoversCarouselRecyclerAdapter(
            IMvxAndroidBindingContext bindingContext,
            MvxRecyclerView recyclerView,
            int itemsPerRow,
            int itemSpacing) : base(bindingContext)
        {
            _recyclerView = recyclerView;
            _itemsPerRow = itemsPerRow;
            _itemSpacing = itemSpacing;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);

            switch (viewType)
            {
                case Resource.Layout.listitem_cover_with_title:
                    return new CoverWithTitleViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_cover_with_title_flexible:
                {
                    //int itemSize = (_recyclerView.Width - _itemSpacing * (_itemsPerRow - 1)) / _itemsPerRow;
                    int itemSize = _recyclerView.Width / _itemsPerRow - _itemSpacing / 2;
                    var vh = new CoverWithTitleViewHolder(view, itemBindingContext, itemSize);
                    vh.Update();
                    return vh;
                }
                default:
                    return new MvxRecyclerViewHolder(view, itemBindingContext);
            }
        }
    }
}