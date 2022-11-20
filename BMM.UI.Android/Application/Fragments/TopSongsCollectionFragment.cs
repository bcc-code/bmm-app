using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using BMM.Core.Constants;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.TemplateSelectors;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.TopSongsCollectionFragment")]
    public class TopSongsCollectionFragment : BaseDialogFragment<TopSongsCollectionViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_top_songs;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.DocumentsRecyclerView);
            recyclerView!.Adapter = new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext, ViewTypes.TopSongsCollectionHeader);
            view.SetBackgroundColor(Context.GetColorFromResource(Resource.Color.background_primary_color));
            Toolbar.NavigationIcon = null;
            return view;
        }

        protected override void Bind()
        {
            var set = this.CreateBindingSet<TopSongsCollectionFragment, TopSongsCollectionViewModel>();

            set.Bind(Toolbar)
                .For(v => v.Title)
                .To(vm => vm.PageTitle);
            
            set.Apply();
        }
        
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.share_trackcollection, menu);
            var item = menu.GetItem(0);

            var title = new SpannableString(ViewModel.TextSource[Translations.Global_Done]);
            title.SetSpan(
                new ForegroundColorSpan(Context.GetColorFromResource(Resource.Color.tint_color)),
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