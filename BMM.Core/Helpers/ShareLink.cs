using System;
using System.Text;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace BMM.Core.Helpers
{
    public class ShareLink : IShareLink
    {
        public Uri GetFor(Track track, long? startPositionInSeconds = null)
        {
            var stringBuilder = new StringBuilder("track/" + track.Id);
            
            if (startPositionInSeconds.HasValue)
                stringBuilder.Append($"?t={startPositionInSeconds}");
            
            return GenerateLink(stringBuilder.ToString());
        }

        public async Task Share(Track track,  long? startPositionInSeconds = null)
        {
            var uri = GetFor(track, startPositionInSeconds);
            await PerformRequestFor(uri.AbsoluteUri);
        }
        
        public async Task Share(Album album)
        {
            await GenerateLinkAndShare("album/" + album.Id);
        }

        public async Task Share(Contributor contributor)
        {
            await GenerateLinkAndShare($"playlist/contributor/{contributor.Id}/{contributor.Name}");
        }

        public async Task Share(Playlist playlist)
        {
            await GenerateLinkAndShare($"playlist/curated/{playlist.Id}/{playlist.Title}");
        }

        public async Task PerformRequestFor(string link)
        {
            await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(new ShareTextRequest
            {
                Uri = link
            });
        }

        private async Task GenerateLinkAndShare(string link)
        {
            var uri = GenerateLink(link);
            await PerformRequestFor(uri.AbsoluteUri);
        }

        /// <summary>
        /// See https://docs.microsoft.com/en-us/xamarin/essentials/share?context=xamarin%2Fxamarin-forms&tabs=ios for more information
        /// </summary>
        private Uri GenerateLink(string link)
        {
            return new Uri("https://" + GlobalConstants.BmmUrl + "/" + link);
            // ToDo: provide Subject for Android
        }
    }
}
