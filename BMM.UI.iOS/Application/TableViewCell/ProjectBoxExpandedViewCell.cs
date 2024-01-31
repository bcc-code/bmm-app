using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using System.Diagnostics;
using BMM.Core.Extensions;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.YearInReview;
using BMM.Core.Utils;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.TableViewCell.Base;
using CoreGraphics;
using FFImageLoading.Cross;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ProjectBoxExpandedViewCell : BaseBMMTableViewCell, IEventsHoldingTableViewCell
    {
        public static readonly NSString Key = new(nameof(ProjectBoxExpandedViewCell));
        private IMvxInteraction _expandOrCollapseInteraction;
        private IBmmObservableCollection<IAchievementPO> _itemsSource;

        public ProjectBoxExpandedViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProjectBoxExpandedViewCell, ProjectBoxPO>();

                set.Bind(TitleLabel)
                    .To(po => po.ProjectBox.Title);

                set.Bind(PointsLabel)
                    .To(po => po.ProjectBox.PointsDescription);
                
                set.Bind(PointsNumber)
                    .To(po => po.ProjectBox.Points);
                
                set.Bind(CollapseButton)
                    .For(v => v.BindTap())
                    .To(po => po.ExpandOrCollapseCommand);
                
                set.Bind(this)
                    .For(v => v.ExpandOrCollapseInteraction)
                    .To(po => po.ExpandOrCollapseInteraction);

                set
                    .Bind(this)
                    .For(s => s.ItemsSource)
                    .To(po => po.Achievements);

                set.Bind(QuestionsButton)
                    .For(v => v.BindTitle())
                    .To(po => po.ProjectBox.ButtonTitle);
                
                set.Bind(QuestionsButton)
                    .To(po => po.OpenQuestionsCommand);

                set.Bind(RulesLabel)
                    .To(po => po.ProjectBox.RulesLinkTitle);
                set.Bind(RulesLabel)
                    .For(v => v.BindTap())
                    .To(po => po.OpenRulesCommand);

                set.Bind(IconBackground)
                    .For(v => v.BackgroundColor)
                    .To(po => po.ProjectBox.IconColor)
                    .WithConversion<HexStringToUiColorConverter>();

                set.Apply();
            });
        }

        public IBmmObservableCollection<IAchievementPO> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                SetItems(_itemsSource);
            }
        }

        private void SetItems(IBmmObservableCollection<IAchievementPO> achievements)
        {
            if (achievements == null ||achievements.Count == 0)
                return;

            foreach (var view in AchievementStackView.ArrangedSubviews)
                view.RemoveFromSuperview();

            var rows = ProjectBoxUtils.AdjustAchievementsRows(achievements);
            
            foreach (var row in rows)
            {
                var stackView = new UIStackView
                {
                    Axis = UILayoutConstraintAxis.Horizontal,
                    Spacing = 6,
                    Alignment = UIStackViewAlignment.Fill,
                    Distribution = UIStackViewDistribution.FillProportionally,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                
                foreach (var item in row)
                {
                    if (item is IAchievementPO achievementPO)
                    {
                        var imageView = new MvxCachedImageView();
                        imageView.ImagePath = achievementPO.ImagePath;
                        imageView.UserInteractionEnabled = true;
                        imageView.AddGestureRecognizer(new UITapGestureRecognizer(() => achievementPO.AchievementClickedCommand.Execute()));
                        imageView.TranslatesAutoresizingMaskIntoConstraints = false;
                        imageView.HeightAnchor.ConstraintEqualTo(40).Active = true;
                        imageView.WidthAnchor.ConstraintEqualTo(40).Active = true;
                        stackView.AddArrangedSubview(imageView);
                    }
                    else
                    {
                        var emptyView = new UIView();
                        emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
                        emptyView.HeightAnchor.ConstraintEqualTo(40).Active = true;
                        emptyView.WidthAnchor.ConstraintEqualTo(40).Active = true;
                        stackView.AddArrangedSubview(emptyView);
                    }
                }
                
                AchievementStackView.AddArrangedSubview(stackView);
            }
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            //ToDo: Apply global.black.1 to Icon
            TitleLabel.ApplyTextTheme(AppTheme.Title1);
            RulesLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
            PointsLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            PointsNumber.ApplyTextTheme(AppTheme.Heading2);
            CollapseButton.Transform = CGAffineTransform.MakeRotation(180f.ToRadians());
            QuestionsButton.ApplyButtonStyle(AppTheme.YearInReviewButton);
            QuestionsButton.IsTitleCentered = false;

            QuestionsButton.Layer.CornerRadius = 16;
            QuestionsButton.Layer.ShadowOffset = new CGSize(width: 0.0, height: 1.0);
            QuestionsButton.Layer.ShadowOpacity = 0.2f;
            QuestionsButton.Layer.ShadowRadius = 0.0f;
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