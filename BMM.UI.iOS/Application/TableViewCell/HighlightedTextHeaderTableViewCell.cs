using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Tracks;
using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class HighlightedTextHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(HighlightedTextHeaderTableViewCell));

        public HighlightedTextHeaderTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HighlightedTextHeaderTableViewCell, HighlightedTextHeaderPO>();

                set.Bind(HeaderLabel)
                    .To(vm => vm.HeaderText);

                set.Bind(ContentView)
                    .For(v => v.BindTap())
                    .To(po => po.ItemClickedCommand);
                
                set.Apply();
                SetThemes();
            });
        }

        private void SetThemes()
        {
            HeaderLabel.ApplyTextTheme(AppTheme.Paragraph2);
            HeaderLabel.TextColor = AppColors.UtilityAutoColor;
        }

        protected override bool HasHighlightEffect => false;
    }
}