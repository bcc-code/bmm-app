using System.Threading.Tasks;
using BMM.Api.Abstraction;
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

        public override void SetUp()
        {
            base.SetUp();
            _contributionClientMock = new Mock<IContributorClient>();
        }
        
        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            _contributionClientMock.Setup(x => x.GetFeaturedContributors(It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult<IList<Contributor>>(new List<Contributor>()));
            var exploreContributorViewModel = new ExploreContributorsViewModel(Analytics.Object, _contributionClientMock.Object);

            // Act
            await exploreContributorViewModel.LoadItems();

            // Assert
            Assert.IsEmpty(exploreContributorViewModel.Documents);
        }
    }
}
