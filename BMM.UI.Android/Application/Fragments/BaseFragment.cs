using System;
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
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;
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
        protected virtual bool ShouldClearMenuItemsAtStart => true;

        private Color? _fragmentBaseColor;
        private string _title;
        private MvxRecyclerViewOnScrollListener _onScrollListener;

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

                return new Color(Activity.GetColorFromResource(Resource.Color.background_primary_color));
            }
            set => _fragmentBaseColor = value;
        }

        public MainActivity ParentActivity => (MainActivity)Activity;

        protected MvxRecyclerView RecyclerView { get; private set; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                SetTitle();
            }
        }

        protected BaseFragment()
        {
            RetainInstance = true;
        }

        public override void OnPause()
        {
            base.OnPause();
            DetachEvents();

            if (Activity is MainActivity)
                ParentActivity.HideKeyboard();
        }

        public override void OnResume()
        {
            base.OnResume();
            AttachEvents();
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

            RecyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
            InitRecyclerView(RecyclerView);
            SetStatusBarColor(ColorOfUppermostFragment());
            Bind();

            return view;
        }

        protected virtual void AttachEvents()
        {
            if (_onScrollListener == null)
                return;

            _onScrollListener.LoadMoreEvent += OnScrollListenerOnLoadMoreEvent;
        }

        protected virtual void DetachEvents()
        {
            if (_onScrollListener == null)
                return;

            _onScrollListener.LoadMoreEvent -= OnScrollListenerOnLoadMoreEvent;
        }

        protected virtual void Bind()
        {
        }

        protected virtual void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            if (recyclerView == null)
                return;

            recyclerView.HasFixedSize = true;

            var layoutManager = new LinearLayoutManager(ParentActivity);

            if (ViewModel is ILoadMoreDocumentsViewModel)
            {
                recyclerView.Adapter = CreateAdapter();
                _onScrollListener = new MvxRecyclerViewOnScrollListener(layoutManager);
                recyclerView.AddOnScrollListener(_onScrollListener);
            }
            else if (ViewModel is ITrackListViewModel)
            {
                recyclerView.Adapter = CreateAdapter();
            }

            recyclerView.SetLayoutManager(layoutManager);
        }

        private void OnScrollListenerOnLoadMoreEvent(object sender, EventArgs e)
        {
            var vm = (ILoadMoreDocumentsViewModel) ViewModel;
            if (vm != null && vm.IsInitialized && !vm.IsLoading && !vm.IsFullyLoaded)
            {
                vm.LoadMoreCommand.Execute();
            }
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

            var window = Activity.Window;
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.SetStatusBarColor(color);

            window.DecorView.SystemUiVisibility = color.GetStatusBArVisibilityBasedOnColor(window);
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
            if (ShouldClearMenuItemsAtStart)
                menu.Clear();

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            SetTitle();
        }

        private void SetTitle()
        {
            bool shouldUpdateTitle = !string.IsNullOrEmpty(Title)
                                     && Toolbar != null
                                     && ParentActivity.SupportActionBar.Title != Title;

            if (!shouldUpdateTitle)
                return;

            ParentActivity.SupportActionBar.Title = Title;

            if (CollapsingToolbar != null)
                CollapsingToolbar.Title = Title;
        }
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : BaseViewModel, IMvxViewModel
    {
        protected virtual bool HasCustomTitle => false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            if (!HasCustomTitle)
                Title = ViewModel.TextSource[ViewModelUtils.GetVMTitleKey(ViewModel.GetType())];

            return view;
        }

        public new TViewModel ViewModel => (TViewModel)base.ViewModel;
    }
}