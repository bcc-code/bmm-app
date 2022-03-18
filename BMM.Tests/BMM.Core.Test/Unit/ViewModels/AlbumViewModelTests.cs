using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.ContinueListening.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class AlbumViewModelTests : BaseViewModelTests
    {
        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            Client.Setup(x => x.Albums.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult<Album>(null));
            var album = new AlbumViewModel(new Mock<IShareLink>().Object, new Mock<IResumeOrShufflePlayAction>().Object);

            // Act
            await album.LoadItems();

            // Assert
            Client.Verify(x => x.Albums.GetById(It.IsAny<int>()), Times.Once);
            Assert.IsEmpty(album.Documents);
        }
    }
}
