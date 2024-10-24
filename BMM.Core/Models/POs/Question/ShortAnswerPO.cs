using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Question.Interfaces;

namespace BMM.Core.Models.POs.Question;

public class ShortAnswerPO : BasePO, IShortAnswerPO
{
    public ShortAnswerPO(ShortAnswer shortAnswer)
    {
        ShortAnswer = shortAnswer;
    }
    
    public ShortAnswer ShortAnswer { get; }
}