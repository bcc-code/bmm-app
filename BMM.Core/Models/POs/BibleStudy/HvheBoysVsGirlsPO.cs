using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy;

public class HvheBoysVsGirlsPO : BasePO, IBasePO
{
    public HvheBoysVsGirlsPO(
        string boysTitle,
        int boysPoints,
        string girlsTitle,
        int girlsPoints)
    {
        BoysTitle = boysTitle;
        BoysPoints = boysPoints;
        GirlsTitle = girlsTitle;
        GirlsPoints = girlsPoints;
    }

    public string BoysTitle { get; }
    public int BoysPoints { get; }
    public string GirlsTitle { get; }
    public int GirlsPoints { get; }
}