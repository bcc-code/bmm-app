using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.InfoMessages;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class BibleStudyRulesContentTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(BibleStudyRulesContentTableViewCell));

        public BibleStudyRulesContentTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BibleStudyRulesContentTableViewCell, IBibleStudyRulesContentPO>();

                set.Bind(TitleLabel)
                    .To(d => d.Title);

                set.Bind(TextLabel)
                    .To(d => d.Text);
                
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title1);
            TextLabel.ApplyTextTheme(AppTheme.Paragraph2);
        }
    }
}