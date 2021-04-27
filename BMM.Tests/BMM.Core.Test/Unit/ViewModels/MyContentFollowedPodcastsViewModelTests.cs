using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels.MyContent;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class MyContentFollowedPodcastsViewModelTests : BaseViewModelTests
    {
        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();

            Client.Setup(x => x.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated))
                .Returns(Task.FromResult<IList<Podcast>>(null));
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            var podcastOfflineManager = new Mock<IPodcastOfflineManager>();
            var filter = new Mock<IDownloadedTracksOnlyFilter>();
            var podcastBaseViewModel = new DownloadedFollowedPodcastsViewModel(podcastOfflineManager.Object, MvxMessenger.Object, filter.Object);

            // Act
            await podcastBaseViewModel.LoadItems();

            // Assert
            Client.Verify(x => x.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated), Times.Once);
            Assert.IsEmpty(podcastBaseViewModel.Documents);
        }
    }
}
