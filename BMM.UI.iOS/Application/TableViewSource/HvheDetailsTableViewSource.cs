using System.Diagnostics;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;
using BMM.UI.iOS.CustomViews;
using MvvmCross.Binding.Extensions;

namespace BMM.UI.iOS;

public class HvheDetailsTableViewSource : BaseTableViewSource
{
    public HvheDetailsTableViewSource(UITableView tableView) : base(tableView)
    {
    }

    protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
    {
        if (item is HvheChurchPO hvheChurchPO)
        {
            return tableView.DequeueReusableCell(hvheChurchPO.Church.IsHighlighted
                ? HighlightedChurchTableViewCell.Key
                : StandardChurchTableViewCell.Key);
        }
        
        return base.GetOrCreateCellFor(tableView, indexPath, item);
    }

    protected override IEnumerable<ITableCellType> GetTableCellTypes()
    {
        return
        [
            new TableCellType(typeof(HvheBoysVsGirlsPO), HvheBoysVsGirlsTableViewCell.Key),
            new TableCellType(typeof(HvheChurchesSelectorPO), HvheChurchesSelectorTableViewCell.Key),
            new TableCellType(typeof(HvheHeaderPO), HvheHeaderTableViewCell.Key),
            new TableCellType(typeof(HvheChurchPO), HighlightedChurchTableViewCell.Key),
            new TableCellType(typeof(HvheChurchPO), StandardChurchTableViewCell.Key)
        ];
    }
}