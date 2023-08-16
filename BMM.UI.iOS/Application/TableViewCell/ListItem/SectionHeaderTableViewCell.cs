using BMM.Core.Models.POs.Other.Interfaces;
using MvvmCross.Binding.BindingContext;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class SectionHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(SectionHeaderTableViewCell));

        public SectionHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SectionHeaderTableViewCell, ISectionHeaderPO>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(Separator).For(v => v.Alpha).To(listItem => listItem.ShowDivider).WithConversion<BoolToNfloatConverter>();
                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}