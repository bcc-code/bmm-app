using BMM.UI.iOS.CustomViews.Swipes.Base;
using BMM.UI.iOS.Utils;

namespace BMM.UI.iOS.CustomViews.Swipes
{
    public partial class SwipeMenuSimpleItem : SwipeMenuBase
    {
        public SwipeMenuSimpleItem()
        {
            XibLoad();
        }

        private void XibLoad()
        {
            var view = NibLoader.Load(nameof(SwipeMenuSimpleItem), Bounds, this);
            AddSubview(view);
        }

        public override UILabel LabelTitle => SwipeLabel;
    }
}