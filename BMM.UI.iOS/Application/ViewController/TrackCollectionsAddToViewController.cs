using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using System;
using BMM.Core.ValueConverters;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TrackCollectionsAddToViewController : BaseViewController<TrackCollectionsAddToViewModel>
    {
        public TrackCollectionsAddToViewController() : base("TrackCollectionsAddToViewController")
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new DocumentsTableViewSource(TrackCollectionsTableView);

            var set = this.CreateBindingSet<TrackCollectionsAddToViewController, TrackCollectionsAddToViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();

            set.Apply();

            AddNavigationBarItemForAddPlaylist();

            TrackCollectionsTableView.ReloadData();
        }

        private void AddNavigationBarItemForAddPlaylist()
        {
            var sidebarButton = new UIBarButtonItem(
                UIImage.FromFile("icon_add_static.png"),
                UIBarButtonItemStyle.Plain,
                (object sender, EventArgs e) =>
                {
                    ViewModel.CreatePlaylistCommand.Execute();
                }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }
    }
}