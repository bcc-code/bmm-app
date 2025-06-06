using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IAdminTracksClient
    {
        /// <summary>Adds the specified track.</summary>
        /// <returns>The identifier for the added track.</returns>
        Task<int> Add(TrackRaw track);

        /// <summary>Adds the specified file to a track.</summary>
        Task<bool> AddFile(int id, Stream file, string filename);

        /// <summary>Deletes the specified track.</summary>
        Task<bool> Delete(TrackRaw track);

        /// <summary>Gets the cover for the specified track.</summary>
        Task<Stream> GetCover(int id);

        /// <summary>Gets the raw track with the specified identifier.</summary>
        Task<TrackRaw> GetRawById(int id);

        /// <summary>Gets the related tracks for the specified query.</summary>
        /// <param name="size">The number of tracks to get.</param>
        /// <param name="from">The number of tracks to skip.</param>
        /// <param name="contentTypes">The content types to filter by.</param>
        Task<IList<Track>> GetRelated(TrackRelation relation,
            int size = ApiConstants.LoadMoreSize,
            int from = 0,
            IEnumerable<string> contentTypes = null);

        /// <summary>Saves the specified track.</summary>
        Task<bool> Save(TrackRaw track);

        /// <summary>Sets the cover for a track.</summary>
        Task<bool> SetCover(int id, Stream file, string filename);
    }

    public interface ITracksClient
    {
        /// <summary>Gets all tracks matching the specified paramaters.</summary>
        /// <param name="cachePolicy"></param>
        /// <param name="size">The number of tracks to get.</param>
        /// <param name="from">The number of tracks to skip.</param>
        /// <param name="contentTypes">The content types to filter by.</param>
        /// <param name="tags">The tags to filter by.</param>
        /// <param name="excludeTags"></param>
        /// <returns>A list of matching tracks.</returns>
        Task<IList<Track>> GetAll(CachePolicy cachePolicy, int size = ApiConstants.LoadMoreSize, int @from = 0, IEnumerable<TrackSubType> contentTypes = null,
            IEnumerable<string> tags = null, IEnumerable<string> excludeTags = null);

        /// <summary>Gets the track with the specified identifier.</summary>
        Task<Track> GetById(int id, string desiredLanguage = default);
        
        Task<IList<Track>> GetRecommendations();
        
        Task<IList<Transcription>> GetTranscriptions(int trackId);
        Task<IList<Track>> GetRecommendationsAfterFraKaare();
    }
}