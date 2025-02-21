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
    public partial class StandardChurchTableViewCell : BaseBMMTableViewCell
    {
        private GameNights _firstGameNight;
        private GameNights _thirdGameNight;
        private GameNights _secondGameNight;
        public static readonly UINib Nib = UINib.FromName(nameof(StandardChurchTableViewCell), NSBundle.MainBundle);
        public static readonly NSString Key = new(nameof(StandardChurchTableViewCell));

        public StandardChurchTableViewCell(NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(Bind);
        }

        protected override bool HasHighlightEffect => false;

        private void Bind()
        {
            var set = this.CreateBindingSet<StandardChurchTableViewCell, HvheChurchPO>();
            
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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
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
                _ => AppColors.BackgroundTwoColor
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
            ChurchName.ApplyTextTheme(AppTheme.Title2);
            RightPointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            MiddlePointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            LeftPointsLabel.ApplyTextTheme(AppTheme.Subtitle2Label1);
            LeftPointsLabel.TextColor = AppColors.GlobalWhiteOneColor;
            LeftPointsContainer.BackgroundColor = AppColors.BoysColor;
        }
    }
}