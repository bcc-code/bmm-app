using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class ChapterHeaderTableViewCell : BaseBMMTableViewCell
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

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            Titel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}