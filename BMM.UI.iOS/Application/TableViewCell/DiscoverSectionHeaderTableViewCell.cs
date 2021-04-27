using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class DiscoverSectionHeaderTableViewCell: MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("DiscoverSectionHeaderTableViewCell");

        public DiscoverSectionHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DiscoverSectionHeaderTableViewCell, CellWrapperViewModel<DiscoverSectionHeader>>();
                set.Bind(Titel).To(vm => vm.Item.Title);
                set.Bind(LinkButton).For("Title").To(vm => vm.ViewModel.TextSource).WithConversion<MvxLanguageConverter>("ShowAll");
                set.Bind(LinkButton).WithConversion<DeepLinkButtonCommandValueConverter>();
                set.Bind(LinkButton).For(v => v.Hidden).To(vm => vm.Item.HasLink).WithConversion<VisibilityConverter>();
                set.Apply();
            });
        }
    }
}