using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using ObjCRuntime;

namespace BMM.UI.iOS.CustomViews
{
    public partial class HighlightedChurchTableViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName(nameof(HighlightedChurchTableViewCell), NSBundle.MainBundle);
        public static readonly NSString Key = new(nameof(HighlightedChurchTableViewCell));

        public HighlightedChurchTableViewCell(NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(Bind);
        }

        protected override bool HasHighlightEffect => false;

        private void Bind()
        {
            var set = this.CreateBindingSet<HighlightedChurchTableViewCell, HvheChurchPO>();

            set.Bind(BoysPointsLabel)
                .To(po => po.Church.BoysPoints);
            
            set.Bind(GirlsPointsLabel)
                .To(po => po.Church.GirlsPoints);
            
            set.Bind(BoysLabel)
                .To(po => po.TextSource[Translations.HvheDetailsViewModel_Boys]);
            
            set.Bind(GirlsLabel)
                .To(po => po.TextSource[Translations.HvheDetailsViewModel_Girls]);
            
            set.Bind(ChurchName)
                .To(po => po.Church.Name);
            
            set.Apply();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            BoysPointsContainer.ApplyRoundedCorners(4, 12, 4, 4);
            GirlsPointsContainer.ApplyRoundedCorners(4, 4, 4, 12);
        }

        private void SetThemes()
        {
            ChurchName.ApplyTextTheme(AppTheme.Title2);
            BoysLabel.ApplyTextTheme(AppTheme.Title2);
            BoysLabel.TextColor = AppColors.BoysColor;
            GirlsLabel.ApplyTextTheme(AppTheme.Title2);
            GirlsLabel.TextColor = AppColors.GirlsColor;
            ChurchName.ApplyTextTheme(AppTheme.Title2);
            BoysPointsLabel.ApplyTextTheme(AppTheme.Title2);
            GirlsPointsLabel.ApplyTextTheme(AppTheme.Title2);
            RightPointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            MiddlePointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            LeftPointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label1);
            LeftPointsLabel.TextColor = AppColors.GlobalWhiteOneColor;
            LeftPointsContainer.BackgroundColor = AppColors.BoysColor;
        }
    }
}