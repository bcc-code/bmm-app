using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyHeaderPO : BasePO, IBibleStudyHeaderPO
{
    public BibleStudyHeaderPO(string themeName, string episodeTitle, string episodeDate)
    {
        ThemeName = themeName;
        EpisodeTitle = episodeTitle;
        EpisodeDate = episodeDate;
    }
    
    public string ThemeName { get; }
    public string EpisodeTitle { get; }
    public string EpisodeDate { get; }
}