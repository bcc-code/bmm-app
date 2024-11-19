using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class ReadTranscriptionTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(ReadTranscriptionTableViewCell));
        private string _transcriptionText;
        private string _headerText;

        public ReadTranscriptionTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ReadTranscriptionTableViewCell, ReadTranscriptionsPO>();

                set.Bind(this)
                    .For(v => v.TranscriptionText)
                    .To(po => po.Text);
                
                set.Bind(TranscriptionsTextLabel)
                    .For(v => v.Alpha)
                    .To(po => po.IsHighlighted)
                    .WithConversion<IsHighlightedToAlphaValueConverter>();

                set.Bind(this)
                    .For(v => v.HeaderText)
                    .To(po => po.Header);

                set.Bind(this)
                    .For(v => v.BindTap())
                    .To(po => po.ItemClickedCommand);
                
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
            HeaderTextLabel.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);
            HeaderTextLabel.TextColor = AppColors.LabelFourColor;
        }
        
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                HeaderTextLabel.Text = value;
                HeaderTextLabel.SetHiddenIfNeeded(_headerText.IsNullOrEmpty());
            }
        }

        public string TranscriptionText
        {
            get => _transcriptionText;
            set
            {
                _transcriptionText = value;

                if (_transcriptionText.IsNullOrEmpty())
                {
                    TranscriptionsTextLabel.SetHiddenIfNeeded(true);
                    return;
                }

                TranscriptionsTextLabel.SetHiddenIfNeeded(false);

                var paragraphStyle = new NSMutableParagraphStyle();
                paragraphStyle.LineSpacing = 10;

                var attrString = new NSMutableAttributedString(_transcriptionText);
                attrString.AddAttribute(UIStringAttributeKey.ParagraphStyle, paragraphStyle, new NSRange(0, _transcriptionText.Length));
                attrString.AddAttribute(UIStringAttributeKey.ForegroundColor, AppColors.LabelOneColor, new NSRange(0, _transcriptionText.Length));
                attrString.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(16, UIFontWeight.Regular), new NSRange(0, _transcriptionText.Length));
                TranscriptionsTextLabel.AttributedText = attrString;
            }
        }

        protected override bool HasHighlightEffect => false;
    }
}