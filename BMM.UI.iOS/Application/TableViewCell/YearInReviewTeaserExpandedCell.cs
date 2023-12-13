using MvvmCross.Binding.BindingContext;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.TableViewCell.Base;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS
{
    public partial class YearInReviewTeaserExpandedCell : BaseBMMTableViewCell, IEventsHoldingTableViewCell
    {
        public static readonly NSString Key = new(nameof(YearInReviewTeaserExpandedCell));
        private IMvxInteraction _expandOrCollapseInteraction;

        public YearInReviewTeaserExpandedCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<YearInReviewTeaserExpandedCell, YearInReviewTeaserPO>();

                set.Bind(TitleLabel)
                    .To(po => po.YearInReviewTeaser.Title);

                set.Bind(DescriptionLabel)
                    .To(po => po.YearInReviewTeaser.Description);
                
                set.Bind(CollapseButton)
                    .For(v => v.BindTap())
                    .To(po => po.ExpandOrCollapseCommand);
                
                set.Bind(this)
                    .For(v => v.ExpandOrCollapseInteraction)
                    .To(po => po.ExpandOrCollapseInteraction);
                
                set.Bind(PlaylistButton)
                    .For(v => v.BindTitle())
                    .To(po => po.YearInReviewTeaser.PlaylistName);
                
                set.Bind(PlaylistButton)
                    .To(po => po.OpenTopSongsCommand);
                
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