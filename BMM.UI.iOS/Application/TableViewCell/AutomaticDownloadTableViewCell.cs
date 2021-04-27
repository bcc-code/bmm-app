using System;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class AutomaticDownloadTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("AutomaticDownloadTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("AutomaticDownloadTableViewCell");

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

        public static AutomaticDownloadTableViewCell Create()
        {
	        return (AutomaticDownloadTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}
