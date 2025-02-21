using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using ObjCRuntime;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS.CustomViews
{
    public partial class HighlightedChurchTableViewCell : BaseBMMTableViewCell
    {
        private GameNights _firstGameNight;
        private GameNights _secondGameNight;
        private GameNights _thirdGameNight;
        public static readonly UINib Nib = UINib.FromName(nameof(HighlightedChurchTableViewCell), NSBundle.MainBundle);
        public static readonly NSString Key = new(nameof(HighlightedChurchTableViewCell));

        public HighlightedChurchTableViewCell(NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(Bind);
        }

        protected override bool HasHighlightEffect => false;

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

        private void Bind()
        {
            var set = this.CreateBindingSet<HighlightedChurchTableViewCell, HvheChurchPO>();

            set.Bind(BoysPointsLabel)
                .To(po => po.Church.BoysPoints);
            
            set.Bind(GirlsPointsLabel)
                .To(po => po.Church.GirlsPoints);
            
            set.Bind(BoysLabel)
                .To(po => po.BoysTitle);
            
            set.Bind(GirlsLabel)
                .To(po => po.GirlsTitle);
            
            set.Bind(ChurchName)
                .To(po => po.Church.Name);
            
            set.Bind(this)
                .For(v => v.FirstGameNight)
                .To(po => po.FirstGameNight);
            
            set.Bind(this)
                .For(v => v.SecondGameNight)
                .To(po => po.SecondGameNight);
            
            set.Bind(this)
                .For(v => v.ThirdGameNight)
                .To(po => po.ThirdGameNight);
            
            set.Apply();
        }

        public GameNights FirstGameNight
        {
            get => _firstGameNight;
            set
            {
                _firstGameNight = value;
                LeftPointsContainer.BackgroundColor = GetBackgroundColor(_firstGameNight);
                LeftPointsLabel.TextColor = GetTextColor(_firstGameNight);
            }
        }

        public GameNights SecondGameNight
        {
            get => _secondGameNight;
            set
            {
                _secondGameNight = value;
                MiddlePointsContainer.BackgroundColor = GetBackgroundColor(_secondGameNight);
                MiddlePointsLabel.TextColor = GetTextColor(_secondGameNight);
            }
        }
        
        public GameNights ThirdGameNight
        {
            get => _thirdGameNight;
            set
            {
                _thirdGameNight = value;
                RightPointsContainer.BackgroundColor = GetBackgroundColor(_thirdGameNight);
                RightPointsLabel.TextColor = GetTextColor(_thirdGameNight);
            }
        }
        
        private UIColor GetBackgroundColor(GameNights gameNight)
        {
            return gameNight switch
            {
                GameNights.Boys => AppColors.BoysColor,
                GameNights.Girls => AppColors.GirlsColor,
                _ => AppColors.BackgroundOneColor
            };
        }
        
        private UIColor GetTextColor(GameNights gameNight)
        {
            return gameNight switch
            {
                GameNights.Boys or GameNights.Girls => AppColors.GlobalWhiteOneColor,
                _ => AppColors.LabelThreeColor
            };
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