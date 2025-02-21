using BMM.UI.iOS.Utils;
using CoreAnimation;
using Foundation;
using UIKit;
using CGSize = CoreGraphics.CGSize;

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

        public static void ApplyRoundedCorners(
            this UIView view, 
            float topLeftRadius,
            float bottomLeftRadius, 
            float topRightRadius,
            float bottomRightRadius)
        {
            var rect = view.Bounds;
            var path = new UIBezierPath();

            path.MoveTo(topLeftRadius > 0
                ? new CGPoint(rect.X + topLeftRadius, rect.Y)
                : new CGPoint(rect.X, rect.Y));

            path.AddLineTo(new CGPoint(rect.Right - topRightRadius, rect.Y));

            if (topRightRadius > 0)
            {
                path.AddArc(new CGPoint(rect.Right - topRightRadius, rect.Y + topRightRadius), 
                    topRightRadius, (float)(3 * Math.PI / 2), 0, true);
            }

            path.AddLineTo(new CGPoint(rect.Right, rect.Bottom - bottomRightRadius));

            if (bottomRightRadius > 0)
            {
                path.AddArc(new CGPoint(rect.Right - bottomRightRadius, rect.Bottom - bottomRightRadius), 
                    bottomRightRadius, 0, (float)(Math.PI / 2), true);
            }

            path.AddLineTo(new CGPoint(rect.X + bottomLeftRadius, rect.Bottom));

            if (bottomLeftRadius > 0)
            {
                path.AddArc(new CGPoint(rect.X + bottomLeftRadius, rect.Bottom - bottomLeftRadius), 
                    bottomLeftRadius, (float)(Math.PI / 2), (float)Math.PI, true);
            }

            path.AddLineTo(new CGPoint(rect.X, rect.Y + topLeftRadius));

            if (topLeftRadius > 0)
            {
                path.AddArc(new CGPoint(rect.X + topLeftRadius, rect.Y + topLeftRadius), 
                    topLeftRadius, (float)Math.PI, (float)(3 * Math.PI / 2), true);
            }

            path.ClosePath();

            var maskLayer = new CAShapeLayer
            {
                Path = path.CGPath
            };

            view.Layer.Mask = maskLayer;
        }

        public static void RoundLeftCorners(this UIView view, float radius)
        {
            var path = UIBezierPath.FromRoundedRect(view.Bounds,
                UIRectCorner.TopLeft | UIRectCorner.BottomLeft,
                new CGSize(radius, radius));

            var mask = new CAShapeLayer
            {
                Path = path.CGPath
            };

            view.Layer.Mask = mask;
        }
        
        public static void RoundRightCorners(this UIView view, float radius)
        {
            var path = UIBezierPath.FromRoundedRect(view.Bounds,
                UIRectCorner.TopRight | UIRectCorner.BottomRight,
                new CGSize(radius, radius));

            var mask = new CAShapeLayer
            {
                Path = path.CGPath
            };

            view.Layer.Mask = mask;
        }
    }
}