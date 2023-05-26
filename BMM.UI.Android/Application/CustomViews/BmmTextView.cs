using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Widget;
using BMM.Core.Implementations.UI.StyledText;
using BMM.Core.Implementations.UI.StyledText.Enums;
using BMM.Core.Implementations.UI.StyledText.Interfaces;
using BMM.UI.Droid.Application.CustomViews.BackgroundedText;
using BMM.UI.Droid.Application.CustomViews.TextSpans;
using BMM.UI.Droid.Application.Extensions;
using FFImageLoading.Extensions;
using Java.Lang;
using Microsoft.IdentityModel.Tokens;
using static Android.Graphics.Paint;
using static Android.Icu.Text.ListFormatter;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.BmmTextView")]
    public partial class BmmTextView : AppCompatTextView
    {
        private SpannableString _spannableString;
        private StyledTextContainer _formattedText;
        private int _displayedTextLength;
        private bool _shouldAutoScaleFormattedText = true;
        private TextRoundedBgHelper _textRoundedBgHelper;

        public BmmTextView(Context context)
            : base(context)
        {
            Initialize(context, null);
        }

        public BmmTextView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(context, attrs);
        }

        public BmmTextView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context, attrs);
        }

        private void Initialize(Context context, IAttributeSet attrs)
        {
            SetIncludeFontPadding(false);
            var attributeReader = new TextBackgroundedAttributeReader(context, attrs);
            _textRoundedBgHelper = new TextRoundedBgHelper(
                attributeReader.HorizontalPadding,
                attributeReader.VerticalPadding,
                attributeReader.Drawable
            );
        }

        public StyledTextContainer StyledTextContainer
        {
            get => _formattedText;
            set
            {
                if (!_shouldAutoScaleFormattedText
                    && _formattedText?.FullText?.Length != value?.FullText?.Length)
                {
                    _shouldAutoScaleFormattedText = true;
                }

                _formattedText = value;
                ApplyStyleForFormattedText();
            }
        }

        private void ApplyStyleForFormattedText()
        {
            _spannableString = new SpannableString(_formattedText.FullText);

            SetColorsForFormattedText();
            SetTextStyleForFormattedText();

            SetTextForFormattedText(_spannableString, BufferType.Spannable);
            SetStyle();
        }

        private void SetStyle()
        {
            TextSize = StyledTextContainer.FontSize.DpToPixels();
            SetLineSpacing(0, StyledTextContainer.LineHeightMultiplier);
        }

        private void SetTextForFormattedText(ICharSequence? text, BufferType? type)
        {
            if (text == null)
            {
                SetText(text, type);
                return;
            }

            if (Text == text.ToString())
                return;

            SetText(text, type);
        }

        private void SetColorsForFormattedText()
        {
            SetSpan((styledText) => new ForegroundColorSpan(GetForegroundColorForFormattedText(styledText)));
            SetSpan((styledText) => new CustomBackgroundColorSpan(GetBackgroundColorForFormattedText(styledText)));
        }

        private Color GetBackgroundColorForFormattedText(IStyledText styledText)
        {
            if (StyledTextContainer.ShouldColorBackground)
            {
                return styledText.Style == TextStyle.Default
                    ? Color.Transparent
                    : Context.GetColorFromResource(Resource.Color.utility_auto_color).WithAlpha(0.3f);
            }

            return Color.Transparent;
        }

        protected virtual Color GetForegroundColorForFormattedText(IStyledText styledText)
        {
            if (StyledTextContainer.ShouldColorBackground)
            {
                return styledText.Style == TextStyle.Default
                    ? Context.GetColorFromResource(Resource.Color.label_two_color)
                    : Context.GetColorFromResource(Resource.Color.label_one_color);
            }

            return styledText.Style == TextStyle.Default
                ? Context.GetColorFromResource(Resource.Color.label_three_color)
                : Context.GetColorFromResource(Resource.Color.utility_auto_color);
        }

        private void SetTextStyleForFormattedText()
        {
            SetSpan(_ =>
            {
                int scaledFontSize = (int)TypedValue.ApplyDimension(
                    ComplexUnitType.Sp,
                    StyledTextContainer.FontSize,
                    Context.Resources.DisplayMetrics);

                return new AbsoluteSizeSpan(scaledFontSize);
            });
        }

        private void SetSpan(Func<IStyledText, Java.Lang.Object> spanObject)
        {
            if (_spannableString == null || spanObject == null)
                return;

            int offset = 0;
            int start, end;
            int fullTextLength = _spannableString.Length();

            foreach (var styledText in _formattedText.StyledTexts)
            {
                if (styledText.Content.IsNullOrEmpty())
                    continue;

                start = offset;
                end = start + styledText.Content.Length;

                if (start >= fullTextLength || end > fullTextLength)
                    break;

                var span = spanObject.Invoke(styledText);
                if (span != null)
                {
                    _spannableString.SetSpan(
                        span,
                        start,
                        end,
                        SpanTypes.ExclusiveExclusive);
                }

                offset += styledText.Content.Length;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (Layout != null)
            {
                _textRoundedBgHelper.Draw(canvas, _spannableString, Layout);
            }
            base.OnDraw(canvas);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            CheckIfTextNeedsTruncation();
        }

        private void CheckIfTextNeedsTruncation()
        {
            if (Layout == null || Ellipsize == null)
                return;

            bool textNeedsTruncation = Layout.LineCount > MaxLines;
            if (!textNeedsTruncation)
                return;

            int textLength = GetVisibleTextLengthWithSpaceForEllipsis();
            if (textLength == _displayedTextLength)
                return;

            _displayedTextLength = textLength;
            ApplyStyleForFormattedText();
        }

        private int GetVisibleTextLengthWithSpaceForEllipsis()
        {
            int textLength = Layout.GetLineVisibleEnd(MaxLines - 1);
            if (textLength > 0 && Ellipsize == TextUtils.TruncateAt.End)
                textLength -= 1;
            return textLength;
        }
    }
}