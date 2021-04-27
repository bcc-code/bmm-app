using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.TemplateSelectors;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    /// <summary>
    /// This adapter pretends that there is an additional item at the beginning which in turn can be used in a TemplateSelector to display a header
    /// </summary>
    public class HeaderRecyclerAdapter : LoadMoreRecyclerAdapter
    {
        protected virtual int NumberOfAdditionalItems => 1;

        public HeaderRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
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
                return ItemTemplateSelector.GetItemLayoutId(ViewTypes.Header);

            return base.GetItemViewType(position);
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
