using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    /// <summary>
    /// Always shows a spinner at the end of the list (and another class loads more items once that spinner is scrolled into view).
    /// </summary>
    public class LoadMoreRecyclerAdapter : MvxRecyclerAdapter
    {
        protected ILoadMoreDocumentsViewModel ViewModel => BindingContext.DataContext as ILoadMoreDocumentsViewModel;

        private bool ShouldDisplayBottomSpinner => ViewModel?.IsFullyLoaded == false;

        public LoadMoreRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        // This fixes a bug that hides the latest element of the list but causes problems with the scroll position
        //public override int ItemCount => base.ItemCount + (ShouldDisplayBottomSpinner ? 1 : 0);

        public override object GetItem(int position)
        {
            if (ShouldDisplayBottomSpinner && position + 1 == ItemCount)
                return null;
            else
                return base.GetItem(position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (ShouldDisplayBottomSpinner && position == ItemCount - 1)
            {
                ((IMvxRecyclerViewHolder) holder).DataContext = ViewModel;
            }
            else
            {
                base.OnBindViewHolder(holder, position);
            }
        }
    }
}
