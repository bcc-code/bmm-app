using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using CoreGraphics;
using System.Linq;
using BMM.Core.ValueConverters;
using MvvmCross.Localization;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class AlbumViewController : BaseViewController<AlbumViewModel>
    {
        public AlbumViewController()
            : base("AlbumViewController")
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            addNavigationBarItemForOptions();

            var source = new DocumentsTableViewSource(AlbumTable);

            var set = this.CreateBindingSet<AlbumViewController, AlbumViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Album).WithConversion<AlbumTitleConverter>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(AlbumCoverImageView).For(v => v.ImagePath).To(vm => vm.Album.Cover);
            set.Bind(AlbumBlurCoverImage).For(v => v.ImagePath).To(vm => vm.Album.Cover);
            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("ShufflePlay");
            set.Bind(ShuffleButton).For("Visibility").To(vm => vm.IsShuffleAvailable);
            set.Apply();

            // Hide the ShuffleButton and adjust TableHeaderView based on if there are track-documents in the listing
            ((AlbumViewModel)DataContext).Documents.CollectionChanged += (sender, e) =>
            {
                ShuffleButton.Hidden = !((AlbumViewModel)DataContext).Documents.Any(t => t.DocumentType == DocumentType.Track);

                var headerFrame = AlbumHeaderView.Frame;
                var size = headerFrame.Size;
                // If we'd remove the ShuffleButton we could use AlbumHeaderView.SystemLayoutSizeFittingSize() ... sizes are calculated and fixed in the design
                size.Height = (ShuffleButton.Hidden ? 220 : 270);
                headerFrame.Size = size;
                AlbumHeaderView.Frame = headerFrame;
                AlbumTable.TableHeaderView = AlbumHeaderView;
                AlbumHeaderView.AccessibilityIdentifier = "album_header";
            };

            AlbumTable.ReloadData();
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

        private void addNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                UIImage.FromFile("icon_topbar_options_static.png"),
                UIBarButtonItemStyle.Plain,
                (object sender, EventArgs e) =>
                {
                    ViewModel.OptionCommand.Execute(ViewModel.Album);
                }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }
    }
}