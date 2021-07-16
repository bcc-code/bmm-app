using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Activities;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross.ViewModels;
using System.Linq;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Implementations.Analytics;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using Google.Android.Material.AppBar;
using MvvmCross;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace BMM.UI.Droid.Application.Fragments
{
    public abstract class BaseFragment : MvxFragment
    {
        protected ISdkVersionHelper SdkVersion = Mvx.IoCProvider.Resolve<ISdkVersionHelper>();

        public CollapsingToolbarLayout CollapsingToolbar;

        public Toolbar Toolbar { get; protected set; }


        protected abstract int FragmentId { get; }
        protected virtual bool IsTabBarVisible => true;

        private Color? _fragmentBaseColor;

        protected virtual Color FragmentBaseColor
        {
            get
            {
                if (_fragmentBaseColor.HasValue)
                {
                    return _fragmentBaseColor.Value;
                }

                if (CheckIfFragmentIsDetachedFromActivity())
                {
                    Mvx.IoCProvider.Resolve<IAnalytics>().LogEvent("FragmentBaseColor, fragment is detached but accessed anyway");
                    return new Color();
                }

                return new Color(ContextCompat.GetColor(Activity.BaseContext, Resource.Color.white));
            }
            set => _fragmentBaseColor = value;
        }

        protected virtual Color ActionBarColor => new Color(ContextCompat.GetColor(Activity.BaseContext, Resource.Color.white));

        public MainActivity ParentActivity => (MainActivity)Activity;

        protected virtual string Title => string.Empty;

        protected BaseFragment()
        {
            RetainInstance = true;
        }

        public override void OnPause()
        {
            base.OnPause();

            if (Activity is MainActivity)
            {
                ParentActivity.HideKeyboard();
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            (Activity as MainActivity)?.SetBottomBarVisibility(IsTabBarVisible.ToViewState());
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(FragmentId, null);

            Toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            InitToolbar();

            CollapsingToolbar = view.FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);

            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
            InitRecyclerView(recyclerView);
            SetStatusBarColor(ColorOfUppermostFragment());

            return view;
        }

        protected virtual void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            if (recyclerView == null)
            {
                return;
            }

            recyclerView.HasFixedSize = true;

            var layoutManager = new LinearLayoutManager(ParentActivity);

            if (ViewModel is ILoadMoreDocumentsViewModel)
            {
                recyclerView.Adapter = CreateAdapter();

                var onScrollListener = new MvxRecyclerViewOnScrollListener(layoutManager);
                onScrollListener.LoadMoreEvent += (sender, e) =>
                {
                    var vm = (ILoadMoreDocumentsViewModel) ViewModel;
                    if (vm != null && vm.IsInitialized && !vm.IsLoading && !vm.IsFullyLoaded)
                    {
                        vm.LoadMoreCommand.Execute();
                    }
                };

                recyclerView.AddOnScrollListener(onScrollListener);
            }
            else if (ViewModel is ITrackListViewModel)
            {
                recyclerView.Adapter = CreateAdapter();
            }

            recyclerView.SetLayoutManager(layoutManager);
        }

        protected virtual MvxRecyclerAdapter CreateAdapter()
        {
            return new LoadMoreRecyclerAdapter((IMvxAndroidBindingContext) BindingContext);
        }

        protected void InitToolbar()
        {
            if (Toolbar == null)
            {
                return;
            }

            ParentActivity.SetSupportActionBar(Toolbar);

            if (ShouldDisplayUpButton())
            {
                ParentActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var isHomeButton = item.ItemId == Android.Resource.Id.Home;

            if (IsNestedView() && isHomeButton)
            {
                Activity.SupportFragmentManager.PopBackStackImmediate();
            }

            return base.OnOptionsItemSelected(item);
        }

        private bool ShouldDisplayUpButton()
        {
            var isQueue = this is QueueFragment;

            return IsNestedView() && !isQueue;
        }

        private bool IsNestedView()
        {
            return Activity?.SupportFragmentManager?.BackStackEntryCount > 1;
        }

        public void SetStatusBarColor(Color color)
        {
            if (CheckIfFragmentIsDetachedFromActivity())
            {
                Mvx.IoCProvider.Resolve<IAnalytics>().LogEvent("SetStatusBarColor, fragment is detached but accessed anyway");
                return;
            }

            var darkenedStatusBarColor = BitmapHelper.Darken(color);

            if (Toolbar != null)
            {
                ParentActivity.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(ActionBarColor));
            }

            CollapsingToolbar?.SetContentScrimColor(color);
            var toolbarTextColor = BitmapHelper.BackgroundColorRequiresDarkText(color) ? Color.Black : Color.White;
            CollapsingToolbar?.SetExpandedTitleColor(toolbarTextColor);
            CollapsingToolbar?.SetCollapsedTitleTextColor(toolbarTextColor);

            var window = Activity.Window;
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.SetStatusBarColor(darkenedStatusBarColor);

            window.DecorView.SystemUiVisibility = BitmapHelper.BackgroundColorRequiresDarkText(darkenedStatusBarColor)
                ? window.DecorView.SystemUiVisibility.AddFlag(SystemUiFlags.LightStatusBar)
                : window.DecorView.SystemUiVisibility.RemoveFlag(SystemUiFlags.LightStatusBar);

        }

        protected virtual bool CheckIfFragmentIsDetachedFromActivity() => Activity?.BaseContext == null || Context == null;

        protected Color ColorOfUppermostFragment()
        {
            if (CheckIfFragmentIsDetachedFromActivity())
            {
                Mvx.IoCProvider.Resolve<IAnalytics>().LogEvent("ColorOfUppermostFragment, fragment is detached but accessed anyway, or sdk not support custom status bar color");
                return new Color();
            }

            var fragmentTagsToIgnore = new[]
            {
                "Player_Fragment",
                "BMM.Core.ViewModels.PlayerViewModel",
                "BMM.Core.ViewModels.MiniPlayerViewModel",
                "Queue_Fragment"
            };

            var fragments = FragmentManager.Fragments;

            var supportedFragments = fragments.Where(fragment =>
            {
                var hasTag = !string.IsNullOrEmpty(fragment?.Tag);
                var shouldBeIgnored = !fragmentTagsToIgnore.Contains(fragment?.Tag);

                return hasTag && shouldBeIgnored;
            });

            var uppermostFragment = supportedFragments.LastOrDefault() as BaseFragment;
            var color = uppermostFragment?.FragmentBaseColor ?? FragmentBaseColor;

            return color;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            menu.Clear();
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            if (!string.IsNullOrEmpty(Title) && Toolbar != null)
                ParentActivity.SupportActionBar.Title = Title;
        }
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : BaseViewModel, IMvxViewModel
    {
        public new TViewModel ViewModel
        {
            get => (TViewModel)base.ViewModel;
            set {
                base.ViewModel = value;

                RegisterViewModelPropertyChangedListener();
            }
        }

        protected override string Title => ViewModel.TextSource.GetText("Title");

        private void RegisterViewModelPropertyChangedListener()
        {
            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "TextSource")
                {
                    if (ParentActivity != null)
                    {
                        if (!string.IsNullOrEmpty(Title) && Toolbar != null)
                            ParentActivity.SupportActionBar.Title = Title;
                        else if (Toolbar != null)
                            ParentActivity.SupportActionBar.Title = "";
                    }
                }
            };
        }
    }
}