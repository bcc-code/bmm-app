using System.Collections.Generic;
using System.Threading;
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
    public class ContributorViewModelTests : BaseViewModelTests
    {
        private int id = 1;

        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();
            Client.Setup(x => x.Contributors.GetById(It.IsAny<int>())).Returns(Task.FromResult(new Contributor() { Id = id, DocumentType = DocumentType.Contributor, Name = "Test" }));
        }

        [Test]
        public async Task Refresh_CheckIfRefreshWorks()
        {
            //Arrange
            var contributorViewModel = new ContributorViewModel();

            //Act
            var refreshing = contributorViewModel.IsRefreshing;
            await contributorViewModel.LoadItems(1, 1, CachePolicy.UseCache);

            Client.Setup(x => x.Contributors.GetTracks(It.IsAny<int>(), size: It.IsAny<int>(),@from: It.IsAny<int>(),contentTypes: It.IsAny<IEnumerable<string>>()))
                .Returns(Task.Run(() => { Thread.Sleep(1000); return CreateListOfTracks(); }));

            //it should not be called using await //Why???
            contributorViewModel.Refresh();

            //Assert
            Assert.False(refreshing);
            Assert.True(contributorViewModel.IsRefreshing);
        }

        [Test]
        public async Task LoadItems_ShouldReturnATestContributors()
        {
            // Arrange
            Client.Setup(x => x.Contributors.GetTracks(It.IsAny<int>(), size: It.IsAny<int>(), @from: It.IsAny<int>(), contentTypes: null)).Returns(Task.FromResult<IList<Track>>(null));
            var contributor = new ContributorViewModel();

            // Act
            await contributor.LoadItems();

            // Assert
            Client.Verify(x => x.Contributors.GetTracks(It.IsAny<int>(), size: It.IsAny<int>(), @from: It.IsAny<int>(), contentTypes: null), Times.Once);
        }

        private IList<Track> CreateListOfTracks()
        {
            return new List<Track>();
        }
    }
}
