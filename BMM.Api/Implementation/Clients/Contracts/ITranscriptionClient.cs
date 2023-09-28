using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ITranscriptionClient
    {
        Task<IList<Transcription>> GetTranscription(int trackId, string language, int first, int last);
        Task PostSuggestEdit(int trackId, string language, IList<SuggestEditTranscription> suggestEditTranscriptions);
    }
}