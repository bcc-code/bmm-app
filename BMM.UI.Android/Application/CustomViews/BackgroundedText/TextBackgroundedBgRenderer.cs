using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;

namespace BMM.UI.Droid.Application.CustomViews.BackgroundedText
{
    public abstract class TextBackgroundedBgRenderer
    {
        public int HorizontalPadding { get; }
        public int VerticalPadding { get; }

        protected TextBackgroundedBgRenderer(int horizontalPadding, int verticalPadding)
        {
            HorizontalPadding = horizontalPadding;
            VerticalPadding = verticalPadding;
        }

        public abstract void Draw(Canvas canvas, Layout layout, int startLine, int endLine, int startOffset, int endOffset);

        protected int GetLineTop(Layout layout, int line)
        {
            return layout.GetLineTopWithoutPadding(line) - VerticalPadding;
        }

        protected int GetLineBottom(Layout layout, int line)
        {
            return layout.GetLineBottomWithoutPadding(line) + VerticalPadding;
        }
    }

    public class SingleLineRenderer : TextBackgroundedBgRenderer
    {
        private const int DefaultPadding = 5;
        public Drawable Drawable { get; }

        public SingleLineRenderer(int horizontalPadding, int verticalPadding, Drawable drawable)
            : base(horizontalPadding, verticalPadding)
        {
            Drawable = drawable;
        }

        public override void Draw(Canvas canvas, Layout layout, int startLine, int endLine, int startOffset, int endOffset)
        {
            var lineTop = GetLineTop(layout, startLine) - DefaultPadding;
            var lineBottom = GetLineBottom(layout, startLine) + DefaultPadding;

            var left = Math.Min(startOffset, endOffset) - DefaultPadding;
            var right = Math.Max(startOffset, endOffset) + DefaultPadding;

            Drawable.SetBounds(left,
                lineTop,
                right,
                lineBottom);
            Drawable.Draw(canvas);
        }
    }
}