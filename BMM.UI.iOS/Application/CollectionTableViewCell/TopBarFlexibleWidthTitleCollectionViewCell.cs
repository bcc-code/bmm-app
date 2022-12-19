using System;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CollectionTableViewCell
{
    public partial class TopBarFlexibleWidthTitleCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(TopBarFlexibleWidthTitleCollectionViewCell));
        public static readonly UINib Nib;
        private string _text;
        private bool _isSelected;

        static TopBarFlexibleWidthTitleCollectionViewCell()
        {
            Nib = UINib.FromName(nameof(TopBarFlexibleWidthTitleCollectionViewCell), NSBundle.MainBundle);
        }

        protected TopBarFlexibleWidthTitleCollectionViewCell(IntPtr handle) : base(handle)
        {
            if (BindingContext is MvxTaskBasedBindingContext mvxTaskBasedBindingContext)
                mvxTaskBasedBindingContext.RunSynchronously = true;

            this.DelayBind(
                () =>
                {
                    var set = this.CreateBindingSet<TopBarFlexibleWidthTitleCollectionViewCell, SearchResultsViewModel>();
                    
                    set.Bind(this)
                        .For(x => x.Text)
                        .To(vm => vm.Title);
                    
                    set.Bind(this)
                        .For(x => x.IsSelected)
                        .To(vm => vm.Selected);

                    set.Apply();
                });
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (_isSelected)
                    TitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
                else
                    TitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label3);
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                TitleLabel.Text = _text;
                MainView.SetNeedsLayout();
                MainView.LayoutIfNeeded();
            }
        }

        public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
            UICollectionViewLayoutAttributes layoutAttributes)
        {
            double width = MainView.Frame.Width;

            layoutAttributes.Frame = new CGRect(
                layoutAttributes.Frame.X,
                layoutAttributes.Frame.Y,
                width,
                MainView.Frame.Height);

            return base.PreferredLayoutAttributesFittingAttributes(layoutAttributes);
        }
    }
}