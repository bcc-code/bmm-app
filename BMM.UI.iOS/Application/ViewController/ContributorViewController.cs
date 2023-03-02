using System.ComponentModel;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class ContributorViewController : BaseViewController<ContributorViewModel>
    {
        public ContributorViewController()
            : base(nameof(ContributorViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new DocumentsTableViewSource(TracksTable);

            var set = this.CreateBindingSet<ContributorViewController, ContributorViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Contributor).WithConversion<ContributorNameConverter>();
            set.Bind(source).To(vm => vm.Documents);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand);
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(s => s.IsFullyLoaded);
            
            set.Bind(CircleCoverImage).For(v => v.ImagePath).To(vm => vm.Contributor.Cover);
            set.Bind(TrackCountLabel).To(vm => vm.TrackCountString);
            set.Bind(NameLabel).To(vm => vm.Contributor.Name);
            set.Bind(ShuffleButton).To(vm => vm.PlayCommand);
            set.Bind(ShuffleButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.DocumentsViewModel_Shuffle]);
            
            set.Apply();

            TracksTable.ResizeHeaderView();
            SetThemes();
        }

        private void SetThemes()
        {
            NameLabel.ApplyTextTheme(AppTheme.Heading2);
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            TrackCountLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
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
            if (e.PropertyName == nameof(ContributorViewModel.Contributor))
                TracksTable.ResizeHeaderView();
        }
    }
}