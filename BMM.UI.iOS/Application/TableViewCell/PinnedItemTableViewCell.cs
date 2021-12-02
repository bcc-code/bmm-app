using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Helpers;

namespace BMM.UI.iOS
{
    public partial class PinnedItemTableViewCell : BaseBMMTableViewCell
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
                set.Bind(TypeImage)
                    .For(v => v.ImagePath)
                    .To(vm => vm.Item.Icon)
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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}