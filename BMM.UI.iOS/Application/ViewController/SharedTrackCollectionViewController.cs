using System;
using BMM.Core.ValueConverters;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class SharedTrackCollectionViewController : BaseViewController<SharedTrackCollectionViewModel>
    {
        private string _name;

        public SharedTrackCollectionViewController() : base(null)
        {
        }

        public SharedTrackCollectionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PlaylistName.Text = _name;
                CollectionTableView.ResizeHeaderView();
            }
        }

        public string Author
        {
            get => _name;
            set
            {
                _name = value;
                AuthorNameLabel.Text = _name;
                CollectionTableView.ResizeHeaderView();
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss,
                OnDidAttemptToDismiss = HandleDismiss
            };

            PrepareHeader();
            SetThemes();
            Bind();

            CollectionTableView.TableFooterView = new UIView(
                new CGRect(
                    0,
                    0,
                    View.Frame.Width,
                    View.SafeAreaInsets.Bottom + ButtonBottomConstraint.Constant + AddToMyPlaylistButton.Frame.Height));
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<SharedTrackCollectionViewController, SharedTrackCollectionViewModel>();
            var source = new DocumentsTableViewSource(CollectionTableView);

            set
                .Bind(this)
                .For(v => v.Name)
                .To(vm => vm.MyCollection.Name);

            set
                .Bind(this)
                .For(v => v.Author)
                .To(vm => vm.PlaylistAuthor)
                .WithConversion<PlaylistAuthorToLabelConverter>();

            set
                .Bind(PlaylistName)
                .To(vm => vm.MyCollection.Name);

            set
                .Bind(TrackCountLabel)
                .To(vm => vm.TrackCountString);

            set
                .Bind(source)
                .To(vm => vm.Documents)
                .WithConversion<DocumentListValueConverter>(ViewModel);

            set
                .Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand)
                .WithConversion<DocumentSelectedCommandValueConverter>();

            set
                .Bind(source)
                .For(s => s.IsFullyLoaded)
                .To(vm => vm.IsLoading)
                .WithConversion<InvertedVisibilityConverter>();

            set
                .Bind(AddToMyPlaylistButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>("AddToMyPlaylist");

            set
                .Bind(AddToMyPlaylistButton)
                .To(vm => vm.AddToMyPlaylistCommand);

            set.Apply();
        }

        private void SetThemes()
        {
            PlaylistName.ApplyTextTheme(AppTheme.Heading2.Value);
            AuthorNameLabel.ApplyTextTheme(AppTheme.Paragraph2.Value);
            AddToMyPlaylistButton.ApplyButtonStyle(AppTheme.ButtonPrimary.Value);
        }

        private void PrepareHeader()
        {
            var closeButton = new UIBarButtonItem(
                ViewModel.TextSource.GetText("Close"),
                UIBarButtonItemStyle.Plain,
                (sender, e) =>
                {
                    ViewModel.CloseCommand.Execute();
                }
            );

            NavigationItem.SetLeftBarButtonItem(closeButton, true);
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}