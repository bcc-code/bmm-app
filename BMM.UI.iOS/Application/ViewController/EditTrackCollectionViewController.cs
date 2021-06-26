using System;
using System.ComponentModel;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Extensions;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    [Register(nameof(EditTrackCollectionViewController))]
    public class EditTrackCollectionViewController : BaseViewController<EditTrackCollectionViewModel>
    {
        public EditTrackCollectionViewController() : base(null)
        { }

        public EditTrackCollectionViewController(string nib) : base(nib)
        { }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate()
            {
                OnDidDismiss = OnDidDismiss,
                OnDidAttemptToDismiss = OnDidAttemptToDismiss
            };

            var saveButton = new UIBarButtonItem(
                ViewModel.TextSource.GetText("MenuSave"),
                UIBarButtonItemStyle.Plain,
                (sender, e) => { ViewModel.SaveAndCloseCommand.Execute(); }
            );

            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel,
                (sender, args) => ViewModel.DiscardAndCloseCommand.Execute());

            NavigationItem.SetRightBarButtonItem(saveButton, true);
            NavigationItem.SetLeftBarButtonItem(cancelButton, true);

            var tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                RowHeight = 64,
                KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag
            };

            var titleView = new TextViewWithDescription();

            View.AddSubview(tableView);
            var tableHeaderView = new UIView();
            tableView.TableHeaderView = tableHeaderView;
            tableView.TableHeaderView.AddSubview(titleView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                tableHeaderView.TrailingAnchor.ConstraintEqualTo(titleView.TrailingAnchor, 16),
                titleView.LeadingAnchor.ConstraintEqualTo(tableHeaderView.LeadingAnchor, 16),
                titleView.TopAnchor.ConstraintEqualTo(tableHeaderView.TopAnchor, 16),
                tableHeaderView.BottomAnchor.ConstraintEqualTo(titleView.BottomAnchor, 16),

                tableView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                tableView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                tableView.TopAnchor.ConstraintEqualTo(View.TopAnchor),
                tableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
            });
            tableView.SetEditing(true, false);

            var source = new EditingTableViewSource<Document, OrderingTrackTableViewCell>(tableView);
            var set = this.CreateBindingSet<EditTrackCollectionViewController, EditTrackCollectionViewModel>();
            set.Bind(source).To(vm => vm.Documents);
            set.Bind(titleView.TitleTextField).To(vm => vm.TrackCollectionTitle);
            set.Bind(titleView.TitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("RenameLabel");
            set.Bind(saveButton).For(v => v.Enabled).To(vm => vm.HasChanges).WithConversion<VisibilityConverter>();
            set.Apply();
            tableView.ResizeHeaderView();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void OnDidAttemptToDismiss(UIPresentationController presentationController)
        {
            ViewModel.DiscardAndCloseCommand.Execute();
        }

        private void OnDidDismiss(UIPresentationController presentationController)
        {
            ViewModel.DiscardAndCloseCommand.Execute();

            if (presentationController.Delegate is CustomUIAdaptivePresentationControllerDelegate customUiAdaptivePresentationControllerDelegate)
                customUiAdaptivePresentationControllerDelegate.Clear();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditTrackCollectionViewModel.HasChanges))
                ModalInPresentation = ViewModel.HasChanges;
        }
    }
}