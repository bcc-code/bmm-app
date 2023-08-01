using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters;

public class BibleStudyParameters : IBibleStudyParameters
{
    public BibleStudyParameters(Track track)
    {
        Track = track;
    }
    
    public Track Track { get; }
}