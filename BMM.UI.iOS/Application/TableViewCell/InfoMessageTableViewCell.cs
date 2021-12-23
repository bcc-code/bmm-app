using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class InfoMessageTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(InfoMessageTableViewCell));
        private string _text;

        public InfoMessageTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<InfoMessageTableViewCell, CellWrapperViewModel<InfoMessage>>();

                set.Bind(InfoMessageLabel)
                    .To(d => d.Item.MessageText);

                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            InfoMessageLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
        }
    }
}