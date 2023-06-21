using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS
{
    public partial class ExploreRecentSpeechesViewController : BaseViewController<ExploreRecentSpeechesViewModel>
    {
        public ExploreRecentSpeechesViewController()
            : base(nameof(ExploreRecentSpeechesViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            TrackTableView.RefreshControl = refreshControl;

            var source = new DocumentsTableViewSource(TrackTableView);

            var set = this.CreateBindingSet<ExploreRecentSpeechesViewController, ExploreRecentSpeechesViewModel>();
            set.Bind(source).To(vm => vm.Documents);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand);
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(s => s.IsFullyLoaded);

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Apply();

            TrackTableView.ReloadData();
        }
    }
}