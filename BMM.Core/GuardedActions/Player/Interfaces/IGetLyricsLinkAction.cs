using BMM.Api.Abstraction;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.Player.Lyrics;

namespace BMM.Core.GuardedActions.Player.Interfaces;

public interface IGetLyricsLinkAction : IGuardedActionWithParameterAndResult<ITrackModel, LyricsLink>
{
}