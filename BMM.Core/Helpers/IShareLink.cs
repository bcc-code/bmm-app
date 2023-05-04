using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Helpers
{
    public interface IShareLink
    {
        Uri GetFor(Track track, long? startPositionInMs = null);
        Task Share(Track track);
        Task Share(Album album);
        Task Share(Contributor contributor);
        Task PerformRequestFor(string link);
    }
}