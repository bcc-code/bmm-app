using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Question.Interfaces;
using BMM.Core.ViewModels.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BMM.Core.GuardedActions.Question;

public class StandardAnswerSelectedAction
    : GuardedActionWithParameter<Answer>,
      IStandardAnswerSelectedAction
{
    private readonly IQuestionsClient _questionsClient;

    public StandardAnswerSelectedAction(IQuestionsClient questionsClient)
    {
        _questionsClient = questionsClient;
    }

    private IQuizQuestionViewModel QuizQuestionViewModel => this.GetDataContext();
    
    protected override async Task Execute(Answer answer)
    {
        var question = QuizQuestionViewModel
            .QuestionPO
            .Question;

        QuizQuestionViewModel.Tries++;

        if (QuizQuestionViewModel.InitialAnswerId.IsNullOrEmpty())
            QuizQuestionViewModel.InitialAnswerId = answer.Id;

        var correctAnswer = question
            .Answers
            .FirstOrDefault(a => a.IsCorrect);

        if (correctAnswer != null && correctAnswer.Id != answer.Id)
        {
            //TODO execute wrong answer interaction
            return;
        }
        
        //TODO execute correct answer interaction

        await _questionsClient.PostAnswer(new PostAnswer
        {
            FinalAnswerId = answer.Id,
            InitialAnswerId = QuizQuestionViewModel.InitialAnswerId,
            QuestionId = QuizQuestionViewModel.QuestionPO.Question.Id,
            Tries = QuizQuestionViewModel.Tries
        });
    }
}