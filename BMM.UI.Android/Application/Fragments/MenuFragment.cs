using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using Google.Android.Material.BottomNavigation;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.bottom_navigation_frame)]
    [Register("bmm.ui.droid.application.fragments.MenuFragment")]
    public class MenuFragment : BaseFragment<MenuViewModel>, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private BottomNavigationView _navigationView;

        protected override int FragmentId => Resource.Layout.fragment_menu;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_menu, null);

            _navigationView = view.FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            _navigationView.SetOnNavigationItemSelectedListener(this);

            RebuildMenu();

            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsOnline" || e.PropertyName == "TextSource")
                {
                    RebuildMenu();
                }
            };

            return view;
        }

        private void RebuildMenu()
        {
            // ToDo: use proper axml bindings instead of manual code
            var search = _navigationView.Menu.FindItem(Resource.Id.page_1);
            search?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Home]);

            var browse = _navigationView.Menu.FindItem(Resource.Id.page_2);
            browse?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Browse]);

            var myContent = _navigationView.Menu.FindItem(Resource.Id.page_3);
            myContent?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Search]);

            var library = _navigationView.Menu.FindItem(Resource.Id.page_4);
            library?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Favorites]);

            var settings = _navigationView.Menu.FindItem(Resource.Id.page_5);
            settings?.SetTitle(ViewModel.TextSource[Translations.MenuViewModel_Profile]);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.page_1:
                    ViewModel.ExploreCommand.Execute();
                    return true;
                case Resource.Id.page_2:
                    ViewModel.BrowseCommand.Execute();
                    return true;
                case Resource.Id.page_3:
                    ViewModel.SearchCommand.Execute();
                    return true;
                case Resource.Id.page_4:
                    ViewModel.MyContentCommand.Execute();
                    return true;
                case Resource.Id.page_5:
                    ViewModel.SettingsCommand.Execute();
                    return true;
            }

            return false;
        }

        // Prevent interfering with other fragments
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return false;
        }
    }
}