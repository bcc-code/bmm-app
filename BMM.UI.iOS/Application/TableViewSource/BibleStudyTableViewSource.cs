using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;

namespace BMM.UI.iOS;

public class BibleStudyTableViewSource : BaseTableViewSource
{
    public BibleStudyTableViewSource(UITableView tableView) : base(tableView)
    {
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsPlayTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsPlayTableViewCell.Key);
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsOpenTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsOpenTableViewCell.Key);
    }

    protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
    {
        if (item is IBibleStudyExternalRelationPO bibleStudyExternalRelationPO)
        {
            if (bibleStudyExternalRelationPO.WillPlayTrack)
                return tableView.DequeueReusableCell(ExternalRelationsPlayTableViewCell.Key);
            
            return tableView.DequeueReusableCell(ExternalRelationsOpenTableViewCell.Key);
        }
        
        return base.GetOrCreateCellFor(tableView, indexPath, item);
    }

    protected override IEnumerable<ITableCellType> GetTableCellTypes()
    {
        return new[]
        {
            new TableCellType(typeof(IBibleStudyHeaderPO), BibleStudyHeaderTableViewCell.Key),
            new TableCellType(typeof(IBibleStudyProgressPO), BibleStudyProgressTableViewCell.Key),
            new TableCellType(typeof(IExternalRelationListItemPO), ExternalRelationListItemTableViewCell.Key),
            new TableCellType(typeof(ISelectableListContentItemPO), TextListItemTableViewCell.Key),
            new TableCellType(typeof(ISectionHeaderPO), SectionHeaderTableViewCell.Key),
        };
    }
}