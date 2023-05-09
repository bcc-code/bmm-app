using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace BMM.UI.Droid.Application.CustomViews.TextSpans;

public class CustomBackgroundColorSpan : CharacterStyle
{
    public Color Color { get; }

    public CustomBackgroundColorSpan(Color color)
    {
        Color = color;
    }

    public override void UpdateDrawState(TextPaint tp)
    {
    }
}