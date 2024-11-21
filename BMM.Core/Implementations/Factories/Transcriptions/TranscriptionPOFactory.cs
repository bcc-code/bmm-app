using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.Transcriptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Utils;

namespace BMM.Core.Implementations.Factories.Transcriptions;

public class TranscriptionPOFactory : ITranscriptionPOFactory
{
    private readonly IMediaPlayer _mediaPlayer;

    public TranscriptionPOFactory(IMediaPlayer mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
    }
    
    public IList<ReadTranscriptionsPO> Create(
        IList<Transcription> transcriptions,
        Track track,
        bool hasTimeframes)
    {
        return CreateGroupedTranscriptions(transcriptions, track, hasTimeframes);
    }
    
    private List<ReadTranscriptionsPO> CreateGroupedTranscriptions(
        IList<Transcription> transcriptions,
        Track track,
        bool hasTimeframes)
    {
        var result = new List<ReadTranscriptionsPO>();
        string currentHeader = null;
        Transcription lastHeaderTranscription = null;

        foreach (var transcription in transcriptions)
        {
            if (transcription.IsHeader)
            {
                AddHeaderOnlyElementIfNeeded();
                currentHeader = transcription.Text;
                lastHeaderTranscription = transcription;
            }
            else
            {
                result.Add(CreateReadTranscriptionsPO(track, hasTimeframes, transcription, transcription.Text, currentHeader));
                currentHeader = null;
                lastHeaderTranscription = null;
            }
        }

        AddHeaderOnlyElementIfNeeded();
        return result;

        void AddHeaderOnlyElementIfNeeded()
        {
            if (currentHeader != null && lastHeaderTranscription != null)
                result.Add(CreateReadTranscriptionsPO(track, hasTimeframes, lastHeaderTranscription, null, currentHeader));
        }
    }

    private ReadTranscriptionsPO CreateReadTranscriptionsPO(
        Track track,
        bool hasTimeframes,
        Transcription transcription,
        string text,
        string currentHeader)
    {
        return new ReadTranscriptionsPO(
            transcription,
            t => ItemClickedAction(t, track, hasTimeframes),
            text,
            currentHeader);
    }

    private async Task ItemClickedAction(
        Transcription transcription,
        Track track,
        bool hasTimeframes)
    {
        if (!hasTimeframes)
            return;

        long startTime = (long)TimeSpan.FromSeconds(transcription.Start).TotalMilliseconds;
        
        if (_mediaPlayer.CurrentTrack?.Id != track.Id)
        {
            await _mediaPlayer.Play(track.EncloseInArray(), track, startTime);
            return;
        }

        _mediaPlayer.SeekTo(startTime);
    }
}