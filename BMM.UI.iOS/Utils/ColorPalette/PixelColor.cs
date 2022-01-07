namespace BMM.UI.iOS.Utils.ColorPalette
{
    public class PixelColor
    {
        // Color of one pixel
        public YUVColor Color { get; set; }

        // Number of its occurrences in the image
        public int NumberOfOccurrences { get; set; }

        public override string ToString ()
        {
            return $"[PixelColor: Color={Color}, NumberOfOccurrences={NumberOfOccurrences}]";
        }
    }
}