using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Question.Interfaces;

public interface IShortAnswerPO : IBasePO
{
    ShortAnswer ShortAnswer { get; }
}