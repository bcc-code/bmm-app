using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients;

public class QuestionsClient : BaseClient, IQuestionsClient
{
    public QuestionsClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
    {
    }
    
    public Task<bool>  PostQuestion(PostQuestion postQuestion)
    {
        var uri = new UriTemplate(ApiUris.PostQuestion);
        var request = BuildRequest(uri, HttpMethod.Post, postQuestion);
        return RequestIsSuccessful(request);
    }
    
    public Task<Question> GetQuestion(int id)
    {
        var uri = new UriTemplate(ApiUris.QuizQuestion);
        uri.SetParameter("id", id);
        return Get<Question>(uri);
    }
    
    public Task<bool> PostAnswer(PostAnswer answer)
    {
        var uri = new UriTemplate(ApiUris.QuizQuestion);
        uri.SetParameter("id", answer.QuestionId);
        var request = BuildRequest(uri, HttpMethod.Post, answer);
        return RequestIsSuccessful(request);
    }
}