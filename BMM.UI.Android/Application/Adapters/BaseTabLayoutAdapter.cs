using MvvmCross.Binding.Extensions;
using MvvmCross.DroidX.RecyclerView;

namespace BMM.UI.Droid.Application.Adapters
{
    public abstract class BaseTabLayoutAdapter : MvxRecyclerAdapter
    {
        public override int GetItemViewType(int position)
        {
            return Resource.Layout.listitem_tab_layout;
        }

        public override object GetItem(int viewPosition)
        {
            return ItemsSource?.ElementAt(viewPosition);
        }
    }
}