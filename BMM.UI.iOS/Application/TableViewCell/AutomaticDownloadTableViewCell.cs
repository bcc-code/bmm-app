using System;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class AutomaticDownloadTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(AutomaticDownloadTableViewCell));

        protected AutomaticDownloadTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
			{
            	var set = this.CreateBindingSet<AutomaticDownloadTableViewCell, AutomaticDownloadCellWrapperViewModel>();
			    set.Bind(TextLabel).To(vm => vm.Item.Title);
            	set.Bind(IsSelectedImage).For("Visibility").To(vm => vm.IsSelected).WithConversion(new InvertedVisibilityConverter());
            	set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.ApplyTextTheme(AppTheme.Paragraph1Label1);
            IsSelectedImage.TintColor = AppColors.LabelOneColor;
        }
    }
}
