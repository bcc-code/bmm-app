using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace BMM.UI.iOS
{
    public class EditingTableViewSource<TDocumentType, TCellType> : MvxTableViewSource
    where TCellType: MvxTableViewCell
    where TDocumentType: IDocumentPO
    {
        private readonly NSString _reuseIdentifier = new NSString(typeof(TCellType).Name);
        public EditingTableViewSource(UITableView tableView) : base(tableView)
        {
            tableView.Source = this;
            tableView.RegisterClassForCellReuse(typeof(TCellType), _reuseIdentifier);
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var data = ItemsSource as MvxObservableCollection<TDocumentType>;
            if (sourceIndexPath.Row == destinationIndexPath.Row)
                return;

            data?.Move(sourceIndexPath.Row, destinationIndexPath.Row);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            return tableView.DequeueReusableCell(_reuseIdentifier) as TCellType;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            var data = ItemsSource as MvxObservableCollection<TDocumentType>;

            data?.RemoveAt(indexPath.Row);
        }
    }
}