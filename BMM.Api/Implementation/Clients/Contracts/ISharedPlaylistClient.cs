using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ISharedPlaylistClient
    {
        /// <summary>Gets the track collection with specified sharing secret.</summary>
        Task<TrackCollection> Get(string sharingSecret);

        /// <summary>Follows the track collection with specified sharing secret.</summary>
        Task<bool> Follow(string sharingSecret);
    }
}