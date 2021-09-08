﻿using System.ComponentModel;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;

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

            NameLabel.ApplyTextTheme(AppTheme.Heading2.Value);
            PlayButton.ApplyButtonStyle(AppTheme.ButtonPrimary.Value);
            TrackCountLabel.ApplyTextTheme(AppTheme.Subtitle3.Value);

            var set = this.CreateBindingSet<ContributorViewController, ContributorViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Contributor).WithConversion<ContributorNameConverter>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(s => s.IsFullyLoaded);
            set.Bind(CircleCoverImage).For(v => v.ImagePath).To(vm => vm.Contributor.Cover);
            set.Bind(TrackCountLabel).To(vm => vm.TrackCountString);
            set.Bind(NameLabel).To(vm => vm.Contributor.Name);
            set.Bind(PlayButton).To(vm => vm.PlayCommand);
            set.Bind(PlayButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.DocumentsViewModel_Play);

            set.Apply();

            TracksTable.ResizeHeaderView();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ContributorViewModel.Contributor))
                TracksTable.ResizeHeaderView();
        }
    }
}