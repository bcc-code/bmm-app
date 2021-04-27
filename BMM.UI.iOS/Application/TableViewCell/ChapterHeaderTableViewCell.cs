using System;
using BMM.Core.Models;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ChapterHeaderTableViewCell: MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ChapterHeaderTableViewCell");

        public ChapterHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ChapterHeaderTableViewCell, CellWrapperViewModel<ChapterHeader>>();
                set.Bind(Titel).To(vm => vm.Item.Title).WithConversion<ToUppercaseConverter>();
                set.Apply();
            });
        }
    }
}