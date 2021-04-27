using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class ExplorePopularViewModelTests : BaseViewModelTests
    {
        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();

            Client.Setup(x => x.Statistics.GetGlobalDownloadedMost(It.IsAny<DocumentType>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IList<Document>>(GetTestDocuments()));

            var mockMvxMessenger = new Mock<IMvxMessenger>();
            Ioc.RegisterSingleton(mockMvxMessenger.Object);
            Ioc.RegisterSingleton(new Mock<IMvxTextProvider>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaQueue>().Object);
            Ioc.RegisterSingleton(new Mock<INotificationCenter>(MockBehavior.Strict).Object);
        }

        [Test]
        public void Initialization()
        {
            // Arrange & Act
            var explorePopularView = new ExplorePopularViewModel();

            // Assert
            Assert.AreNotEqual(explorePopularView, null);
        }

        [Test]
        public async Task LoadItems_ShouldReturnATestDocuments()
        {
            // Arrange
            var explorePopularView = new ExplorePopularViewModel();

            // Act
            await explorePopularView.LoadItems(1, 1, CachePolicy.UseCache);

            // Assert
            Client.Verify(x => x.Statistics.GetGlobalDownloadedMost(It.IsAny<DocumentType>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task LoadItems_ShouldHandleNullValueAndDisplayEmptyDocument()
        {
            // Arrange
            Client.Setup(x => x.Statistics.GetGlobalDownloadedMost(It.IsAny<DocumentType>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IList<Document>>(null));
            var contributors = new ExplorePopularViewModel();

            // Act
            await contributors.LoadItems(1, 1, CachePolicy.UseCache);

            // Assert
            Client.Verify(x => x.Statistics.GetGlobalDownloadedMost(It.IsAny<DocumentType>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.IsEmpty(contributors.Documents);
        }

        private IList<Document> GetTestDocuments()
        {
            return new List<Document>();
        }
    }
}
