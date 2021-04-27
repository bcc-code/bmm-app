using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Localization;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    class PlayerBaseViewModelTests : BaseViewModelTests
    {
        private Mock<IToastDisplayer> _toastDiplayer = new Mock<IToastDisplayer>();
        private Mock<IMvxLanguageBinder> _mvxLanguageBinder = new Mock<IMvxLanguageBinder>();
        private Mock<IMediaPlayer> _mediaController = new Mock<IMediaPlayer>();

        [SetUp]
        public void Init()
        {
            base.Setup();
            base.AdditionalSetup();

            Client.Setup(x => x.Podcast.GetById(It.IsAny<int>(), It.IsAny<CachePolicy>()))
                .Returns(Task.FromResult(new Podcast() {Cover = null, DocumentType = DocumentType.Podcast, Id = 1, Language = "en-US", Title = "Test"}));
            _mvxLanguageBinder.Setup(x => x.GetText(It.IsAny<string>())).Returns("test");

            Ioc.RegisterSingleton(_toastDiplayer.Object);
            Ioc.RegisterSingleton(_mvxLanguageBinder);
        }

        [Test]
        public async Task Initialize_ShouldAssignDownloaded()
        {
            //Arrange
            var playerBaseViewmodel = new PlayerBaseViewModel(_mediaController.Object);

            //Act
            await playerBaseViewmodel.Initialize();

            //Assert
            Assert.IsNotNull(playerBaseViewmodel.Downloaded);
        }

        [Test]
        public async Task Initialize_ShouldAssignPosition()
        {
            //Arrange
            var playerBaseViewmodel = new PlayerBaseViewModel(_mediaController.Object);

            //Act
            await playerBaseViewmodel.Initialize();

            //Assert
            Assert.IsNotNull(playerBaseViewmodel.SliderPosition);
        }

        [Test]
        public async Task Initialize_ShouldAssignIsLoading()
        {
            //Arrange
            var playerBaseViewmodel = new PlayerBaseViewModel(_mediaController.Object);

            //Act
            await playerBaseViewmodel.Initialize();

            //Assert
            Assert.IsNotNull(playerBaseViewmodel.IsLoading);
        }

        [Test]
        public async Task Initialize_ShouldAssignIsSeeking()
        {
            //Arrange
            var playerBaseViewmodel = new PlayerBaseViewModel(_mediaController.Object);

            //Act
            await playerBaseViewmodel.Initialize();

            //Assert
            Assert.IsNotNull(playerBaseViewmodel.IsSeeking);
        }

        [Test]
        public async Task Initialize_ShouldAssignPlayPauseCommand()
        {
            //Arrange
            var playerBaseViewmodel = new PlayerBaseViewModel(_mediaController.Object);

            //Act
            await playerBaseViewmodel.Initialize();

            //Assert
            Assert.IsNotNull(playerBaseViewmodel.PlayPauseCommand);
        }

        [Test]
        public async Task Initialize_ShouldAssignIsPlaying()
        {
            //Arrange
            var playerBaseViewmodel = new PlayerBaseViewModel(_mediaController.Object);

            //Act
            await playerBaseViewmodel.Initialize();

            //Assert
            Assert.IsNotNull(playerBaseViewmodel.IsPlaying);
        }
    }
}