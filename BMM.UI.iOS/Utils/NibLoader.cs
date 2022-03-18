using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BMM.UI.iOS.Utils
{
    public static class NibLoader
    {
        public static UIView Load(string nibName, CGRect bounds, NSObject owner, NSDictionary options = null)
        {
            var nibsArray = NSBundle.MainBundle.LoadNib(nibName, owner, options);
            var view = Runtime.GetNSObject<UIView>(nibsArray.ValueAt(0));
            view.Frame = bounds;
            return view;
        }
    }
}