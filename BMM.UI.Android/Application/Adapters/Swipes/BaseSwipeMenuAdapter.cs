using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.Adapters.Swipes
{
    public class BaseSwipeMenuAdapter : MvxRecyclerAdapter, ISwipeMenuAdapter,  View.IOnScrollChangeListener
    {
        private readonly IList<SwipeMenuViewHolder> _visibleViewHolders = new List<SwipeMenuViewHolder>();
        private RecyclerView _recyclerView;

        public BaseSwipeMenuAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext){}

        public SwipeMenuViewHolder GetViewHolder(View view)
            => _visibleViewHolders.FirstOrDefault(x => x.ItemView == view);

        public void SetActiveMenu(SwipeMenuViewHolder swipeMenuViewHolder)
        {
            ResetMenusOnVisibleViewHolders(swipeMenuViewHolder);
        }

        public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            ResetMenusOnVisibleViewHolders();
        }

        public override void OnViewRecycled(Object holder)
        {
            base.OnViewRecycled(holder);
            if (holder is SwipeMenuViewHolder swipeViewHolder)
                _visibleViewHolders.Remove(swipeViewHolder);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);

            if (holder is SwipeMenuViewHolder swipeViewHolder)
                _visibleViewHolders.Add(swipeViewHolder);
        }

        public void ResetMenusOnVisibleViewHolders(SwipeMenuViewHolder active = null)
        {
            if (_recyclerView?.GetLayoutManager() is MvxGuardedLinearLayoutManager linearLayoutManager)
            {
                int first = linearLayoutManager.FindFirstVisibleItemPosition();
                int last = linearLayoutManager.FindLastVisibleItemPosition();
                for (int i = first; i <= last; i++)
                {
                    if (_recyclerView.FindViewHolderForAdapterPosition(i) is SwipeMenuViewHolder swipeMenuViewHolder
                        && swipeMenuViewHolder != active)
                        swipeMenuViewHolder.HideMenusIfNeeded();

                }
            }
            else
            {
                foreach (var viewHolder in _visibleViewHolders.Where(x => x!= active))
                    viewHolder.HideMenusIfNeeded();
            }
        }

        public override void OnViewDetachedFromWindow(Object holder)
        {
            if (holder is SwipeMenuViewHolder swipeMenuViewHolder
                && _visibleViewHolders.Contains(swipeMenuViewHolder))
                _visibleViewHolders.Remove(swipeMenuViewHolder);

            base.OnViewDetachedFromWindow(holder);
        }

        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            _recyclerView = recyclerView;
        }

        public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
        {
            _visibleViewHolders.Clear();
            _recyclerView = null;
            base.OnDetachedFromRecyclerView(recyclerView);
        }
    }
}
