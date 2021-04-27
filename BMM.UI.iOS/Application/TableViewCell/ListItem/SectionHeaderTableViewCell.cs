using BMM.Core.Models;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using System;
using Foundation;

namespace BMM.UI.iOS
{
    public partial class SectionHeaderTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(SectionHeaderTableViewCell));

        public SectionHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SectionHeaderTableViewCell, SectionHeader>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(Separator).For(v => v.Alpha).To(listItem => listItem.ShowDivider).WithConversion<BoolToNfloatConverter>();
                set.Apply();
            });
        }
    }
}