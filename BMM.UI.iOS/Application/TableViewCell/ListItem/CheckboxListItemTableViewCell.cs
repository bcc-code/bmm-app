using BMM.Core.Models;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class CheckboxListItemTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("CheckboxListItemTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("CheckboxListItemTableViewCell");

        public CheckboxListItemTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<CheckboxListItemTableViewCell, CheckboxListItem>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(TextLabel).To(listItem => listItem.Text);
                set.Bind(Switch).To(listItem => listItem.IsChecked);
                set.Apply();
            });
        }

        public static CheckboxListItemTableViewCell Create()
        {
            return (CheckboxListItemTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}