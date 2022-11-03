using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using System.Diagnostics;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.TableViewCell.Base;
using CoreGraphics;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class YearInReviewViewExpandedCell : BaseBMMTableViewCell, IEventsHoldingTableViewCell
    {
        public static readonly NSString Key = new(nameof(YearInReviewViewExpandedCell));
        private IMvxInteraction _expandOrCollapseInteraction;

        public YearInReviewViewExpandedCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<YearInReviewViewExpandedCell, YearInReviewPreviewPO>();

                set.Bind(TitleLabel)
                    .To(po => po.YearInReviewPreview.Title);

                set.Bind(DescriptionLabel)
                    .To(po => po.YearInReviewPreview.Description);
                
                set.Bind(CollapseButton)
                    .For(v => v.BindTap())
                    .To(po => po.ExpandOrCollapseCommand);
                
                set.Bind(this)
                    .For(v => v.ExpandOrCollapseInteraction)
                    .To(po => po.ExpandOrCollapseInteraction);
                
                set.Bind(PlaylistButton)
                    .For(v => v.BindTitle())
                    .To(po => po.YearInReviewPreview.PlaylistName);
                
                set.Bind(SeeReviewButton)
                    .To(po => po.SeeReviewCommand);
                
                set.Apply();
            });
        }
        
        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Heading3);
            DescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            SeeReviewButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            CollapseButton.Transform = CGAffineTransform.MakeRotation(180f.ToRadians());
            PlaylistButton.ApplyButtonStyle(AppTheme.YearInReviewButton);
            PlaylistButton.IsTitleCentered = false;
        }
        
        public IMvxInteraction ExpandOrCollapseInteraction
        {
            get => _expandOrCollapseInteraction;
            set
            {
                if (_expandOrCollapseInteraction != null)
                    _expandOrCollapseInteraction.Requested -= ExpandOrCollapseCellInteractionRequested;

                _expandOrCollapseInteraction = value;

                if (_expandOrCollapseInteraction != null)
                    _expandOrCollapseInteraction.Requested += ExpandOrCollapseCellInteractionRequested;
            }
        }

        private void ExpandOrCollapseCellInteractionRequested(object sender, EventArgs e)
        {
            if (Superview is UITableView tableView)
            {
                Debug.WriteLine(tableView);
                tableView.ReloadRows(
                    tableView.IndexPathForCell(this).EncloseInArray(),
                    UITableViewRowAnimation.Automatic);
            }
        }
        
        public void AttachEvents()
        {
            if (ExpandOrCollapseInteraction == null)
                return;

            ExpandOrCollapseInteraction.Requested -= ExpandOrCollapseCellInteractionRequested;
            ExpandOrCollapseInteraction.Requested += ExpandOrCollapseCellInteractionRequested;
        }

        public void DetachEvents()
        {
            if (ExpandOrCollapseInteraction == null)
                return;

            ExpandOrCollapseInteraction.Requested -= ExpandOrCollapseCellInteractionRequested;
        }
    }
}