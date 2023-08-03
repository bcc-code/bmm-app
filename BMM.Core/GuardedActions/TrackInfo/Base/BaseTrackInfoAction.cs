using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.GuardedActions.TrackInfo.Base;

public abstract class BaseTrackInfoAction : GuardedActionWithParameterAndResult<Track, IEnumerable<IBasePO>>
{
    protected abstract Track Track { get; }
    
    protected IEnumerable<T> GetRelationsOfType<T>()
        where T : TrackRelation
    {
        return Track.Relations?.OfType<T>();
    }
}