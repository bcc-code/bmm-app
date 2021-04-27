using System;
using BMM.Core.Models;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TextListItemDetailTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("TextListItemDetailTableViewCell");

        public TextListItemDetailTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TextListItemDetailTableViewCell, SelectableListItem>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(TextLabel).To(listItem => listItem.Text);
                set.Apply();
            });
        }

    }
}