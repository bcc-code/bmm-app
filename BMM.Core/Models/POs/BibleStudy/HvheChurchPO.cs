using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.BibleStudy;

public class HvheChurchPO : BasePO
{
    public HvheChurchPO(
        Church church,
        string boysTitle,
        string girlsTitle)
    {
        Church = church;
        BoysTitle = boysTitle;
        GirlsTitle = girlsTitle;
        FirstGameNight = church.GameNights.SafeGetAt(0, GameNights.None);
        SecondGameNight = church.GameNights.SafeGetAt(1, GameNights.None);
        ThirdGameNight = church.GameNights.SafeGetAt(2, GameNights.None);
    }
    
    public Church Church { get; }
    public string BoysTitle { get; }
    public string GirlsTitle { get; }
    public GameNights FirstGameNight { get; }
    public GameNights SecondGameNight { get; }
    public GameNights ThirdGameNight { get; }
}