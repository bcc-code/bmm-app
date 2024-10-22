using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Question.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Interfaces;

public interface IQuizQuestionViewModel : IBaseViewModel<IQuizQuestionViewModelParameter>
{
    IQuestionPO QuestionPO { get; set; }
}