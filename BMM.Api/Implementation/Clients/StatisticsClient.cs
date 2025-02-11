using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class StatisticsClient : BaseClient, IStatisticsClient
    {
        public StatisticsClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        public Task<IList<Document>> GetGlobalDownloadedMost(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsGlobalDownloadedMost);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetGlobalDownloadedRecently(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsGlobalDownloadedRecently);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetGlobalViewedMost(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsGlobalViewedMost);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetGlobalViewedRecently(DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsGlobalViewedRecently);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetUserDownloadedMost(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsUserDownloadedMost);
            uri.SetParameter("user", username);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetUserDownloadedRecently(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsUserDownloadedRecently);
            uri.SetParameter("user", username);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetUserViewedMost(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsUserViewedMost);
            uri.SetParameter("user", username);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public Task<IList<Document>> GetUserViewedRecently(string username, DocumentType type, int size = ApiConstants.LoadMoreSize, int from = 0)
        {
            var uri = new UriTemplate(ApiUris.StatisticsUserViewedRecently);
            uri.SetParameter("user", username);
            uri.SetParameter("type", type.ToString().ToLower());
            uri.SetParameter("size", size);
            uri.SetParameter("from", from);

            return Get<IList<Document>>(uri);
        }

        public virtual async Task PostTrackPlayedEvent(IEnumerable<TrackPlayedEvent> trackPlayedEvents)
        {
            var uri = new UriTemplate(ApiUris.StatisticsPostTrackPlayedEvent);
            var request = BuildRequest(uri, HttpMethod.Post, trackPlayedEvents);
            await RequestIsSuccessful(request);
        }

        public virtual async Task PostStreakPoints(IList<StreakPointEvent> streakPointEvents)
        {
            var uri = new UriTemplate(ApiUris.StatisticsPostStreakPoints);
            var request = BuildRequest(uri, HttpMethod.Post, streakPointEvents);
            await RequestIsSuccessful(request);
        }
        
        public virtual async Task PostListeningEvents(IList<ListeningEvent> listeningEvents)
        {
            var uri = new UriTemplate(ApiUris.StatisticsListening);
            var request = BuildRequest(uri, HttpMethod.Post, listeningEvents);
            await RequestIsSuccessful(request);
        }

        public Task<IList<YearInReviewItem>> GetYearInReview()
        {
            var uri = new UriTemplate(ApiUris.YearInReview);
            return Get<IList<YearInReviewItem>>(uri);
        }

        public async Task<AchievementsHolder> GetAchievements(AppTheme theme)
        {
            var uri = new UriTemplate(ApiUris.Achievements);
            uri.SetParameter("theme", theme);
            return await Get<AchievementsHolder>(uri);
        }
        
        public Task<IList<Achievement>> GetAchievementsToAcknowledge(AppTheme theme)
        {
            var uri = new UriTemplate(ApiUris.AchievementsToAcknowledge);
            uri.SetParameter("theme", theme);
            return Get<IList<Achievement>>(uri);
        }

        public Task<Achievement> GetAchievement(string id, AppTheme theme)
        {
            var uri = new UriTemplate(ApiUris.Achievement);
            uri.SetParameter("id", id);
            uri.SetParameter("theme", theme);
            return Get<Achievement>(uri);
        }

        public Task<ProjectProgress> GetProjectProgress(AppTheme theme)
        {
            var uri = new UriTemplate(ApiUris.ProjectProgress);
            uri.SetParameter("theme", theme);
            return Get<ProjectProgress>(uri);
        }

        public Task<ProjectRules> GetProjectRules(int projectId)
        {
            var uri = new UriTemplate(ApiUris.ProjectRules);
            uri.SetParameter("projectId", projectId);
            return Get<ProjectRules>(uri);
        }

        public Task AchievementAcknowledge(string achievementType)
        {
            var uri = new UriTemplate(ApiUris.AchievementAcknowledge);
            uri.SetParameter("name", achievementType);
            var request = BuildRequest(uri, HttpMethod.Put);
            return RequestIsSuccessful(request);
        }

        public Task DeleteAchievements()
        {
            var uri = new UriTemplate(ApiUris.StatisticsAchievement);
            var request = BuildRequest(uri, HttpMethod.Delete);
            return RequestIsSuccessful(request);
        }
    }
}