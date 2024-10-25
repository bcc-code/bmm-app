using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.Core.Translation;
using BMM.UI.iOS.CollectionViewSource;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Delegates;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;

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

            set.Bind(AchievementsCollectionView)
                .For(v => v.BindVisible())
                .To(vm => vm.AreAchievementsVisible);
            
            set.Bind(NoAchievementsStackView)
                .For(v => v.BindVisible())
                .To(vm => vm.AreAchievementsVisible)
                .WithConversion<InvertedBoolConverter>();

            set.Bind(NoAchievementsTitle)
                .To(vm => vm.TextSource[Translations.AchievementsViewModel_EmptyTitle]);
            
            set.Bind(NoAchievementsSubtitle)
                .To(vm => vm.TextSource[Translations.AchievementsViewModel_EmptySubtitle]);
            
            set.Apply();

            AchievementsCollectionView.CollectionViewLayout = new ProfileAchievementsCollectionViewLayout()
            {
                SectionInset = new UIEdgeInsets(0, 16, 0, 16)
            };
            AchievementsCollectionView.Source = _source;

            SetThemes();
        }

        private void SetThemes()
        {
            NoAchievementsTitle.ApplyTextTheme(AppTheme.Heading3);
            NoAchievementsSubtitle.ApplyTextTheme(AppTheme.Paragraph1Label2);
        }
    }
}