using System.ComponentModel;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Helpers;
using FFImageLoading;
using Google.Android.Material.AppBar;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.CuratedPlaylistFragment")]
    public class CuratedPlaylistFragment : BaseFragment<CuratedPlaylistViewModel>
    {
        public override void OnDestroy()
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            base.OnDestroy();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var propertyName = args.PropertyName;
            var allPropertiesHaveChanged = string.IsNullOrEmpty(propertyName);

            if (allPropertiesHaveChanged || args.PropertyName == "CuratedPlaylist")
            {
                if (ViewModel.CuratedPlaylist?.Cover == null)
                    return;

                Mvx.IoCProvider.Resolve<IExceptionHandler>()
                    .FireAndForgetWithoutUserMessages(async () =>
                    {
                        var drawable = await ImageService.Instance.LoadUrl(ViewModel.CuratedPlaylist.Cover).AsBitmapDrawableAsync();
                        var bitmap = drawable.Bitmap;
                        var newStatusbarColor = new Color(BitmapHelper.GetColor(bitmap));
                        FragmentBaseColor = newStatusbarColor;

                        await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                            .ExecuteOnMainThreadAsync(() => { SetStatusBarColor(newStatusbarColor); });
                    });
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar);

            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            InitRecyclerView(view);

            return view;
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;

        private void InitRecyclerView(View view)
        {
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
            if (recyclerView == null)
                return;
            recyclerView.Adapter = new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            recyclerView.HasFixedSize = true;

            var layoutManager = new LinearLayoutManager(ParentActivity);
            recyclerView.SetLayoutManager(layoutManager);
        }
    }
}