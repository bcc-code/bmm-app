using System;
using System.ComponentModel;
using BMM.Core.Implementations.UI.StyledText;
using BMM.Core.Implementations.UI.StyledText.Enums;
using BMM.Core.Implementations.UI.StyledText.Interfaces;
using BMM.UI.iOS.Constants;
using CoreGraphics;
using Foundation;
using Microsoft.IdentityModel.Tokens;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    [Register(nameof(BmmFormattedLabel)), DesignTimeVisible(true)]
    public class BmmFormattedLabel : UILabel
    {
        public BmmFormattedLabel()
        {
        }

        public BmmFormattedLabel(NSCoder coder) : base(coder)
        {
        }

        protected BmmFormattedLabel(NSObjectFlag t) : base(t)
        {
        }

        protected internal BmmFormattedLabel(IntPtr handle) : base(handle)
        {
        }

        public BmmFormattedLabel(CGRect frame) : base(frame)
        {
        }
        
        private NSMutableAttributedString _mutableAttributedText;
        private StyledTextContainer _styledTextContainer;

        public StyledTextContainer StyledTextContainer
        {
            get => _styledTextContainer;
            set
            {
                _styledTextContainer = value;
                ApplyStyleForFormattedText();
            }
        }

        private void ApplyStyleForFormattedText()
        {
            _mutableAttributedText = new NSMutableAttributedString(_styledTextContainer.FullText);
            SetColorsForFormattedText();
        }

        private void SetColorsForFormattedText()
        {
            SetAttribute(
                UIStringAttributeKey.BackgroundColor,
                (styledTextBase) =>
                {
                    if (StyledTextContainer.ShouldColorBackground)
                    {
                        return styledTextBase.Style == TextStyle.Default
                            ? UIColor.Clear
                            : AppColors.UtilityAutoColor.ColorWithAlpha(0.3f);
                    }

                    return UIColor.Clear;
                });

            SetAttribute(
                UIStringAttributeKey.ForegroundColor,
                styledTextBase =>
                {
                    if (StyledTextContainer.ShouldColorBackground)
                    {
                        return styledTextBase.Style == TextStyle.Default
                            ? AppColors.LabelSecondaryColor
                            : AppColors.LabelPrimaryColor;
                    }
                    
                    return styledTextBase.Style == TextStyle.Default
                        ? AppColors.LabelTertiaryColor
                        : AppColors.UtilityAutoColor;
                });
            
            Font = UIFont.SystemFontOfSize(StyledTextContainer.FontSize, UIFontWeight.Regular);

            var paragraphStyle = new NSMutableParagraphStyle();
            paragraphStyle.LineHeightMultiple = StyledTextContainer.LineHeightMultiplier;
            
            _mutableAttributedText.AddAttribute(
                UIStringAttributeKey.ParagraphStyle,
                paragraphStyle,
                new NSRange(0, _mutableAttributedText.Length));
            
            AttributedText = _mutableAttributedText;
        }

        private void SetAttribute(NSString attributeKey, Func<IStyledText, NSObject> attributeValue)
        {
            if (_mutableAttributedText == null || attributeKey == null || attributeValue == null)
                return;

            int offset = 0;
            foreach (var styledText in _styledTextContainer.StyledTexts)
            {
                if (styledText.Content.IsNullOrEmpty())
                    continue;

                int end = offset + styledText.Content.Length;
                if (offset >= _mutableAttributedText.Length
                    || end > _mutableAttributedText.Length)
                    break;

                var value = attributeValue.Invoke(styledText);

                if (attributeKey == UIStringAttributeKey.Attachment)
                {
                    if (value is NSAttributedString attributedStringWithAttachment)
                    {
                        _mutableAttributedText.Replace(
                            new NSRange(offset, styledText.Content.Length),
                            attributedStringWithAttachment);
                    }
                }
                else if (value != null)
                {
                    _mutableAttributedText.AddAttribute(
                        attributeKey,
                        value,
                        new NSRange(offset, styledText.Content.Length));
                }

                offset += styledText.Content.Length;
            }
        }
    }
}
