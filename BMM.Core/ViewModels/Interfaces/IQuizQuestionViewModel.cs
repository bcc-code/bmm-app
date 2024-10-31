using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Question.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces;

public interface IQuizQuestionViewModel : IBaseViewModel<IQuizQuestionViewModelParameter>
{
    IQuestionPO QuestionPO { get; set; }
    IMvxAsyncCommand<Answer> AnswerSelectedCommand { get; }
    IMvxAsyncCommand<ShortAnswer> ShortAnswerSelectedCommand { get; }
    int Tries { get; set; }
    string InitialAnswerId { get; set; }
}