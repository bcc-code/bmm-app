using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class PlaybackHistoryViewController : BaseViewController<PlaybackHistoryViewModel>
    {
        public PlaybackHistoryViewController()
            : base(nameof(PlaybackHistoryViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(UINavigationController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new DocumentsTableViewSource(PlaybackHistoryTableView);

            var set = this.CreateBindingSet<PlaybackHistoryViewController, PlaybackHistoryViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand)
                .WithConversion<DocumentSelectedCommandValueConverter>();
            set.Apply();
        }
    }
}