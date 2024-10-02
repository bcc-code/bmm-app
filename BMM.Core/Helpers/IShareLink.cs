using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Helpers
{
    public interface IShareLink
    {
        Uri GetFor(Track track, long? startPositionInSeconds = null);
        Task Share(Track track);
        Task Share(Album album);
        Task Share(Contributor contributor);
        Task Share(Playlist playlist);
        Task PerformRequestFor(string link);
    }
}