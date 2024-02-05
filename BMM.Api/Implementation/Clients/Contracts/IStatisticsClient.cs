using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IStatisticsClient
    {
        /// <summary>Gets the globally most downloaded documents.</summary>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetGlobalDownloadedMost(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the globally most recently downloaded documents.</summary>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetGlobalDownloadedRecently(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the globally most viewed documents.</summary>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetGlobalViewedMost(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the globally most recently viewed documents.</summary>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetGlobalViewedRecently(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the most downloaded documents for the specified user.</summary>
        /// <param name="username">The username.</param>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetUserDownloadedMost(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the most recently downloaded documents for the specified user.</summary>
        /// <param name="username">The username.</param>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetUserDownloadedRecently(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the most viewed documents for the specified user.</summary>
        /// <param name="username">The username.</param>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetUserViewedMost(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        /// <summary>Gets the most recently viewed documents for the specified user.</summary>
        /// <param name="username">The username.</param>
        /// <param name="type">The document type.</param>
        /// <param name="size">The number of documents to get.</param>
        /// <param name="from">The number of documents to skip.</param>
        /// <returns>A list of documents.</returns>
        Task<IList<Document>> GetUserViewedRecently(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0);

        Task PostTrackPlayedEvent(IEnumerable<TrackPlayedEvent> trackPlayedEvents);
        
        Task PostStreakPoints(IList<StreakPointEvent> trackPlayedEvents);
        
        Task<IList<YearInReviewItem>> GetYearInReview();

        Task<ProjectProgress> GetProjectProgress(AppTheme theme);

        Task<ProjectRules> GetProjectRules(int projectId);
        
        Task AchievementAcknowledge(string achievementType);
        
        Task DeleteAchievements();
        Task PostListeningEvents(IList<ListeningEvent> listeningEvents);
        Task<AchievementsHolder> GetAchievements(AppTheme theme);
        Task<IList<Achievement>> GetAchievementsToAcknowledge(AppTheme theme);
    }
}