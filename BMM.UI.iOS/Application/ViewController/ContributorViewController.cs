using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using CoreGraphics;
using MvvmCross.Localization;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ContributorViewController : BaseViewController<ContributorViewModel>
    {
        public ContributorViewController()
            : base("ContributorViewController")
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new DocumentsTableViewSource(TracksTable);

            var set = this.CreateBindingSet<ContributorViewController, ContributorViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Contributor).WithConversion<ContributorNameConverter>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(s => s.IsFullyLoaded);
            set.Bind(CoverImage).For(v => v.ImagePath).To(vm => vm.Contributor.Cover);
            set.Bind(CircleCoverImage).For(v => v.ImagePath).To(vm => vm.Contributor.Cover);
            set.Bind(TrackTableTitle).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("HeaderText");

            set.Apply();

            TracksTable.ReloadData();
            blurBackground();
        }

        private void blurBackground()
        {
            var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
            var blurEffect = new UIVisualEffectView(blur);
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            var frame = new CGRect(0, 0, screenWidth, 520);
            blurEffect.Frame = frame;
            blurView.Add(blurEffect);
        }
    }
}