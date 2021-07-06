using System;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;
using Xamarin.Essentials;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class ShareTrackCollectionViewController : BaseViewController<ShareTrackCollectionViewModel>
    {
        private bool _isPrivate;

        public ShareTrackCollectionViewController() : base(null)
        {
        }

        public ShareTrackCollectionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public bool IsPrivate
        {
            get => _isPrivate;
            set
            {
                _isPrivate = value;
                MakePrivateButton.Hidden = _isPrivate;
                PlaylistStateIcon.SetHiddenIfNeeded(_isPrivate);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = OnDidDismiss,
                OnDidAttemptToDismiss = OnDidAttemptToDismiss
            };

            PrepareHeader();
            SetThemes();
            Bind();
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<ShareTrackCollectionViewController, ShareTrackCollectionViewModel>();

            set
                .Bind(PlaylistName)
                .To(vm => vm.TrackCollectionName);

            set
                .Bind(PlaylistState)
                .To(vm => vm.FollowersCount)
                .WithConversion<FollowersCountToTrackCollectionStateConverter>();

            set
                .Bind(NoteLabel)
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>("ShareNote");

            set
                .Bind(ShareLinkButton)
                .To(vm => vm.ShareCommand);

            set
                .Bind(ShareLinkButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>("ShareLink");

            set
                .Bind(this)
                .For(v => v.IsPrivate)
                .To(vm => vm.FollowersCount)
                .WithConversion<FollowersCountToIsPrivateConverter>();

            set
                .Bind(MakePrivateButton)
                .To(vm => vm.MakePrivateCommand);

            set.Apply();
        }

        private void SetThemes()
        {
            PlaylistName.ApplyTextTheme(AppTheme.Heading2.Value);
            NoteLabel.ApplyTextTheme(AppTheme.Paragraph2.Value);
            PlaylistState.ApplyTextTheme(AppTheme.Paragraph2.Value);

            MakePrivateButton.ApplyButtonStyle(AppTheme.ButtonSecondary.Value);
            ShareLinkButton.ApplyButtonStyle(AppTheme.ButtonTertiary.Value);
        }

        private void PrepareHeader()
        {
            var saveButton = new UIBarButtonItem(
                ViewModel.TextSource.GetText("Done"),
                UIBarButtonItemStyle.Plain,
                (sender, e) =>
                {
                    ViewModel.CloseCommand.Execute();
                }
            );

            NavigationItem.SetRightBarButtonItem(saveButton, true);
        }

        private void OnDidAttemptToDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
        }

        private void OnDidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();

            if (presentationController.Delegate is CustomUIAdaptivePresentationControllerDelegate customUiAdaptivePresentationControllerDelegate)
                customUiAdaptivePresentationControllerDelegate.Clear();
        }
    }
}