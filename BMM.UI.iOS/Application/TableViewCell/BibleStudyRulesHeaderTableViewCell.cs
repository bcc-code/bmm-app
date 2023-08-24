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
    public partial class BibleStudyRulesHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(BibleStudyRulesHeaderTableViewCell));

        public BibleStudyRulesHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BibleStudyRulesHeaderTableViewCell, IBibleStudyRulesHeaderPO>();

                set.Bind(HeaderLabel)
                    .To(d => d.HeaderText);

                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            HeaderLabel.ApplyTextTheme(AppTheme.Heading2);
        }
    }
}