using System;
using BMM.Core.Models;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TextListItemTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("TextListItemTableViewCell");
        public static readonly UINib Nib;

        public TextListItemTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TextListItemTableViewCell, SelectableListItem>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(TextLabel).To(listItem => listItem.Text);
                set.Apply();
            });
        }

        public static TextListItemTableViewCell Create()
        {
            return (TextListItemTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}