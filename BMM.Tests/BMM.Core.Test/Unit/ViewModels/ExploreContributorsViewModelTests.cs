using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class ExploreContributorsViewModelTests : BaseViewModelTests
    {
        private Mock<ITrackPOFactory> _trackPOFactoryMock;

        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();
            _trackPOFactoryMock = new Mock<ITrackPOFactory>();
        }
        
        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            Client.Setup(x => x.Contributors.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult<Contributor>(null));
            var exploreContributorViewModel = new ExploreContributorsViewModel(Analytics.Object, _trackPOFactoryMock.Object);

            // Act
            await exploreContributorViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(exploreContributorViewModel.Documents);
        }
    }
}
