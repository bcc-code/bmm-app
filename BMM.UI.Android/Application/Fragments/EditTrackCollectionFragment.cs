using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Activity;
using AndroidX.Core.Widget;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Adapters.DragAndDrop;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.EditTrackCollectionFragment")]
    public class EditTrackCollectionFragment : BaseFragment<EditTrackCollectionViewModel>, IOnStartDragListener
    {
        protected override int FragmentId => Resource.Layout.fragment_edit_trackcollection;

        protected override bool IsTabBarVisible => false;

        private ItemTouchHelper _touchHelper;
        private EditText _textField;
        private NestedScrollView _nestedScrollView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            InitRecyclerView(view);
            _textField = view.FindViewById<EditText>(Resource.Id.title_text_field);
            _nestedScrollView = view.FindViewById<NestedScrollView>(Resource.Id.nested_scroll_view);

            Activity?.OnBackPressedDispatcher.AddCallback(this, new EditTrackCollectionOnBackPressedCallback(ViewModel));
            ParentActivity?.SupportActionBar?.SetHomeAsUpIndicator(Resource.Drawable.icon_close_static);

            return view;
        }

        private void InitRecyclerView(View view)
        {
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.trackcollection_recycler_view);
            if (recyclerView == null)
                return;

            var adapter = new DocumentDragAndDropAdapter((IMvxAndroidBindingContext)BindingContext, this);

            recyclerView.Adapter = adapter;

            var callback = new UpAndDownDraggingTouchCallback(adapter);
            _touchHelper = new ItemTouchHelper(callback);
            _touchHelper.AttachToRecyclerView(recyclerView);

            var layoutManager = new LinearLayoutManager(ParentActivity);
            recyclerView.SetLayoutManager(layoutManager);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.edit_trackcollection, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource[Translations.EditTrackCollectionViewModel_MenuSave]);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_save:
                    ViewModel.SaveAndCloseCommand.Execute();
                    return true;

                case Android.Resource.Id.Home:
                    ViewModel.DiscardAndCloseCommand.Execute();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            _nestedScrollView.ScrollChange += ClearTextFieldFocus;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            _nestedScrollView.ScrollChange -= ClearTextFieldFocus;
        }

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            _touchHelper.StartDrag(viewHolder);
        }

        private void ClearTextFieldFocus(object sender, NestedScrollView.ScrollChangeEventArgs scrollChangeEventArgs)
        {
            _textField?.ClearFocus();
            this.HideKeyboardForView(_textField);
        }

        private class EditTrackCollectionOnBackPressedCallback : OnBackPressedCallback
        {
            private readonly EditTrackCollectionViewModel _viewModel;

            public EditTrackCollectionOnBackPressedCallback(EditTrackCollectionViewModel viewModel) : base(true)
            {
                _viewModel = viewModel;
            }

            public override void HandleOnBackPressed()
            {
                _viewModel.DiscardAndCloseCommand.Execute();
            }
        }
    }
}