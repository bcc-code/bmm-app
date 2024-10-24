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
    public partial class GibraltarProjectBoxViewCell : BaseBMMTableViewCell
    {
        private const int ItemSize = 64;
        
        public static readonly NSString Key = new(nameof(GibraltarProjectBoxViewCell));
        private IBmmObservableCollection<IAchievementPO> _itemsSource;

        public GibraltarProjectBoxViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<GibraltarProjectBoxViewCell, ProjectBoxPO>();

                set.Bind(TitleLabel)
                    .To(po => po.ProjectBox.Title);

                set.Bind(PointsLabel)
                    .To(po => po.ProjectBox.PointsDescription);
                
                set
                    .Bind(this)
                    .For(s => s.ItemsSource)
                    .To(po => po.Achievements);

                set.Bind(RulesLabel)
                    .To(po => po.ProjectBox.RulesLinkTitle);
                
                set.Bind(RulesContainer)
                    .For(v => v.BindTap())
                    .To(po => po.OpenRulesCommand);

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

            foreach (var item in achievements.Take(4))
            {
                var imageView = new MvxCachedImageView();
                imageView.ImagePath = item.ImagePath;
                imageView.UserInteractionEnabled = true;
                imageView.AddGestureRecognizer(new UITapGestureRecognizer(() => item.AchievementClickedCommand.Execute()));
                imageView.TranslatesAutoresizingMaskIntoConstraints = false;
                imageView.WidthAnchor.ConstraintEqualTo(ItemSize).Active = true;
                imageView.HeightAnchor.ConstraintEqualTo(ItemSize).Active = true;
                AchievementStackView.AddArrangedSubview(imageView);
            }
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            RulesLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);
            PointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);
        }
    }
}