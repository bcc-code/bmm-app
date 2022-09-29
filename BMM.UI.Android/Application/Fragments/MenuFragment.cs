using System.ComponentModel;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.bottom_navigation_frame)]
    [Register("bmm.ui.droid.application.fragments.MenuFragment")]
    public class MenuFragment : BaseFragment<MenuViewModel>, NavigationBarView.IOnItemSelectedListener
    {
        private BottomNavigationView _navigationView;

        protected override int FragmentId => Resource.Layout.fragment_menu;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_menu, null);

            _navigationView = view.FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            _navigationView!.SetOnItemSelectedListener(this);

            RebuildMenu();

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
    }
}