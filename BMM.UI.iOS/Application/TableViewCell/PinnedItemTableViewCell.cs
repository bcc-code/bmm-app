using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.UI.iOS.Helpers;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class PinnedItemTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("PinnedItemTableViewCell");

        private VisibilityBindingsManager<CellWrapperViewModel<Document>> _bindingsManager;

        public PinnedItemTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<PinnedItemTableViewCell, CellWrapperViewModel<PinnedItem>>();
                set.Bind(TitleLabel).To(vm => vm.Item.Title);
                set.Bind(TypeImage).For(i => i.ImagePath).To(vm => vm.Item.Icon)
                    .WithConversion<PinnedItemIconToImageConverter>(new object[] { (Func<Document>)(() => ((CellWrapperViewModel<Document>)DataContext).Item), "" });
                set.Apply();
            });

            BindingContext.DataContextChanged += (sender, e) =>
            {
                if (DataContext == null)
                    return;

                if (_bindingsManager == null)
                    _bindingsManager = new VisibilityBindingsManager<CellWrapperViewModel<Document>>();

                _bindingsManager.Update((CellWrapperViewModel<Document>) DataContext);
            };
        }
    }
}