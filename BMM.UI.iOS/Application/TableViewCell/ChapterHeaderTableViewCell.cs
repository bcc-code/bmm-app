using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class ChapterHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(ChapterHeaderTableViewCell));

        public ChapterHeaderTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ChapterHeaderTableViewCell, ChapterHeaderPO>();
                set.Bind(Titel).To(po => po.ChapterHeader.Title);
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