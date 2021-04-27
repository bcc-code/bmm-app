using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class AlbumsViewModelTests : BaseViewModelTests
    {
        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();

            Client.Setup(x => x.Albums.GetPublishedByYear(It.IsAny<int>()))
                .Returns(Task.FromResult<IList<Album>>(null));
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            var albumViewModel = new AlbumsViewModel();

            // Act
            await albumViewModel.LoadItems();

            // Assert
            Client.Verify(x => x.Albums.GetPublishedByYear(It.IsAny<int>()));
            Assert.IsEmpty(albumViewModel.Documents);
        }
    }
}
