using System;
using AndroidX.RecyclerView.Widget;
using BMM.Api.Framework;
using MvvmCross;

namespace BMM.UI.Droid.Application.Helpers
{
    public class MvxRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);

        public event LoadMoreEventHandler LoadMoreEvent;

        private readonly LinearLayoutManager _layoutManager;

        public MvxRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
        {
            _layoutManager = layoutManager;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter()?.ItemCount;
            var pastVisiblesItems = _layoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount)
                LoadMoreEvent?.Invoke(this, null);
        }
    }
}