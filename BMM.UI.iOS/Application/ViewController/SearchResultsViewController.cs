using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS
{
    public partial class SearchResultsViewController : BaseViewController<SearchResultsViewModel>, IUITableViewDelegate
    {
        private DocumentsTableViewSource _documentsTableViewSource;

        public SearchResultsViewController()
            : base(nameof(SearchResultsViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ResultsTableView.Delegate = this;

            var set = this.CreateBindingSet<SearchResultsViewController, SearchResultsViewModel>();
            
            _documentsTableViewSource = new DocumentsTableViewSource(ResultsTableView);
            
            set.Bind(_documentsTableViewSource)
                .To(vm => vm.Documents);
            
            set.Bind(_documentsTableViewSource)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand);
            
            set.Bind(_documentsTableViewSource)
                .For(s => s.LoadMoreCommand)
                .To(s => s.LoadMoreCommand);
            
            set.Bind(_documentsTableViewSource)
                .For(s => s.IsFullyLoaded)
                .To(s => s.IsFullyLoaded);

            set.Bind(ResultsTableView)
                .For(v => v.BindVisible())
                .To(vm => vm.HasAnyItem);

            set.Bind(NoItemsLayer)
                .For(v => v.BindVisible())
                .To(vm => vm.ShowNoItemsInfo);

            set.Bind(NoResultsTitle)
                .To(vm => vm.TextSource[Translations.SearchViewModel_NoResults]);
            
            set.Bind(NoResultsMessage)
                .To(vm => vm.NoResultsDescriptionLabel);

            set.Bind(ActivityIndicator)
                .For(v => v.BindVisible())
                .To(vm => vm.IsLoading);

            set.Apply();
            SetThemes();
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            _documentsTableViewSource.ScrolledEvent += ResultsTableViewOnScrolled;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            _documentsTableViewSource.ScrolledEvent -= ResultsTableViewOnScrolled;
        }
        
        private void ResultsTableViewOnScrolled(object sender, EventArgs e)
        {
            ViewModel?.ClearFocusAction?.Invoke();
        }

        private void SetThemes()
        {
            NoResultsTitle.ApplyTextTheme(AppTheme.Heading3);
            NoResultsMessage.ApplyTextTheme(AppTheme.Paragraph1Label2);
        }
    }
}