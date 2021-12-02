using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class ShareTrackCollectionViewController : BaseViewController<ShareTrackCollectionViewModel>
    {
        private bool _isPrivate;
        private bool _isSharingStatusIconVisible;

        public ShareTrackCollectionViewController() : base(null)
        {
        }

        public ShareTrackCollectionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
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
                .To(vm => vm.TrackCollection)
                .WithConversion<TrackCollectionToSharingStateConverter>();

            set
                .Bind(NoteLabel)
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>(Translations.ShareTrackCollectionViewModel_ShareNote);

            set
                .Bind(ShareLinkButton)
                .To(vm => vm.ShareCommand);

            set
                .Bind(ShareLinkButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>(Translations.ShareTrackCollectionViewModel_ShareLink);

            set
                .Bind(this)
                .For(v => v.IsSharingStatusIconVisible)
                .To(vm => vm.TrackCollection)
                .WithConversion<TrackCollectionToPlaylistStatusIconIsVisibleConverter>();

            set
                .Bind(MakePrivateButton)
                .To(vm => vm.MakePrivateCommand);

            set
                .Bind(MakePrivateButton)
                .For(v => v.BindVisible())
                .To(vm => vm.FollowersCount)
                .WithConversion<FollowersCountToMakePrivateButtonIsVisibleConverter>();

            set.Apply();
        }

        public bool IsSharingStatusIconVisible
        {
            get => _isSharingStatusIconVisible;
            set
            {
                _isSharingStatusIconVisible = value;
                PlaylistStateIcon.SetHiddenIfNeeded(!_isSharingStatusIconVisible);
            }
        }

        private void SetThemes()
        {
            PlaylistName.ApplyTextTheme(AppTheme.Heading3);
            NoteLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
            PlaylistState.ApplyTextTheme(AppTheme.Paragraph2);

            MakePrivateButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
            ShareLinkButton.ApplyButtonStyle(AppTheme.ButtonPrimary.Value);
        }

        private void PrepareHeader()
        {
            var saveButton = new UIBarButtonItem(
                ViewModel.TextSource[Translations.Global_Done],
                UIBarButtonItemStyle.Plain,
                (sender, e) =>
                {
                    ViewModel.CloseCommand.Execute();
                }
            );

            NavigationItem.SetRightBarButtonItem(saveButton, true);
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}