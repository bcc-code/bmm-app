using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.TableViewCell.Base;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ProjectBoxCollapsedViewCell : BaseBMMTableViewCell, IEventsHoldingTableViewCell
    {
        public static readonly NSString Key = new(nameof(ProjectBoxCollapsedViewCell));
        private IMvxInteraction _expandOrCollapseInteraction;

        public ProjectBoxCollapsedViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProjectBoxCollapsedViewCell, ProjectBoxPO>();

                set.Bind(TitleLabel)
                    .To(po => po.ProjectBox.Title);
                
                set.Bind(ContainerView)
                    .For(v => v.BindTap())
                    .To(po => po.ExpandOrCollapseCommand);
                
                set.Bind(ExpandButton)
                    .For(v => v.BindTap())
                    .To(po => po.ExpandOrCollapseCommand);
                
                set.Bind(this)
                    .For(v => v.ExpandOrCollapseInteraction)
                    .To(po => po.ExpandOrCollapseInteraction);
                
                set.Apply();
            });
        }
        
        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title1);
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