using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.Other.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BMM.UI.iOS;

public class BibleStudyTableViewSource : BaseTableViewSource
{
    public BibleStudyTableViewSource(UITableView tableView) : base(tableView)
    {
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsPlayTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsPlayTableViewCell.Key);
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsPlayWithSubtitleTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsPlayWithSubtitleTableViewCell.Key);
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsOpenTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsOpenTableViewCell.Key);
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsOpenWithSubtitleTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsOpenWithSubtitleTableViewCell.Key);
        tableView.RegisterNibForCellReuse(UINib.FromName(ExternalRelationsQuizTableViewCell.Key, NSBundle.MainBundle), ExternalRelationsQuizTableViewCell.Key);
    }

    protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
    {
        if (item is not IBibleStudyExternalRelationPO bibleStudyExternalRelationPO)
            return base.GetOrCreateCellFor(tableView, indexPath, item);
        
        if (bibleStudyExternalRelationPO.WillPlayTrack)
        {
            return bibleStudyExternalRelationPO.Subtitle.IsNullOrEmpty()
                ? tableView.DequeueReusableCell(ExternalRelationsPlayTableViewCell.Key)
                : tableView.DequeueReusableCell(ExternalRelationsPlayWithSubtitleTableViewCell.Key);
        }

        if (bibleStudyExternalRelationPO.HasQuestion)
            return tableView.DequeueReusableCell(ExternalRelationsQuizTableViewCell.Key);

        return bibleStudyExternalRelationPO.Subtitle.IsNullOrEmpty()
            ? tableView.DequeueReusableCell(ExternalRelationsOpenTableViewCell.Key)
            : tableView.DequeueReusableCell(ExternalRelationsOpenWithSubtitleTableViewCell.Key);

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