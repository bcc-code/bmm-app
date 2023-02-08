using System.Threading.Tasks;
using BMM.Api.Implementation.Clients.Contracts;
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
        private Mock<IContributorClient> _contributionClientMock;

        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();
            _contributionClientMock = new Mock<IContributorClient>();
        }
        
        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            Client.Setup(x => x.Contributors.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult<Contributor>(null));
            var exploreContributorViewModel = new ExploreContributorsViewModel(Analytics.Object, _contributionClientMock.Object);

            // Act
            await exploreContributorViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(exploreContributorViewModel.Documents);
        }
    }
}
