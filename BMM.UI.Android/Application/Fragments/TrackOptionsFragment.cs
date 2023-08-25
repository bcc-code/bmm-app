using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = true, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.TrackOptionsFragment")]
    public class TrackOptionsFragment : BaseSlideInOutFragment<OptionsListViewModel>
    {
        private IMvxInteraction _closeInteraction;
        private bool _isExecuting;
        protected override int FragmentId => Resource.Layout.fragment_track_options;
        protected override int RootViewId => Resource.Id.TrackOptionsInnerContainer;

        protected override int? WindowAnimationsStyle => null;

        public IMvxInteraction CloseInteraction
        {
            get => _closeInteraction;
            set
            {
                if (_closeInteraction != null)
                    _closeInteraction.Requested -= OnCloseInteractionRequested;

                _closeInteraction = value;
                _closeInteraction.Requested += OnCloseInteractionRequested;
            }
        }

        protected override void Bind()
        {
            base.Bind();

            var set = this.CreateBindingSet<TrackOptionsFragment, OptionsListViewModel>();

            set.Bind(this)
                .For(f => f.CloseInteraction)
                .To(vm => vm.CloseInteraction)
                .OneWay();

            set.Apply();
        }
        
        protected override void AttachEvents()
        {
            base.AttachEvents();
            if (_closeInteraction != null)
            {
                _closeInteraction.Requested -= OnCloseInteractionRequested;
                _closeInteraction.Requested += OnCloseInteractionRequested;
            }
        }
        

        protected override void DetachEvents()
        {
            base.DetachEvents();

            if (_closeInteraction != null)
                _closeInteraction.Requested -= OnCloseInteractionRequested;
        }
        
        private async void OnCloseInteractionRequested(object sender, EventArgs e)
        {
            if (_isExecuting)
                return;

            try
            {
                _isExecuting = true;
                await AnimateSlide(false);
                await ViewModel.CloseCommand.ExecuteAsync();
            }
            finally
            {
                _isExecuting = false;
            }
        }

        protected override void HandleClose()
        {
            ViewModel?.CloseOptionsCommand?.Execute();
        }
    }
}