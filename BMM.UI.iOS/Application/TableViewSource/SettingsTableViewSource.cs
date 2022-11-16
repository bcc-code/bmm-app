using System;
using System.Collections.Generic;
using BMM.Core.Models;
using BMM.Core.Models.POs.Other;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    public class SettingsTableViewSource : VariableRowHeightTableViewSource
    {
        public SettingsTableViewSource(UITableView tableView)
            : base(tableView)
        { }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);

            if (cell is SectionHeaderTableViewCell)
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = UIColor.FromRGB(239, 242, 245)
            };

            return cell;
        }

        protected override IEnumerable<IHeightAwareTableCellType> GetHeightAwareTableCellTypes()
        {
            return new List<IHeightAwareTableCellType>
            {
                new HeightAwareTableCellType(typeof(ProfileListItem), ProfileListItemTableViewCell.Key, (nfloat)156.5),
                new HeightAwareTableCellType(typeof(CheckboxListItemPO), CheckboxListItemTableViewCell.Key, 88),
                new HeightAwareTableCellType(typeof(SelectableListItem), TextListItemDetailTableViewCell.Key, 75),
                new HeightAwareTableCellType(typeof(SectionHeader), SectionHeaderTableViewCell.Key, 56),
            };
        }
    }
}