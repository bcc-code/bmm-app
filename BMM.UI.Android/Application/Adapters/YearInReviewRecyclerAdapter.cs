using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class YearInReviewRecyclerAdapter : MvxRecyclerAdapter
    {
        private readonly double _itemWidth;
        private readonly double _itemHeight;
        private readonly double _imageWidth;
        private readonly double _imageHeight;

        public YearInReviewRecyclerAdapter(
            IMvxAndroidBindingContext bindingContext,
            double itemWidth,
            double itemHeight,
            double imageWidth,
            double imageHeight) : base(bindingContext)
        {
            _itemWidth = itemWidth;
            _itemHeight = itemHeight;
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);
            var vh = new YearInReviewItemViewHolder(
                view,
                itemBindingContext,
                _itemWidth,
                _itemHeight,
                _imageWidth,
                _imageHeight);
            vh.Update();
            return vh;
        }
    }
}