using System;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class TopSongsCollectionViewController : BaseViewController<TopSongsCollectionViewModel>
    {
        private string _viewTitle;

        public TopSongsCollectionViewController() : base(null)
        {
        }

        public TopSongsCollectionViewController(string nib) : base(nib)
        {
        }

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

            CollectionTableView.TableFooterView = new UIView(
                new CGRect(
                    0,
                    0,
                    View.Frame.Width,
                    View.SafeAreaInsets.Bottom + ButtonBottomConstraint.Constant + AddToFavouritesButton.Frame.Height));
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<TopSongsCollectionViewController, TopSongsCollectionViewModel>();
            var source = new DocumentsTableViewSource(CollectionTableView);

            set.Bind(this)
                .For(v => v.ViewTitle)
                .To(vm => vm.PageTitle);

            set
                .Bind(TrackCountLabel)
                .To(vm => vm.TrackCountString);

            set
                .Bind(source)
                .To(vm => vm.Documents);

            set
                .Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand);

            set
                .Bind(source)
                .For(s => s.IsFullyLoaded)
                .To(vm => vm.IsLoading)
                .WithConversion<InvertedVisibilityConverter>();

            set
                .Bind(AddToFavouritesButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource)
                .WithConversion<MvxLanguageConverter>(Translations.TopSongsCollectionViewModel_AddToMyPlaylist);

            set
                .Bind(AddToFavouritesButton)
                .To(vm => vm.AddToFavouritesCommand);

            set.Apply();
        }

        public string ViewTitle
        {
            get => _viewTitle;
            set
            {
                _viewTitle = value;
                if (string.IsNullOrEmpty(_viewTitle))
                    return;
                Title = _viewTitle;
            }
        }

        private void SetThemes()
        {
            HeaderLabel.ApplyTextTheme(AppTheme.Heading2);
            AddToFavouritesButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
        }

        private void PrepareHeader()
        {
            var closeButton = new UIBarButtonItem(
                ViewModel.TextSource[Translations.Global_Done],
                UIBarButtonItemStyle.Plain,
                (sender, e) =>
                {
                    ViewModel.CloseCommand.Execute();
                }
            );

            NavigationItem.SetRightBarButtonItem(closeButton, true);
            
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}