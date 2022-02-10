namespace BMM.UI.iOS.Utils.ColorPalette
{
    public class PixelColor
    {
        public YUVColor Color { get; set; }
        public int NumberOfOccurrences { get; set; }

        public override string ToString ()
        {
            return $"[PixelColor: Color={Color}, NumberOfOccurrences={NumberOfOccurrences}]";
        }
    }
}