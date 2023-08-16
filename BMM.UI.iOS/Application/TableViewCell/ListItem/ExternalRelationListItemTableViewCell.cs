using System;
using BMM.Core.Models;
using BMM.Core.Models.POs.Other.Interfaces;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationListItemTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ExternalRelationListItemTableViewCell));

        static ExternalRelationListItemTableViewCell()
        {
        }

        public ExternalRelationListItemTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ExternalRelationListItemTableViewCell, IExternalRelationListItemPO>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}