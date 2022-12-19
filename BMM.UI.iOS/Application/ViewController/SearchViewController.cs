using System;
using System.ComponentModel;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Combiners;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;
using AppTheme = BMM.UI.iOS.Constants.AppTheme;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = Translations.MenuViewModel_Search, TabIconName = "icon_search", TabSelectedIconName = "icon_search_active", WrapInNavigationController = false)]
    public partial class SearchViewController : FlexibleWidthPagerBaseController<SearchViewModel, SearchResultsViewModel>
    {
        public SearchViewController()
            : base(nameof(SearchViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        protected override UIView HostViewForPager => ContainerView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController!.NavigationBarHidden = true;

            SearchTextField.ShouldReturn += textField =>
            {
                textField.ResignFirstResponder();
                ViewModel.SearchCommand.Execute();
                return true;
            };
            
            SetThemes();
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
            if (e.PropertyName == nameof(ViewModel.HasAnyHistoryEntry) || e.PropertyName == nameof(ViewModel.IsHistoryVisible))
                SearchBarBottomSeparator.Hidden = !ViewModel.IsHistoryVisible;
        }

        protected override void Bind(MvxFluentBindingDescriptionSet<BaseViewController<SearchViewModel>, SearchViewModel> set)
        {
            base.Bind(set);
            
            set.Bind(CancelButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.UserDialogs_Cancel]);

            set.Bind(CancelButton)
                .To(vm => vm.ClearCommand);
            
            set.Bind(ClearHistoryButton)
                .For(v => v.BindTap())
                .To(vm => vm.DeleteHistoryCommand);

            set.Bind(SearchTextField)
                .For(v => v.Text)
                .To(vm => vm.SearchTerm);
            
            set.Bind(SearchTextField)
                .For(v => v.Placeholder)
                .To(vm => vm.TextSource[Translations.SearchViewModel_SearchHint]);
            
            set.Bind(RecentSearchesLabel)
                .To(vm => vm.TextSource[Translations.SearchViewModel_SearchHistory]);
            
            set.Bind(WelcomeTitleLabel)
                .To(vm => vm.TextSource[Translations.SearchViewModel_WelcomeTitle]);
            
            set.Bind(WelcomeSubtitleLabel)
                .To(vm => vm.TextSource[Translations.SearchViewModel_WelcomeSubTitle]);
            
            var historyTable = new MvxSimpleTableViewSource(RecentSearchesTableView, SearchSuggestionTableViewCell.Key);

            set.Bind(historyTable)
                .For(v => v.ItemsSource)
                .To(vm => vm.SearchHistory);
            
            set.Bind(historyTable)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.SearchByTermCommand);
            
            set.Bind(RecentSearchesLayer)
                .For(s => s.BindVisible())
                .To(s => s.IsHistoryVisible);
            
            set.Bind(WelcomeLayer)
                .For(s => s.Hidden)
                .To(s => s.HasAnyHistoryEntry);

            set.Bind(SearchBarBottomSeparator)
                .For(s => s.BindVisible())
                .To(s => s.HasAnyHistoryEntry);
            
            RecentSearchesTableView.Source = historyTable;
        }

        private void SetThemes()
        {
            CancelButton.ApplyButtonStyle(AppTheme.CancelSearchButton);
            RecentSearchesLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            WelcomeTitleLabel.ApplyTextTheme(AppTheme.Heading3);
            WelcomeSubtitleLabel.ApplyTextTheme(AppTheme.Paragraph1Label1);
            SearchTextField.Font = Typography.Subtitle2.Value;
            SearchTextField.TextColor = AppColors.LabelPrimaryColor;
        }

        protected override MvxViewController CreateOrRefreshViewController(object item, MvxViewController existingController = null)
        {
            var vm = (SearchResultsViewModel)item;
            if (existingController is SearchResultsViewController searchResultsViewController)
            {
                searchResultsViewController.ViewModel = vm;
                return existingController;
            }

            return new SearchResultsViewController
            {
                ViewModel = vm
            };
        }
    }
}