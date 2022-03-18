using BMM.UI.iOS.Utils;
using Foundation;
using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class ViewExtensions
    {
        /// <summary>
        /// This method should be used for changing Hidden state in views added to UIStackView
        /// as this property needs to be guarded of multiple set the same state.
        /// </summary>
        public static void SetHiddenIfNeeded(this UIView view, bool isHidden)
        {
            if (view.Hidden == isHidden)
                return;

            view.Hidden = isHidden;
        }
        
        public static void LoadXib(
            this UIView owner,
            bool shouldAttachConstraints = false,
            NSDictionary options = null)
        {
            var view = NibLoader.Load(owner.GetType().Name, owner.Bounds, owner, options);
            owner.AddSubview(view);

            if (!shouldAttachConstraints)
                return;

            view.TranslatesAutoresizingMaskIntoConstraints = false;
            owner.TranslatesAutoresizingMaskIntoConstraints = false;

            owner.AddConstraint(NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, owner, NSLayoutAttribute.Width, 1, 0));
            owner.AddConstraint(NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.Equal, owner, NSLayoutAttribute.Height, 1, 0));
            owner.AddConstraint(NSLayoutConstraint.Create(view, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, owner, NSLayoutAttribute.CenterX, 1, 0));
            owner.AddConstraint(NSLayoutConstraint.Create(view, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, owner, NSLayoutAttribute.CenterY, 1, 0));
        }
    }
}