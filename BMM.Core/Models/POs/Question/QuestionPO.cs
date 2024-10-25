using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Question.Interfaces;

namespace BMM.Core.Models.POs.Question;

public class QuestionPO : BasePO, IQuestionPO
{
    public QuestionPO(Api.Implementation.Models.Question question)
    {
        Question = question;
    }
    
    public Api.Implementation.Models.Question Question { get; }
}