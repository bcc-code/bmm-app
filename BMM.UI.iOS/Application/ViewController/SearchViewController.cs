using System;
using System.ComponentModel;
using BMM.Core.Constants;
using BMM.Core.Interactions.Base;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Utils;
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
        private const int TopStackViewTrailingValue = 16;
        private bool _isHistoryVisible;
        private IBmmInteraction _removeFocusOnSearchInteraction;

        public SearchViewController()
            : base(nameof(SearchViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        protected override UIView HostViewForPager => ContainerView;

        public IBmmInteraction RemoveFocusOnSearchInteraction
        {
            get => _removeFocusOnSearchInteraction;
            set
            {
                if (_removeFocusOnSearchInteraction != null)
                    _removeFocusOnSearchInteraction.Requested -= OnRemoveFocusRequested;

                _removeFocusOnSearchInteraction = value;
                _removeFocusOnSearchInteraction.Requested += OnRemoveFocusRequested;
            }
        }

        private void OnRemoveFocusRequested(object sender, EventArgs e)
        {
            SearchTextField.ResignFirstResponder();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddGestureRecognizer(new UITapGestureRecognizer(() => SearchTextField.ResignFirstResponder())
            {
                CancelsTouchesInView = false
            });
            
            View.AddGestureRecognizer(new UISwipeGestureRecognizer(() => SearchTextField.ResignFirstResponder())
            {
                Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up,
                CancelsTouchesInView = false
            });
            
            SetSearchBarSeparatorVisibility();
            SetThemes();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController!.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController!.SetNavigationBarHidden(false, true);
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            SearchTextField.ShouldReturn += ShouldReturn;
            SearchTextField.EditingDidBegin += SearchTextFieldOnEditingDidBegin;
            SearchTextField.EditingDidEnd += SearchTextFieldOnEditingDidEnd;
            RemoveFocusOnSearchInteraction.Requested += OnRemoveFocusRequested;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            SearchTextField.ShouldReturn -= ShouldReturn;
            SearchTextField.EditingDidBegin -= SearchTextFieldOnEditingDidBegin;
            SearchTextField.EditingDidEnd -= SearchTextFieldOnEditingDidEnd;
            RemoveFocusOnSearchInteraction.Requested -= OnRemoveFocusRequested;
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }
        
        private void SearchTextFieldOnEditingDidBegin(object sender, EventArgs e)
        {
            HideOrShowCancelButton();
        }
        
        private void SearchTextFieldOnEditingDidEnd(object sender, EventArgs e)
        {
            HideOrShowCancelButton();
        }
        
        private bool ShouldReturn(UITextField textfield)
        {
            textfield.ResignFirstResponder();
            ViewModel.SearchCommand.Execute();
            return true;
        }
        
        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.HasAnyHistoryEntry) || e.PropertyName == nameof(ViewModel.IsHistoryVisible))
                SetSearchBarSeparatorVisibility();
        }

        private void SetSearchBarSeparatorVisibility()
        {
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
            
            set.Bind(this)
                .For(s => s.IsHistoryVisible)
                .To(s => s.IsHistoryVisible);
                        
            set.Bind(this)
                .For(s => s.RemoveFocusOnSearchInteraction)
                .To(s => s.RemoveFocusOnSearchInteraction);
            
            RecentSearchesTableView.Source = historyTable;
        }

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set
            {
                _isHistoryVisible = value;
                HideOrShowCancelButton();
            }
        }

        private void HideOrShowCancelButton()
        {
            ViewUtils.RunAnimation(ViewConstants.DefaultAnimationDuration,
                () =>
                {
                    bool isCancelVisible = SearchTextField.IsEditing || !_isHistoryVisible;
                    CancelButton.SetHiddenIfNeeded(!isCancelVisible);

                    StackViewTrailingConstraint.Constant = isCancelVisible
                        ? NumericConstants.Zero
                        : TopStackViewTrailingValue;

                    TopStackView.LayoutIfNeeded();
                });
        }

        private void SetThemes()
        {
            CancelButton.ApplyButtonStyle(AppTheme.CancelSearchButton);
            RecentSearchesLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            WelcomeTitleLabel.ApplyTextTheme(AppTheme.Heading3);
            WelcomeSubtitleLabel.ApplyTextTheme(AppTheme.Paragraph1Label1);
            SearchTextField.Font = Typography.Subtitle2.Value;
            SearchTextField.TextColor = AppColors.LabelOneColor;
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