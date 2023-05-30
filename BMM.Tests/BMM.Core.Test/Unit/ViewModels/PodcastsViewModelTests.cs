using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class PodcastsViewModelTests : BaseViewModelTests
    {
        public override void SetUp()
        {
            base.SetUp();
            Client.Setup(x => x.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated))
                .Returns(Task.FromResult<IList<Podcast>>(null));
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            var podcastsViewModel = new PodcastsViewModel();

            // Act
            await podcastsViewModel.LoadItems();

            // Assert
            Client.Verify(x => x.Podcast.GetAll(CachePolicy.UseCacheAndRefreshOutdated), Times.Once);
            Assert.IsEmpty(podcastsViewModel.Documents);
        }
    }
}
