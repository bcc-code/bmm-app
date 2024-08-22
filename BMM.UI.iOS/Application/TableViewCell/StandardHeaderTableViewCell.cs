using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class StandardHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(StandardHeaderTableViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(StandardHeaderTableViewCell), NSBundle.MainBundle);

        public StandardHeaderTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<StandardHeaderTableViewCell, HeaderPO>();
                set
                    .Bind(TitleLabel)
                    .To(vm => vm.Header);
                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}