using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class HvheBoysVsGirlsTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(HvheBoysVsGirlsTableViewCell));

        public HvheBoysVsGirlsTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HvheBoysVsGirlsTableViewCell, HvheBoysVsGirlsPO>();

                set.Bind(BoysLabel)
                    .To(po => po.BoysPoints);
                
                set.Bind(GirlsLabel)
                    .To(po => po.GirlsPoints);
                
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            BoysPointsContainer.RoundLeftCorners(20);
            GirlsPointsContainer.RoundRightCorners(20);
        }

        private void SetThemes()
        {
            BoysLabel.Font = UIFont.FromName(FontNames.IBMPlexMonoBold, 72);
            GirlsLabel.Font = UIFont.FromName(FontNames.IBMPlexMonoBold, 72);
        }

        protected override bool HasHighlightEffect => false;
    }
}