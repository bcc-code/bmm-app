using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Models.POs.Other;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class PinnedItemTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(PinnedItemTableViewCell));

        public PinnedItemTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<PinnedItemTableViewCell, PinnedItemPO>();
                set.Bind(TitleLabel).To(vm => vm.PinnedItem.Title);
                set.Bind(TypeImage)
                    .For(v => v.ImagePath)
                    .To(vm => vm.PinnedItem)
                    .WithConversion<PinnedItemIconToImageConverter>();
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}