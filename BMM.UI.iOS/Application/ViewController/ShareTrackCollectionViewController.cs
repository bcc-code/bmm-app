using System;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class ShareTrackCollectionViewController : BaseViewController<ShareTrackCollectionViewModel>
    {
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
                .Bind(PlaylistType)
                .To(vm => vm.TrackCollectionShareType);

            set
                .Bind(NoteLabel)
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>("ShareNote");

            set.Apply();
        }

        private void SetThemes()
        {
            PlaylistName.ApplyTextTheme(AppTheme.Heading2.Value);
            NoteLabel.ApplyTextTheme(AppTheme.Paragraph2.Value);
            PlaylistType.ApplyTextTheme(AppTheme.Paragraph2.Value);

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