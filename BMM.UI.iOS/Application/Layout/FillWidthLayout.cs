using System;
using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS
{
    public class FillWidthLayout: UICollectionViewFlowLayout
    {
        public override nfloat MinimumInteritemSpacing => 16;

        public override nfloat MinimumLineSpacing => 5;

        public override CGSize EstimatedItemSize => ItemSize;

        public override CGSize ItemSize => new CGSize(Width, Height);

        private nfloat Height => Width + 40f;

        private nfloat Width => (UIScreen.MainScreen.Bounds.Width - TotalHorizontalSpace)/Columns;

        private nfloat TotalHorizontalSpace => SectionInset.Left + SectionInset.Right + MinimumInteritemSpacing;

        private int Columns => 2;

        public FillWidthLayout(): base()
        {
            SectionInset = new UIEdgeInsets(16, 16, 16, 16);
            InvalidateLayout();
        }
    }
}
