using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.CollectionViewSource;
using BMM.UI.iOS.Delegates;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class AchievementsViewController : BaseViewController<AchievementsViewModel>
    {
        private ProfileAchievementsCollectionViewSource _source;

        public AchievementsViewController()
            : base(nameof(AchievementsViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _source = new ProfileAchievementsCollectionViewSource(AchievementsCollectionView);
            
            var set = this.CreateBindingSet<AchievementsViewController, AchievementsViewModel>();

            set.Bind(_source)
                .To(v => v.Achievements);
            
            set.Apply();

            AchievementsCollectionView.CollectionViewLayout = new ProfileAchievementsCollectionViewLayout()
            {
                SectionInset = new UIEdgeInsets(0, 16, 0, 16)
            };
            AchievementsCollectionView.Source = _source;
        }
    }
}