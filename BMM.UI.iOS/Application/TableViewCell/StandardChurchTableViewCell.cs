using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using ObjCRuntime;

namespace BMM.UI.iOS.CustomViews
{
    public partial class StandardChurchTableViewCell : BaseBMMTableViewCell
    {
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
            
            set.Apply();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
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