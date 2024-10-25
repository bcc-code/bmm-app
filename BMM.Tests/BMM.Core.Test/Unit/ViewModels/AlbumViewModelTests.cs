using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class AlbumViewModelTests : BaseViewModelTests
    {
        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            Client.Setup(x => x.Albums.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult<Album>(null));
            var album = new AlbumViewModel(
                new Mock<IShareLink>().Object,
                new Mock<IPlayOrResumePlayAction>().Object,
                new Mock<IDocumentsPOFactory>().Object,
                new Mock<IStorageManager>().Object,
                new Mock<IDocumentFilter>().Object,
                new Mock<IDownloadQueue>().Object,
                new Mock<IConnection>().Object,
                new Mock<INetworkSettings>().Object,
                new Mock<IAlbumManager>().Object,
                new Mock<IOfflineAlbumStorage>().Object);

            // Act
            await album.LoadItems();

            // Assert
            Client.Verify(x => x.Albums.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsEmpty(album.Documents);
        }
    }
}
