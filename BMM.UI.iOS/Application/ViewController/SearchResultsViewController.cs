using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS
{
    public partial class SearchResultsViewController : BaseViewController<SearchResultsViewModel>
    {
        public SearchResultsViewController()
            : base(nameof(SearchResultsViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<SearchResultsViewController, SearchResultsViewModel>();
            
            var documentsTableViewSource = new DocumentsTableViewSource(ResultsTableView);
            
            set.Bind(documentsTableViewSource)
                .To(vm => vm.Documents);
            
            set.Bind(documentsTableViewSource)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand);
            
            set.Bind(documentsTableViewSource)
                .For(s => s.LoadMoreCommand)
                .To(s => s.LoadMoreCommand);
            
            set.Bind(documentsTableViewSource)
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
        
        private void SetThemes()
        {
            NoResultsTitle.ApplyTextTheme(AppTheme.Heading3);
            NoResultsMessage.ApplyTextTheme(AppTheme.Paragraph1Label2);
        }
    }
}