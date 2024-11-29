using BMM.UI.iOS.CustomViews.Swipes.Base;
using BMM.UI.iOS.Utils;

namespace BMM.UI.iOS.CustomViews.Swipes
{
    public partial class SwipeMenuItem : SwipeMenuBase
    {
        public SwipeMenuItem()
        {
            XibLoad();
        }

        private void XibLoad()
        {
            var view = NibLoader.Load(nameof(SwipeMenuItem), Bounds, this);
            AddSubview(view);
        }

        public override UILabel LabelTitle => SwipeLabel;
        public override UIView ViewSeparator => SeparatorView;
        public override NSLayoutConstraint ConstraintSeparatorLeading => SeparatorLeadingConstraint;
        public override NSLayoutConstraint ConstraintSeparatorHeight => SeparatorHeightConstraint;
        public override UIView ContainerView => ContentContainer;
        public override int SeparatorVerticalMargin => 0;
    }
}