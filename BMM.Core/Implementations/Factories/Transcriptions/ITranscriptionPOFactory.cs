using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Transcriptions;

namespace BMM.Core.Implementations.Factories.Transcriptions;

public interface ITranscriptionPOFactory
{
    IList<ReadTranscriptionsPO> Create(IList<Transcription> transcriptions, Track track, bool hasTimeframes);
}