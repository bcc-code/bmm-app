using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public interface ITopSongsPOFactory
    {
        ITrackPO Create(TopSong topSong, IMvxAsyncCommand<Document> optionsClickedCommand);
    }
}