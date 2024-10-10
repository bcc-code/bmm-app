using System;
using System.ComponentModel;
using BMM.Core.Constants;
using BMM.Core.Translation;
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
            : base(nameof(AlbumViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddNavigationBarItemForOptions();

            var source = new DocumentsTableViewSource(AlbumTable);

            var set = this.CreateBindingSet<AlbumViewController, AlbumViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Album).WithConversion<AlbumTitleConverter>();
            set.Bind(source).To(vm => vm.Documents);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedBoolConverter>();
            set.Bind(AlbumCoverImageView).For(v => v.ImagePath).To(vm => vm.Album.Cover);
            set.Bind(TitleLabel).To(vm => vm.Album.Title);
            set.Bind(DescriptionLabel).To(vm => vm.Album.Description);
            set.Bind(PlayButton).To(vm => vm.PlayCommand);
            set.Bind(PlayButton).For(v => v.BindTitle()).To(vm => vm.PlayButtonText);
            set.Bind(PlayButton).For(v => v.BindVisible()).To(vm => vm.ShowPlayButton);
            set.Bind(TrackCountLabel).To(vm => vm.TrackCountString);

            DownloadButton.DownloadedImage = UIImage.FromBundle(ImageResourceNames.IconTick.ToStandardIosImageName());
            DownloadButton.NormalStateImage = UIImage.FromBundle(ImageResourceNames.IconDownload.ToStandardIosImageName());
            set.Bind(DownloadButton).To(vm => vm.ToggleOfflineCommand);
            set.Bind(DownloadButton).For(v => v.Label).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.TrackCollectionViewModel_AvailableOfflineDownload);
            set.Bind(DownloadButton).For(v => v.IsDownloading).To(vm => vm.IsDownloading);
            set.Bind(DownloadButton).For(v => v.IsDownloaded).To(vm => vm.IsDownloaded);
            set.Bind(DownloadButton).For(v => v.DownloadProgress).To(vm => vm.DownloadStatus);
            set.Bind(DownloadButton).For(v => v.BindVisible()).To(vm => vm.ShowDownloadButtons);
            
            set.Apply();

            SetThemes();
            AlbumTable.ResizeHeaderView();
            DownloadButton.UpdateCurrentState(true);
        }

        private void SetThemes()
        {
            TitleLabel.ApplyTextTheme(AppTheme.Heading2);
            DescriptionLabel.ApplyTextTheme(AppTheme.Paragraph2);
            PlayButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            TrackCountLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            DownloadButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AlbumTable.ResizeHeaderView();
            if (e.PropertyName == nameof(AlbumViewModel.ShowPlayButton))
            {
                StackViewToSeparatorConstraint.Constant = ViewModel.ShowPlayButton ? StackViewToSeparatorConstraint.Constant : 0;
                ButtonStackViewHeight.Constant = ViewModel.ShowPlayButton ? ButtonStackViewHeight.Constant : 0;
            }
        }

        private void AddNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                new UIImage("icon_options"),
                UIBarButtonItemStyle.Plain,
                (sender, e) =>
                {
                    ViewModel.OptionCommand.Execute(ViewModel.Album);
                }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }
    }
}