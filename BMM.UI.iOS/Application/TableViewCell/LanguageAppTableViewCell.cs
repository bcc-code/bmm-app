using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class LanguageAppTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(LanguageAppTableViewCell));

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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.ApplyTextTheme(AppTheme.Title2);
            IsSelectedImage.TintColor = AppColors.LabelPrimaryColor;
        }
    }
}