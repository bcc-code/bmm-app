using System;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.CollectionViewSource;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Helpers;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using TagLib.Id3v2;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class YearInReviewViewController : BaseViewController<YearInReviewViewModel>, ICollectionViewSnapHandler
    {
        public YearInReviewViewController() : base(null)
        {
            Instance = this;
        }

        public YearInReviewViewController(string nib) : base(nib)
        {
            Instance = this;
        }

        public static YearInReviewViewController Instance;
        private YearInReviewSource _source;

        public UIButton ButtonShare => ShareButton;
        public override Type ParentViewControllerType => null;

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            var imageHeight = (View!.Bounds.Height - YearInReviewViewModel.HeaderHeight) * YearInReviewViewModel.ImageHeightToViewHeightRatio;
            var itemHeight = imageHeight + YearInReviewViewModel.BottomImageMargin;
            var itemWidth = imageHeight * YearInReviewViewModel.ImageWidthToHeightRatio;
            double imageWidth = itemWidth;

            var recyclerViewHorizontalPadding = (View!.Bounds.Width - (int)itemWidth) / 2;
            
            _source.ItemHeight = itemHeight;
            _source.ItemWidth = itemWidth;
            _source.CollectionViewMargin = recyclerViewHorizontalPadding;
            YearInReviewCollectionViewConstraint.Constant = itemHeight;
            YearInReviewCollectionViewCell.ImageHeight = imageHeight;
            YearInReviewCollectionViewCell.ImageWidth = (float)imageWidth;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _source = new YearInReviewSource(
                YearInReviewCollectionView,
                YearInReviewCollectionViewCell.Key,
                this);
            
            YearInReviewCollectionView.RegisterNibForCell(YearInReviewCollectionViewCell.Nib, YearInReviewCollectionViewCell.Key);
            YearInReviewCollectionView.DecelerationRate = UIScrollView.DecelerationRateFast;
            
            var set = this.CreateBindingSet<YearInReviewViewController, YearInReviewViewModel>();
            
            set.Bind(_source)
                .For(s => s.ItemsSource)
                .To(vm => vm.YearInReviewElements);
                
            set.Bind(DescriptionLabel)
                .To(vm => vm.Description);
            
            set.Bind(ShareButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.YearInReviewViewModel_Share]);

            set.Bind(ShareButton)
                .To(vm => vm.ShareCommand);
            
            set.Apply();

            YearInReviewCollectionView.Source = _source;
            
            NavigationController!.PresentationController!.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };

            PrepareHeader();
            SetThemes();
        }

        private void SetThemes()
        {
            DescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            DescriptionLabel.MinimumFontSize = 12;
            DescriptionLabel.AdjustsFontSizeToFitWidth = true;
            ShareButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
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

        public void OnPositionChanged(int currentPosition)
        {
            ViewModel.CurrentPosition = currentPosition;
        }
    }
}