using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class AchievementsViewController : BaseViewController<AchievementsViewModel>
    {
        public AchievementsViewController()
            : base(nameof(AchievementsViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<AchievementsViewController, AchievementsViewModel>();
            set.Apply();
        }
    }
}