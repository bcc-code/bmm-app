using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Models.PO;
using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class SelectThemeTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(SelectThemeTableViewCell));

        public SelectThemeTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SelectThemeTableViewCell, ThemeSettingPO>();

                set.Bind(TextLabel)
                    .To(vm => vm.Label);

                set.Bind(IsSelectedImage)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsSelected);

                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.ApplyTextTheme(AppTheme.Title2);
            IsSelectedImage.TintColor = AppColors.LabelPrimaryColor;
        }
    }
}