using System;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.TableViewCell.Base;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public abstract class BaseBMMTableViewCell : MvxTableViewCell
    {
        private UIView _selectionView;

        protected BaseBMMTableViewCell(IntPtr handle) : base(handle)
        {
        }

        protected virtual bool HasHighlightEffect { get; } = true;

        private void InitializeIfNeeded()
        {
            if (_selectionView != null)
                return;

            _selectionView = new UIView();
            _selectionView.BackgroundColor = AppColors.HighlightColor;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public override void TouchesBegan(NSSet touches, UIEvent? evt)
        {
            base.TouchesBegan(touches, evt);

            if (!HasHighlightEffect)
                return;

            InitializeIfNeeded();
            _selectionView.Frame = Bounds;
            ContentView.InsertSubview(_selectionView, 0);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent? evt)
        {
            base.TouchesCancelled(touches, evt);

            if (!HasHighlightEffect)
                return;

            _selectionView.RemoveFromSuperview();
        }

        public override void TouchesEnded(NSSet touches, UIEvent? evt)
        {
            base.TouchesEnded(touches, evt);

            if (!HasHighlightEffect)
                return;

            _selectionView.RemoveFromSuperview();
        }
    }
}