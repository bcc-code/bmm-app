using UIKit;

namespace BMM.UI.iOS.Constants
{
    public class ButtonTheme
    {
        public TextTheme TextTheme { get; set; }
        public UIColor ButtonColor { get; set; }
        public UIEdgeInsets ImageEdgeInsets { get; set; }
        public UIEdgeInsets ContentEdgeInsets { get; set; }
        public UIColor IconTint { get; set; }
        public bool HasRoundedCorners { get; set; }
    }

    public class StandardButtonTheme : ButtonTheme
    {
        public StandardButtonTheme()
        {
            ImageEdgeInsets = new UIEdgeInsets(0, -6, 0, 0);
            ContentEdgeInsets = new UIEdgeInsets(6, 16, 6, 16);
            HasRoundedCorners = true;
        }
    }
}