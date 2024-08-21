using BMM.Core.Models;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using BMM.Core.Models.POs.Other;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class CheckboxListItemTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(CheckboxListItemTableViewCell));

        public CheckboxListItemTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<CheckboxListItemTableViewCell, CheckboxListItemPO>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(TextLabel).To(listItem => listItem.Text);
                set.Bind(Switch).To(listItem => listItem.IsChecked);
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            TextLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);
        }
    }
}