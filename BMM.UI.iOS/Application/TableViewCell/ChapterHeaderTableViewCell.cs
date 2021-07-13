using System;
using BMM.Core.Models;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ChapterHeaderTableViewCell: MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ChapterHeaderTableViewCell));

        public ChapterHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ChapterHeaderTableViewCell, CellWrapperViewModel<ChapterHeader>>();
                set.Bind(Titel).To(vm => vm.Item.Title);
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            Titel.ApplyTextTheme(AppTheme.Subtitle3.Value);
        }
    }
}