using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.SuggestEdit.Interfaces;

public interface ITranscriptionPO : IBasePO
{
    Transcription Transcription { get; }
}