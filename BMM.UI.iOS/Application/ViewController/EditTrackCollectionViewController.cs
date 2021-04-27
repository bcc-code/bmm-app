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

            NavigationController.PresentationController.Delegate = new EditTrackCollectionPresentationControllerDelegate(ViewModel);

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

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditTrackCollectionViewModel.HasChanges))
                ModalInPresentation = ViewModel.HasChanges;
        }

        private class EditTrackCollectionPresentationControllerDelegate : UIAdaptivePresentationControllerDelegate
        {
            private readonly EditTrackCollectionViewModel _viewModel;

            public EditTrackCollectionPresentationControllerDelegate(EditTrackCollectionViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            /// <summary>
            /// This method is called when the property ModalInPresentation of the ViewController is set to true.
            /// The view controller shakes and indicates that open changes in the modal are left when tried to dismiss.
            /// </summary>
            public override void DidAttemptToDismiss(UIPresentationController presentationController)
            {
                _viewModel.DiscardAndCloseCommand.Execute();
            }

            public override void DidDismiss(UIPresentationController presentationController)
            {
                _viewModel.DiscardAndCloseCommand.Execute();
            }
        }

    }
}