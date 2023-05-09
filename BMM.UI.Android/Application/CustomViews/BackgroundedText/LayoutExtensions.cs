using Android.OS;
using Android.Text;

namespace BMM.UI.Droid.Application.CustomViews.BackgroundedText
{
    public static class LayoutExtensions
    {
        private const float DefaultLinespacingExtra = 0f;
        private const float DefaultLinespacingMultiplier = 1f;

        public static int GetLineBottomWithoutSpacing(this Layout layout, int line)
        {
            int lineBottom = layout.GetLineBottom(line);
            bool isLastLine = line == layout.LineCount - 1;

            int lineBottomWithoutSpacing;
            float lineSpacingExtra = layout.SpacingAdd;
            float lineSpacingMultiplier = layout.SpacingMultiplier;
            bool hasLineSpacing = lineSpacingExtra != DefaultLinespacingExtra || lineSpacingMultiplier != DefaultLinespacingMultiplier;

            if (!hasLineSpacing || isLastLine)
            {
                lineBottomWithoutSpacing = lineBottom;
            }
            else
            {
                float extra;
                if (lineSpacingMultiplier.CompareTo(DefaultLinespacingMultiplier) != 0)
                {
                    int lineHeight = layout.GetLineHeight(line);
                    extra = lineHeight - (lineHeight - lineSpacingExtra) / lineSpacingMultiplier;
                }
                else
                {
                    extra = lineSpacingExtra;
                }

                lineBottomWithoutSpacing = (int)(lineBottom - extra);
            }

            return lineBottomWithoutSpacing;
        }

        public static int GetLineHeight(this Layout layout, int line) => layout.GetLineTop(line + 1) - layout.GetLineTop(line);

        public static int GetLineTopWithoutPadding(this Layout layout, int line)
        {
            int lineTop = layout.GetLineTop(line);
            
            if (line == 0)
                lineTop -= layout.TopPadding;
            
            return lineTop;
        }

        public static int GetLineBottomWithoutPadding(this Layout layout, int line)
        {
            int lineBottom = layout.GetLineBottomWithoutSpacing(line);
            
            if (line == layout.LineCount - 1)
                lineBottom -= layout.BottomPadding;
            
            return lineBottom;
        }
    }
}
