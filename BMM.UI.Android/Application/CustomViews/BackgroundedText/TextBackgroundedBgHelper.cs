using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Style;
using BMM.UI.Droid.Application.CustomViews.TextSpans;

namespace BMM.UI.Droid.Application.CustomViews.BackgroundedText
{
    public class TextRoundedBgHelper
    {
        private readonly int _horizontalPadding;
        private readonly TextBackgroundedBgRenderer _singleLineRenderer;

        public TextRoundedBgHelper(int horizontalPadding, int verticalPadding,
                                   Drawable drawable)
        {
            this._horizontalPadding = horizontalPadding;
            _singleLineRenderer = new SingleLineRenderer(horizontalPadding, verticalPadding, drawable);
        }

        public void Draw(Canvas canvas, ISpanned text, Layout layout)
        {
            var spans = text.GetSpans(0, text.Length(), Java.Lang.Class.FromType(typeof(CustomBackgroundColorSpan)));
            foreach (var span in spans)
            {
                if (((CustomBackgroundColorSpan)span).Color == Color.Transparent)
                    continue;

                int spanStart = text.GetSpanStart(span);
                int spanEnd = text.GetSpanEnd(span);
                int startLine = layout.GetLineForOffset(spanStart);
                int endLine = layout.GetLineForOffset(spanEnd);

                int startOffset = (int)(layout.GetPrimaryHorizontal(spanStart)
                                        + -1 * (int)layout.GetParagraphDirection(startLine) * _horizontalPadding);
                int endOffset = (int)(layout.GetPrimaryHorizontal(spanEnd)
                                      + (int)layout.GetParagraphDirection(endLine) * _horizontalPadding);

                _singleLineRenderer.Draw(canvas, layout, startLine, endLine, startOffset, endOffset);
            }
        }
    }
}
