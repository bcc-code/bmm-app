using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using System.Globalization;
using BMM.Core.ValueConverters;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class LanguageContentTableViewCell : MvxTableViewCell
    {
        public int Index {
            get { return 0; }
            set { IndexLabel.Text = value + ""; }
        }

        public static readonly UINib Nib = UINib.FromName("LanguageContentTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("LanguageContentTableViewCell");

        public LanguageContentTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<LanguageContentTableViewCell, CultureInfo>();
                set.Bind(TextLabel).WithConversion<LanguageNameValueConverter>();
                set.Apply();
            });
        }

        public static LanguageContentTableViewCell Create()
        {
            return (LanguageContentTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}