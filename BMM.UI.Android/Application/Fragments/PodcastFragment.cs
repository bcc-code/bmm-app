using System.ComponentModel;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
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
    [Register("bmm.ui.droid.application.fragments.PodcastFragment")]
    public class PodcastFragment : BaseFragment<PodcastViewModel>
    {
        private string _toolbarTitle;

        public string ToolbarTitle
        {
            get
            {
                return _toolbarTitle;
            }
            set
            {
                _toolbarTitle = value;
                CollapsingToolbar.SetExpandedTitleColor(Android.Resource.Color.Transparent);
                CollapsingToolbar.SetTitle(_toolbarTitle);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            var set = this.CreateBindingSet<PodcastFragment, PodcastViewModel>();
            set.Bind().For(sa => sa.ToolbarTitle).To(vm => vm.Podcast.Title);
            set.Apply();
        }

        public override void OnDestroy()
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            base.OnDestroy();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var propertyName = args.PropertyName;
            var allPropertiesHaveChanged = string.IsNullOrEmpty(propertyName);

            if (allPropertiesHaveChanged || args.PropertyName == "Podcast")
            {
                if (ViewModel.Podcast?.Cover == null)
                    return;

                Mvx.IoCProvider.Resolve<IExceptionHandler>()
                    .FireAndForgetWithoutUserMessages(async () =>
                    {
                        var drawable = await ImageService.Instance.LoadUrl(ViewModel.Podcast.Cover).AsBitmapDrawableAsync();
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

            return view;
        }

        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.podcast, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("ManageDownloads"));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_podcast_manage_downloads:
                    ViewModel.OptionCommand.Execute(ViewModel.Podcast);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override Color ActionBarColor => new Color(ContextCompat.GetColor(Context, Android.Resource.Color.Transparent));

        protected override int FragmentId => Resource.Layout.fragment_podcast;
    }
}