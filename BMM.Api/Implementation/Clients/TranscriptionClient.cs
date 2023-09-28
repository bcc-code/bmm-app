using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients;

public class TranscriptionClient : BaseClient, ITranscriptionClient
{
    public TranscriptionClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger) : base(handler, baseUri, logger)
    {
    }
    
    public Task<IList<Transcription>> GetTranscription(int trackId, string language, int first, int last)
    {
        var uri = new UriTemplate(ApiUris.TranscriptionGet);
        uri.SetParameter("trackId", trackId);
        uri.SetParameter("language", language);
        uri.SetParameter("first", first);
        uri.SetParameter("last", last);
        return Get<IList<Transcription>>(uri);
    }

    public async Task PostSuggestEdit(int trackId, string language, IList<SuggestEditTranscription> suggestEditTranscriptions)
    {
        var uri = new UriTemplate(ApiUris.TranscriptionPost);
        uri.SetParameter("trackId", trackId);
        uri.SetParameter("language", language);
        await RequestIsSuccessful(BuildRequest(uri, HttpMethod.Post, suggestEditTranscriptions));
    }
}