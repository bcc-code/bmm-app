using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using AndroidX.Activity;
using AndroidX.Core.Content;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.ShareTrackCollectionFragment")]
    public class ShareTrackCollectionFragment : BaseFragment<ShareTrackCollectionViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_share_trackcollection;

        protected override bool IsTabBarVisible => false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            Toolbar.NavigationIcon = null;
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.share_trackcollection, menu);
            var item = menu.GetItem(0);

            var title = new SpannableString(ViewModel.TextSource[Translations.Global_Done]);
            title.SetSpan(
                new ForegroundColorSpan(Context.GetColorFromTheme(Resource.Attribute.tint_color)),
                0,
                title.Length(),
                0);

            item.SetTitle(title);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_done:
                    ViewModel.CloseCommand.Execute();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}