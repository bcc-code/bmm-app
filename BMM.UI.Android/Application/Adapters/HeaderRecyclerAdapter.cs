using _Microsoft.Android.Resource.Designer;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.TemplateSelectors;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    /// <summary>
    /// This adapter pretends that there is an additional item at the beginning which in turn can be used in a TemplateSelector to display a header
    /// </summary>
    public class HeaderRecyclerAdapter : SwipesLoadMoreRecyclerAdapter
    {
        private readonly int _headerViewType;

        protected virtual int NumberOfAdditionalItems => 1;

        public HeaderRecyclerAdapter(IMvxAndroidBindingContext bindingContext, int? headerViewType = null) : base(bindingContext)
        {
            _headerViewType = headerViewType ?? ViewTypes.Header;
        }

        public override int ItemCount => base.ItemCount + NumberOfAdditionalItems;

        protected override int GetViewPosition(int itemsSourcePosition)
        {
            return base.GetViewPosition(itemsSourcePosition) + NumberOfAdditionalItems;
        }

        protected override int GetItemsSourcePosition(int viewPosition)
        {
            return base.GetItemsSourcePosition(viewPosition) - NumberOfAdditionalItems;
        }

        public override int GetItemViewType(int position)
        {
            if (position == 0)
                return ItemTemplateSelector.GetItemLayoutId(_headerViewType);

            return base.GetItemViewType(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);
            view.SetBackgroundColor(parent.Context.GetColorFromResource(ResourceConstant.Color.background_one_color));
            return new TrackSwipeItemViewHolder(view, itemBindingContext, this);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (position == 0)
            {
                ((IMvxRecyclerViewHolder)holder).DataContext = BindingContext.DataContext;
            }
            else
            {
                base.OnBindViewHolder(holder, position);
            }
        }
    }
}
