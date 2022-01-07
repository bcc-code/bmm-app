using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class StandardSelectableTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(StandardSelectableTableViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(StandardSelectableTableViewCell), NSBundle.MainBundle);

        public StandardSelectableTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<StandardSelectableTableViewCell, StandardSelectablePO>();
                set.Bind(OptionTitleLabel)
                    .To(vm => vm.Label);
                set.Bind(SelectIcon)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsSelected);
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            OptionTitleLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}