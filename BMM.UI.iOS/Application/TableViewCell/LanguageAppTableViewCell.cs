using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using UIKit;
using BMM.Core.ValueConverters;

namespace BMM.UI.iOS
{
    public partial class LanguageAppTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("LanguageAppTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("LanguageAppTableViewCell");

        public LanguageAppTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<LanguageAppTableViewCell, LanguageCellWrapperViewModel>();
                set.Bind(TextLabel).To(vm => vm.Item).WithConversion<LanguageNameValueConverter>();
                set.Bind(IsSelectedImage).For("Visibility").To(vm => vm.IsSelected).WithConversion(new InvertedVisibilityConverter());
                set.Apply();
            });
        }

        public static LanguageAppTableViewCell Create()
        {
            return (LanguageAppTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}