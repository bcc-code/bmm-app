using System;
using System.ComponentModel;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
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
            AddNavigationBarItemForOptions();

            var source = new DocumentsTableViewSource(AlbumTable);

            TitleLabel.ApplyTextTheme(AppTheme.Heading1.Value);
            DescriptionLabel.ApplyTextTheme(AppTheme.Paragraph2.Value);
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonPrimary.Value);
            TrackCountLabel.ApplyTextTheme(AppTheme.Subtitle3.Value);

            var set = this.CreateBindingSet<AlbumViewController, AlbumViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Album).WithConversion<AlbumTitleConverter>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(AlbumCoverImageView).For(v => v.ImagePath).To(vm => vm.Album.Cover);
            set.Bind(TitleLabel).To(vm => vm.Album.Title);
            set.Bind(DescriptionLabel).To(vm => vm.Album.Description);
            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("ShufflePlay");
            set.Bind(ShuffleButton).For(v => v.Hidden).To(vm => vm.ShowShuffleButton).WithConversion<InvertedVisibilityConverter>();
            set.Bind(TrackCountLabel).To(vm => vm.TrackCountString);

            set.Apply();

            AlbumTable.ResizeHeaderView();

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }


        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AlbumTable.ResizeHeaderView();
            if (e.PropertyName == nameof(AlbumViewModel.ShowShuffleButton))
                StackViewToSeparatorConstraint.Constant = ViewModel.ShowShuffleButton ? 20 : 0;
        }


        private void AddNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                new UIImage("icon_options"),
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