using System;
using System.Drawing;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.NewMediaPlayer;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = Translations.MenuViewModel_Search, TabIconName = "icon_search", TabSelectedIconName = "icon_search_active", WrapInNavigationController = false)]
    public partial class SearchViewController : BaseViewController<SearchViewModel>
    {
        private readonly System.nint SearchTermTextFieldTag;
        private UIKit.UITextField SearchTermTextField;

        public SearchViewController()
            : base(nameof(SearchViewController))
        {
            SearchTermTextFieldTag = new System.nint(100);
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AddTextFieldToNavigationBar();

            // Run search and hide keyboard after hitting "Search"
            SearchTermTextField.ShouldReturn += ((textField) =>
            {
                textField.ResignFirstResponder();
                ViewModel.SearchCommand.Execute();
                return true;
            });

            var resultsTable = new DocumentsTableViewSource(SearchResultsTable);

            var suggestionsTable = new MvxSimpleTableViewSource(SearchSuggestionTable, SearchSuggestionTableViewCell.Key);
            SearchSuggestionTable.Source = suggestionsTable;

            var historyTable = new MvxSimpleTableViewSource(SearchHistoryTable, SearchSuggestionTableViewCell.Key);
            SearchHistoryTable.Source = historyTable;
            SearchHistoryTable.AccessibilityIdentifier = "history";

            var set = this.CreateBindingSet<SearchViewController, SearchViewModel>();
            set.Bind(SearchTermTextField).To(vm => vm.SearchTerm);
            set.Bind(SearchTermTextField).For(s => s.Placeholder).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_SearchHint);

            set.Bind(resultsTable).To(vm => vm.Documents);
            set.Bind(ResultsHeaderLabel).For(s => s.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_SearchResults);
            set.Bind(resultsTable).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand);
            set.Bind(resultsTable).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(resultsTable).For(s => s.IsFullyLoaded).To(s => s.IsFullyLoaded);

            set.Bind(suggestionsTable).To(vm => vm.SearchSuggestions);
            set.Bind(suggestionsTable).For(s => s.SelectionChangedCommand).To(s => s.SearchByTermCommand);
            set.Bind(SuggestionsHeaderLabel).For(s => s.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_SearchSuggestions);
            set.Bind(SearchSuggestionTable).For(s => s.Hidden).To(vm => vm.ShowSuggestions).WithConversion<InvertedVisibilityConverter>();

            set.Bind(historyTable).To(vm => vm.SearchHistory);
            set.Bind(historyTable).For(s => s.SelectionChangedCommand).To(s => s.SearchByTermCommand);
            set.Bind(HistoryHeaderLabel).For(s => s.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_SearchHistory);
            set.Bind(SearchHistoryTable).For(s => s.Hidden).To(vm => vm.ShowHistory).WithConversion<InvertedVisibilityConverter>();
            set.Bind(ClearHistoryButton).To(vm => vm.DeleteHistoryCommand);

            set.Bind(NoResultsLabel).For(s => s.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_SearchNoResults);
            set.Bind(EmptyResultsView).For(s => s.Hidden).To(vm => vm.NoResults).WithConversion<InvertedVisibilityConverter>();

            set.Bind(WelcomeTitleLabel).For(s => s.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_WelcomeTitle);
            set.Bind(WelcomeSubTitleLabel).For(s => s.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SearchViewModel_WelcomeSubTitle);
            set.Bind(SearchExecutedView).For(s => s.Hidden).To(vm => vm.SearchExecuted).WithConversion<InvertedVisibilityConverter>();

            set.Apply();

            View.AddGestureRecognizer(new UITapGestureRecognizer(() => SearchTermTextField.ResignFirstResponder())
            {
                CancelsTouchesInView = false
            });
            View.AddGestureRecognizer(new UISwipeGestureRecognizer(() => SearchTermTextField.ResignFirstResponder())
            {
                Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up,
                CancelsTouchesInView = false
            });

            SetThemes();
        }

        private void SetThemes()
        {
            ResultsHeaderLabel.ApplyTextTheme(AppTheme.Title1);
            HistoryHeaderLabel.ApplyTextTheme(AppTheme.Title1);
            SuggestionsHeaderLabel.ApplyTextTheme(AppTheme.Title1);
            WelcomeTitleLabel.ApplyTextTheme(AppTheme.Title2);
            NoResultsLabel.ApplyTextTheme(AppTheme.Title2);
            WelcomeSubTitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
            SearchTermTextField.TextColor = AppColors.LabelPrimaryColor;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SearchTermTextField.Hidden = false;
            SearchTermTextField.Frame = GetRectangleForSearchTermTextField();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (SearchTermTextField != null)
            {
                SearchTermTextField.Hidden = true;
            }
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            if (SearchTermTextField != null)
            {
                SearchTermTextField.Frame = GetRectangleForSearchTermTextField();
            }
        }

        private void AddTextFieldToNavigationBar()
        {
            //Clear Title
            NavigationItem.Title = "";

            //Put TextField in NavigationBar
            SearchTermTextField = (UITextField)NavigationController.View.ViewWithTag(SearchTermTextFieldTag);
            if (SearchTermTextField == null)
            {
                SearchTermTextField = new UITextField(GetRectangleForSearchTermTextField());
                SearchTermTextField.Tag = SearchTermTextFieldTag;
                SearchTermTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

                NavigationController.View.AddSubview(SearchTermTextField);
                SearchTermTextField.AccessibilityIdentifier = "search_text_field";
            }

            //Styling input field
            SearchTermTextField.BorderStyle = UITextBorderStyle.None;
            SearchTermTextField.Layer.CornerRadius = 3f;
            SearchTermTextField.AttributedPlaceholder = new NSAttributedString (
                string.Empty,
                Typography.Subtitle2.Value,
                AppColors.LabelTertiaryColor);

            //Add Search Icon
            var searchImageView = new UIImageView(UIImage.FromBundle("InputSearchIcon"));
            searchImageView.TintColor = AppColors.LabelTertiaryColor;
            searchImageView.Frame = new RectangleF(0, 0, 30, 14);
            searchImageView.ContentMode = UIViewContentMode.Center;
            SearchTermTextField.LeftView = searchImageView;
            SearchTermTextField.LeftViewMode = UITextFieldViewMode.Always;

            CustomClearButton();
        }

        private RectangleF GetRectangleForSearchTermTextField()
        {
            // Get the position of the Search field related to the safeGuide
            var yPosition = GetPositionForSearchField();

            return new RectangleF(16, yPosition, (float)NavigationController.View.Frame.Width - 20, 30);
        }

        private float GetPositionForSearchField()
        {
            UIView view = NavigationController.View;
            UILayoutGuide safeGuide = view.SafeAreaLayoutGuide;
            var topSafeAreaHeight = safeGuide.LayoutFrame.Y - view.Frame.Y;

            return (float)topSafeAreaHeight + 7;
        }

        private void CustomClearButton()
        {
            //Custom Clear button
            UIButton ClearButton = UIButton.FromType(UIButtonType.Custom);
            ClearButton.AccessibilityIdentifier = "clear_button";
            ClearButton.SetImage(UIImage.FromFile("icon_close_static.png"), UIControlState.Normal);
            ClearButton.Frame = new RectangleF(0, 0, 30, 30);
            ClearButton.ContentMode = UIViewContentMode.Center;
            SearchTermTextField.RightView = ClearButton;
            SearchTermTextField.RightViewMode = UITextFieldViewMode.Always;

            // Hide clear button if TextField is empty or a search is executed
            Action updateValue = () => ClearButton.Hidden = (!ViewModel.SearchExecuted && String.IsNullOrEmpty(ViewModel.SearchTerm));
            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "SearchTerm" || e.PropertyName == "SearchExecuted")
                {
                    updateValue();
                }
            };
            updateValue();

            // Clear textfield on TouchUpInside
            ClearButton.TouchUpInside += (sender, e) => ViewModel.ClearSearch();
        }
    }
}