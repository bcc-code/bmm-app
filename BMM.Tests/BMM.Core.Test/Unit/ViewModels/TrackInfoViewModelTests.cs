using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class TrackInfoViewModelTests : BaseViewModelTests
    {
        private Mock<IUriOpener> _uriOpenerMock;
        private Mock<IDeepLinkHandler> _deepLinkHandlerMock;

        [SetUp]
        public void Init()
        {
            Setup();
            base.AdditionalSetup();

            TextResource.Setup(x => x.GetText(It.IsAny<string>())).Returns("Test");
        }

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();
            _uriOpenerMock = new Mock<IUriOpener>();
            _deepLinkHandlerMock = new Mock<IDeepLinkHandler>();
        }

        [Test]
        public async Task SelectSection_ShouldNotCallOpenUriWhenThereIsNoExternalRelation()
        {
            // Arrange
            var trackInfoViewModel = new TrackInfoViewModel(_uriOpenerMock.Object, ExceptionHandler.Object, _deepLinkHandlerMock.Object);
            trackInfoViewModel.TextSource = TextResource.Object;
            trackInfoViewModel.Prepare(Track);
            await trackInfoViewModel.Initialize();
            var section = trackInfoViewModel.BuildSections().FirstOrDefault();

            // Act
            ((ExternalRelationListItem)section?.Items.FirstOrDefault(x => x.GetType() == typeof(ExternalRelationListItem)))?.OnSelected.Execute();

            // Assert
            _uriOpenerMock.Verify(x => x.OpenUri(It.IsAny<Uri>()), Times.Never);
        }

        [Test]
        public async Task SelectSection_ShouldCallOpenUriWhenThereITrackContainExternalRelationInformation()
        {
            // Arrange
            var trackInfoViewModel = new TrackInfoViewModel(_uriOpenerMock.Object, ExceptionHandler.Object, _deepLinkHandlerMock.Object);
            trackInfoViewModel.TextSource = TextResource.Object;
            trackInfoViewModel.Prepare(TrackWithExternalRelations);
            await trackInfoViewModel.Initialize();
            var section = trackInfoViewModel.BuildSections().FirstOrDefault();

            // Act
            ((ExternalRelationListItem)section?.Items.FirstOrDefault(x => x.GetType() == typeof(ExternalRelationListItem)))?.OnSelected.Execute();

            // Assert
            _uriOpenerMock.Verify(x => x.OpenUri(It.IsAny<Uri>()), Times.Once);
        }

        public Track Track => new Track
        {
            Meta = new TrackMetadata(),
            Relations = new List<TrackRelation>()
        };

        public Track TrackWithExternalRelations => new Track
        {
            Meta = new TrackMetadata(),
            Relations = new []{new TrackRelationExternal
            {
                Type = TrackRelationType.External,
                Name = "Ta standpunkt",
                Url = "http://portal.brunstad.org/no/Litteratur/Les?id=83325e3a-f5ba-465a-926a-3689206e766c"
            }}
        };
    }
}
