using BMM.Core.ViewModels.Base;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Fragments.Base
{
    public abstract class SwipeToRefreshFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : BaseViewModel, IMvxViewModel
    {
        private MvxSwipeRefreshLayout _swipeRefreshLayout;
        private AppBarLayout _appBarLayout;

        protected virtual MvxSwipeRefreshLayout SwipeRefreshLayout
            => _swipeRefreshLayout ??= View.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);

        protected virtual AppBarLayout AppBarLayout
            => _appBarLayout ??= View.FindViewById<AppBarLayout>(Resource.Id.appbar);

        protected override void AttachEvents()
        {
            base.AttachEvents();

            if (AppBarLayout != null && SwipeRefreshLayout != null)
                AppBarLayout.OffsetChanged += AppBarOnOffsetChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();

            if (AppBarLayout != null)
                AppBarLayout.OffsetChanged -= AppBarOnOffsetChanged;
        }

        private void AppBarOnOffsetChanged(object sender, AppBarLayout.OffsetChangedEventArgs e)
        {
            SwipeRefreshLayout.Enabled = e.VerticalOffset == 0;
        }
    }
}