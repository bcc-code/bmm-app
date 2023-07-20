using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyHeaderPO : IBasePO
{
    string ThemeName { get; }
    string EpisodeTitle { get; }
    string EpisodeDate { get; }
}