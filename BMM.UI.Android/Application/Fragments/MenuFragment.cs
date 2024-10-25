using System.ComponentModel;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.Implementations.Badge;
using BMM.Core.Messages;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Plugin.Messenger;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.bottom_navigation_frame)]
    [Register("bmm.ui.droid.application.fragments.MenuFragment")]
    public class MenuFragment : BaseFragment<MenuViewModel>, NavigationBarView.IOnItemSelectedListener
    {
        private IBadgeService _badgeService;
        private IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
        
        private BottomNavigationView _navigationView;
        private View _badgeView;
        private FrameLayout _iconContainer;

        public MenuFragment()
        {
            _badgeService = Mvx.IoCProvider!.Resolve<IBadgeService>();
            _mvxMainThreadAsyncDispatcher = Mvx.IoCProvider!.Resolve<IMvxMainThreadAsyncDispatcher>();
            _badgeService!.BadgeChanged += BadgeServiceOnBadgeChanged;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _badgeService.BadgeChanged -= BadgeServiceOnBadgeChanged;
        }

        protected override int FragmentId => Resource.Layout.fragment_menu;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_menu, null);

            _navigationView = view.FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            _navigationView!.SetOnItemSelectedListener(this);
            
            RebuildMenu();
            SetBadgeOnTabBarItem();

            return view;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.TextSource))
                RebuildMenu();
        }

        private void BadgeServiceOnBadgeChanged(object sender, EventArgs e)
        {
            SetBadgeOnTabBarItem();
        }

        private void RebuildMenu()
        {
            var explore = _navigationView.Menu.FindItem(Resource.Id.page_1);
            explore?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Home]);

            var browse = _navigationView.Menu.FindItem(Resource.Id.page_2);
            browse?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Browse]);

            var search = _navigationView.Menu.FindItem(Resource.Id.page_3);
            search?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Search]);

            var myContent = _navigationView.Menu.FindItem(Resource.Id.page_4);
            myContent?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Favorites]);

            var settings = _navigationView.Menu.FindItem(Resource.Id.page_5);
            settings?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Profile]);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            var navigationItem = ViewModel.NavigationCommands.ElementAt(item.Order);
            navigationItem.Value.Execute();
            ViewModel.LogBottomBarButtonClicked(navigationItem.Key);
            return true;
        }

        // Prevent interfering with other fragments
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return false;
        }

        private void SetBadgeOnTabBarItem()
        {
            _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                if (!_badgeService.IsBadgeSet)
                {
                    RemoveBadge();
                    return;
                }

                RemoveBadge();
                var notificationItemView = _navigationView.FindViewById(Resource.Id.page_1);
                var inflater = (LayoutInflater)Context.GetSystemService(Android.Content.Context.LayoutInflaterService);
                _badgeView = inflater.Inflate(Resource.Layout.badge, null);
                _iconContainer = (FrameLayout)notificationItemView;
                _iconContainer.AddView(_badgeView);
            });
        }

        private void RemoveBadge()
        {
            if (_iconContainer != null && _iconContainer.HasChild(_badgeView))
                _iconContainer.RemoveView(_badgeView);
        }
    }
}