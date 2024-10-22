using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Question.Interfaces;

public interface IQuestionPO : IBasePO
{
    public Api.Implementation.Models.Question Question { get; }
}