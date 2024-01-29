using BMM.Core.Models.POs.Other;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class HeaderCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new(nameof(HeaderCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(HeaderCollectionViewCell), NSBundle.MainBundle);

        public HeaderCollectionViewCell(IntPtr handle) : base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HeaderCollectionViewCell, ChapterHeaderPO>();
                
                set.Bind(HeaderText)
                    .To(po => po.ChapterHeader.Title);
                
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            HeaderText.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}