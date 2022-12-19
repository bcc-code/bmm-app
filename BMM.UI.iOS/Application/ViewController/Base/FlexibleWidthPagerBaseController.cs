using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.UI.iOS.CollectionTableViewCell;
using BMM.UI.iOS.Layout;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public abstract class FlexibleWidthPagerBaseController<TViewModel, TItem>
        : ViewPagerBaseController<TViewModel, TItem>
        where TViewModel : BaseViewModel, ICollectionViewModel<TItem>
    {
        private TopBarFlexibleWidthCollectionViewLayout _topBarFlexibleCollectionViewLayout;

        protected FlexibleWidthPagerBaseController(string nibName) : base(nibName)
        {
        }

        protected override UICollectionViewFlowLayout TopBarCollectionViewLayout
            => _topBarFlexibleCollectionViewLayout ??= new TopBarFlexibleWidthCollectionViewLayout();

        protected override NSString TopBarCollectionViewCellKey => TopBarFlexibleWidthTitleCollectionViewCell.Key;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TopBarCollectionView.RegisterNibForCell(
                TopBarFlexibleWidthTitleCollectionViewCell.Nib,
                TopBarFlexibleWidthTitleCollectionViewCell.Key);
        }
    }
}