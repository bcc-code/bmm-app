using UIKit;

namespace BMM.UI.iOS.Layout
{
    public sealed class ControllersCollectionViewLayout : UICollectionViewFlowLayout
    {
        public ControllersCollectionViewLayout()
        {
            SectionInset = UIEdgeInsets.Zero;
            MinimumInteritemSpacing = 0;
            MinimumLineSpacing = 0;
        }
    }
}