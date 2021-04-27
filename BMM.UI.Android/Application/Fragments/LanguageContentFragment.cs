using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters.DragAndDrop;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.LanguageContentFragment")]
    public class LanguageContentFragment : BaseFragment<LanguageContentViewModel>, IOnStartDragListener
    {
        private ItemTouchHelper _touchHelper;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            LanguageListAdapter adapter = new LanguageListAdapter((IMvxAndroidBindingContext)BindingContext, this);

            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.language_list);

            var layoutManager = new LinearLayoutManager(ParentActivity);

            recyclerView.Adapter = adapter;
            recyclerView.SetLayoutManager(layoutManager);

            var callback = new UpAndDownDraggingTouchCallback(adapter);
            _touchHelper = new ItemTouchHelper(callback);
            _touchHelper.AttachToRecyclerView(recyclerView);

            ViewModel.LanguageAddedAdapterCallback = position => adapter.OnItemAdd(position);
            ViewModel.LanguageRemovedAdapterCallback = position => adapter.OnItemDismiss(position);

            return view;
        }

        protected override int FragmentId => Resource.Layout.fragment_language_content;

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            _touchHelper.StartDrag(viewHolder);
        }
    }
}