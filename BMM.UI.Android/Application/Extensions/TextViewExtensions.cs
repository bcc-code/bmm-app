using Android.Graphics;
using Android.Text;
using Android.Widget;
using FFImageLoading.Extensions;
using Java.Lang;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class TextViewExtensions
    {
        public static int CalculateRequiredTextViewSize(
            this TextView textView,
            float fontSize,
            float fontSizeAdjustment = 0)
        {
            if (string.IsNullOrEmpty(textView.Text))
                return 0;

            var textPaint = textView.CreateTextPaint(fontSize, fontSizeAdjustment);
            var bounds = GetTextBounds(textPaint, textView.Text);

            int measuredTextWidth = ((int)Math.Ceil(textPaint.MeasureText(textView.Text))).DpToPixels();
            int boundedTextWidth = (bounds.Width()).DpToPixels();

            int textWidth = Math.Max(measuredTextWidth, boundedTextWidth);
            return textWidth;
        }
        
        private static Rect GetTextBounds(TextPaint textPaint, string text)
        {
            var bounds = new Rect();
            textPaint.GetTextBounds(text, 0, text.Length, bounds);
            return bounds;
        }
        
        private static TextPaint CreateTextPaint(this TextView textView, float fontSize, float fontSizeAdjustment)
        {
            var textPaint = new TextPaint();
            textPaint.SetTypeface(textView.Typeface);
            textPaint.TextSize = fontSize + fontSizeAdjustment;
            return textPaint;
        }
    }
}