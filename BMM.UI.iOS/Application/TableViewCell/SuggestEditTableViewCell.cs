using BMM.Core.Models.POs.SuggestEdit;
using MvvmCross.Binding.BindingContext;
using BMM.UI.iOS.Constants;
using CoreFoundation;

namespace BMM.UI.iOS
{
    public partial class SuggestEditTableViewCell : BaseBMMTableViewCell, IUITextViewDelegate
    {
        public static readonly NSString Key = new(nameof(SuggestEditTableViewCell));

        public SuggestEditTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SuggestEditTableViewCell, TranscriptionPO>();

                set.Bind(TranscriptionTextView)
                    .For(v => v.Text)
                    .To(po => po.Text);
                
                set.Apply();

                TranscriptionTextView!.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Medium);
                TranscriptionTextView!.TextColor = AppColors.LabelTwoColor;
                TranscriptionTextView.Delegate = this;
            });
        }

        [Export("textViewDidChange:")]
        public void Changed(UITextView textView)
        {
            if (Superview is UITableView tableView)
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    tableView.BeginUpdates();
                    tableView.EndUpdates();
                });
            };
        }

        protected override bool HasHighlightEffect => false;
    }
}