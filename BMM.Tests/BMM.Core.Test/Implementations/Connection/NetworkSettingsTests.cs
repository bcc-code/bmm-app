using System.Threading.Tasks;
using BMM.Core.Implementations.Connection;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Implementations.Connection
{
    [TestFixture]
    public class NetworkSettingsTests
    {
        private Mock<ISettingsStorage> _settingsStorage;

        [SetUp]
        public void Initialization()
        {
            _settingsStorage = new Mock<ISettingsStorage>();
        }

        [Test]
        public async Task GetMobileNetworkDownloadAllowed_ShouldReturnStorageSetting()
        {
            // Arrange
            _settingsStorage.Setup(x => x.GetMobileNetworkDownloadAllowed()).Returns(Task.FromResult(true));
            var networkSettings = new NetworkSettings(_settingsStorage.Object);

            // Act
            var result = await networkSettings.GetMobileNetworkDownloadAllowed();

            // Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public async Task GetPushNotificationsAllowed_ShouldReturnStorageSetting()
        {
            // Arrange
            _settingsStorage.Setup(x => x.GetPushNotificationsAllowed()).Returns(Task.FromResult(true));
            var networkSettings = new NetworkSettings(_settingsStorage.Object);

            // Act
            var result = await networkSettings.GetPushNotificationsAllowed();

            // Assert
            Assert.AreEqual(result, true);
        }
    }
}
