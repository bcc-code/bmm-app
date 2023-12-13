using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts;

public interface IQuestionsClient
{
    Task<bool> PostQuestion(PostQuestion request);
}