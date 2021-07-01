using System;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using Xamarin.Essentials;

namespace BMM.Core.Helpers
{
    public class ShareLink : IShareLink
    {
        public async Task For(Track track)
        {
            await GenerateLink("track/" + track.Id);
        }

        public async Task For(Album album)
        {
            await GenerateLink("album/" + album.Id);
        }

        public async Task For(Contributor contributor)
        {
            await GenerateLink($"playlist/contributor/{contributor.Id}/{contributor.Name}");
        }

        public async Task For(string link)
        {
            await GenerateLink(link);
        }

        /// <summary>
        /// See https://docs.microsoft.com/en-us/xamarin/essentials/share?context=xamarin%2Fxamarin-forms&tabs=ios for more information
        /// </summary>
        public async Task GenerateLink(string link)
        {
            var url = new Uri("https://" + GlobalConstants.BmmUrl + "/" + link);
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = url.AbsoluteUri
            });
            // ToDo: provide Subject for Android
        }
    }
}
