using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class InfoMessageTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(InfoMessageTableViewCell));

        public InfoMessageTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<InfoMessageTableViewCell, InfoMessagePO>();

                set.Bind(InfoMessageLabel)
                    .To(d => d.InfoMessage.MessageText);
                
                set.Bind(ContentView)
                    .For(v => v.BindTap())
                    .To(d => d.TapCommand);

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