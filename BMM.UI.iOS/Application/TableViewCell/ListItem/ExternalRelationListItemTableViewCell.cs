using System;
using BMM.Core.Models;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationListItemTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ExternalRelationListItemTableViewCell");
        public static readonly UINib Nib;

        static ExternalRelationListItemTableViewCell()
        {
            Nib = UINib.FromName("ExternalRelationListItemTableViewCell", NSBundle.MainBundle); 
        }

        public ExternalRelationListItemTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ExternalRelationListItemTableViewCell, ExternalRelationListItem>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Apply();
            });
        }
    }
}