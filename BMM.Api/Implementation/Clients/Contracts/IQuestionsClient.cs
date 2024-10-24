using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts;

public interface IQuestionsClient
{
    Task<Question> GetQuestion(int id);
    Task<bool> PostQuestion(PostQuestion request);
}