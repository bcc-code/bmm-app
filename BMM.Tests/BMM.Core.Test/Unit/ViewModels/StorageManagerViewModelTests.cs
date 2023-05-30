using System.Collections.Generic;
using System.Collections.ObjectModel;
using Acr.UserDialogs;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.ViewModels;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class StorageManagerViewModelTests : BaseViewModelTests
    {
        private Mock<IStorageManager> _storageManager;
        private Mock<ISettingsStorage> _settingsStorage;

        public override void SetUp()
        {
            base.SetUp();
            _storageManager = new Mock<IStorageManager>();
            _storageManager.Setup(x => x.Storages).Returns(new ObservableCollection<IFileStorage>(CreateFileStorages()));

            _settingsStorage = new Mock<ISettingsStorage>();
        }

        [Test]
        public void Constructor_ShouldFillUpData()
        {
            // Arrange & Act
            var storageManagerViewModel = new StorageManagementViewModel(
                _storageManager.Object,
                _settingsStorage.Object,
                new Mock<IUserDialogs>().Object,
                new Mock<IAnalytics>().Object);

            // Assert
            Assert.IsNotNull(storageManagerViewModel.Storages);
            Assert.IsNotEmpty(storageManagerViewModel.Storages);
            Assert.AreEqual(storageManagerViewModel.Storages.Count, CreateFileStorages().Count);
        }


        private List<IFileStorage> CreateFileStorages()
        {
            Mock<IFileStorage> internalStorage = new Mock<IFileStorage>();
            Mock<IFileStorage> externalStorage = new Mock<IFileStorage>();

            internalStorage.Setup(x => x.StorageKind).Returns(StorageKind.Internal);
            externalStorage.Setup(x => x.StorageKind).Returns(StorageKind.External);

            internalStorage.Setup(x => x.TotalSpace).Returns(100);
            externalStorage.Setup(x => x.TotalSpace).Returns(300);

            return new List<IFileStorage>() { internalStorage.Object, externalStorage.Object };
        }
    }
}
