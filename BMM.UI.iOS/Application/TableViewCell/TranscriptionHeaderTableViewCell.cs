using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class TranscriptionHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(TranscriptionHeaderTableViewCell));

        public TranscriptionHeaderTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TranscriptionHeaderTableViewCell, TranscriptionHeaderPO>();

                set.Bind(TitleLabel)
                    .To(po => po.Title);
                
                set.Bind(SubtitleLabel)
                    .To(po => po.Subtitle);
                
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
            TitleLabel.ApplyTextTheme(AppTheme.Heading3);
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph2Label3);
        }

        protected override bool HasHighlightEffect => false;
    }
}