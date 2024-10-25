using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy.Interfaces;

namespace BMM.Core.Utils;

public class ProjectBoxUtils
{
    private const int ItemsInRow = 3;

    public static IList<IBasePO[]> AdjustAchievementsRows(
        IBmmObservableCollection<IAchievementPO> achievements)
    {
        var rows = achievements.OfType<IBasePO>().Chunk(ItemsInRow).ToList();
        var toFillInLastRow = ItemsInRow - rows.Last().Length;
        var lastRow = rows.Last().ToList();
        
        for (int i = 0; i < toFillInLastRow; i++)
            lastRow.Add(new EmptyPO());

        rows[^1] = lastRow.ToArray();
        return rows;
    }
}