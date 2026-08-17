using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Media;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Media
{
    [TestFixture]
    public class MediaFileUrlSetterTests
    {
        private const string CheckedPath = "/documents/track_1_media_nb.mp3";
        private const string ResolvedPath = "/documents/track_1_media_en.mp3";
        private const string MismatchEvent = "Downloaded track resolved to a different file than the one that was checked";

        private Mock<IStorageManager> _storageManager;
        private Mock<IFileStorage> _storage;
        private Mock<ITrackMediaHelper> _trackMediaHelper;
        private Mock<IAnalytics> _analytics;
        private Track _track;
        private TrackMediaFile _playableFile;

        [SetUp]
        public void Setup()
        {
            _storage = new Mock<IFileStorage>();
            _storageManager = new Mock<IStorageManager>();
            _storageManager.Setup(x => x.SelectedStorage).Returns(_storage.Object);
            _trackMediaHelper = new Mock<ITrackMediaHelper>();
            _analytics = new Mock<IAnalytics>();

            _playableFile = new TrackMediaFile { Url = "https://example.org/track_1_media_en.mp3", MimeType = "audio/mpeg" };
            _track = new Track
            {
                Id = 1,
                Language = "nb",
                Media = new List<TrackMedia>
                {
                    new TrackMedia { Type = TrackMediaType.Audio, Files = new List<TrackMediaFile> { _playableFile } }
                }
            };

            _trackMediaHelper
                .Setup(x => x.GetFileByMediaType(It.IsAny<IEnumerable<TrackMedia>>(), It.IsAny<TrackMediaType>()))
                .Returns(_playableFile);
        }

        private MediaFileUrlSetter CreateSetter()
        {
            return new MediaFileUrlSetter(_storageManager.Object, _trackMediaHelper.Object, _analytics.Object);
        }

        [Test]
        public void SetsLocalPath_WhenTheTrackIsDownloaded()
        {
            _storage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(true);
            _storage.Setup(x => x.GetUrlByFile(_playableFile)).Returns(ResolvedPath);
            _storage.Setup(x => x.GetUrlByFile(It.IsAny<IDownloadable>())).Returns(ResolvedPath);

            CreateSetter().SetLocalPathIfDownloaded(_track);

            Assert.AreEqual(ResolvedPath, _track.LocalPath);
        }

        [Test]
        public void ClearsLocalPath_WhenTheTrackIsNotDownloaded()
        {
            _track.LocalPath = ResolvedPath;
            _storage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(false);

            CreateSetter().SetLocalPathIfDownloaded(_track);

            Assert.IsNull(_track.LocalPath);
        }

        [Test]
        public void ClearsLocalPath_WhenThereIsNoPlayableMediaFile()
        {
            _track.LocalPath = ResolvedPath;
            _trackMediaHelper
                .Setup(x => x.GetFileByMediaType(It.IsAny<IEnumerable<TrackMedia>>(), It.IsAny<TrackMediaType>()))
                .Returns((TrackMediaFile)null);

            CreateSetter().SetLocalPathIfDownloaded(_track);

            Assert.IsNull(_track.LocalPath);
        }

        [Test]
        public void LogsNothing_WhenTheCheckedFileIsTheResolvedFile()
        {
            _storage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(true);
            _storage.Setup(x => x.GetUrlByFile(_playableFile)).Returns(ResolvedPath);
            _storage.Setup(x => x.GetUrlByFile(It.IsAny<IDownloadable>())).Returns(ResolvedPath);

            CreateSetter().SetLocalPathIfDownloaded(_track);

            _analytics.Verify(x => x.LogEvent(MismatchEvent, It.IsAny<IDictionary<string, object>>()), Times.Never);
        }

        [Test]
        public void Logs_WhenIsDownloadedCheckedADifferentFileThanTheOneHandedToThePlayer()
        {
            // IsDownloaded validates the path built from Track.Url while LocalPath is built from the
            // first playable media file. When they differ the player gets an unverified path.
            _storage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(true);
            _storage.Setup(x => x.GetUrlByFile(_playableFile)).Returns(ResolvedPath);
            _storage.Setup(x => x.GetUrlByFile(It.IsAny<IDownloadable>())).Returns(CheckedPath);

            IDictionary<string, object> logged = null;
            _analytics
                .Setup(x => x.LogEvent(MismatchEvent, It.IsAny<IDictionary<string, object>>()))
                .Callback<string, IDictionary<string, object>>((_, parameters) => logged = parameters);

            CreateSetter().SetLocalPathIfDownloaded(_track);

            Assert.NotNull(logged, "the mismatch should have been reported");
            Assert.AreEqual(1, logged["trackId"]);
            Assert.AreEqual("track_1_media_nb.mp3", logged["checkedFile"]);
            Assert.AreEqual("track_1_media_en.mp3", logged["resolvedFile"]);
            Assert.AreEqual(false, logged["resolvedFileExists"]);
        }

        [Test]
        public void LogsEachTrackOnlyOnce_SoAQueueOfMismatchesDoesNotFloodAnalytics()
        {
            _storage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(true);
            _storage.Setup(x => x.GetUrlByFile(_playableFile)).Returns(ResolvedPath);
            _storage.Setup(x => x.GetUrlByFile(It.IsAny<IDownloadable>())).Returns(CheckedPath);

            var setter = CreateSetter();
            setter.SetLocalPathIfDownloaded(_track);
            setter.SetLocalPathIfDownloaded(_track);
            setter.SetLocalPathIfDownloaded(_track);

            _analytics.Verify(x => x.LogEvent(MismatchEvent, It.IsAny<IDictionary<string, object>>()), Times.Once);
        }

        [Test]
        public void LogsNothing_WhenTheTrackIsNotDownloaded()
        {
            _storage.Setup(x => x.IsDownloaded(It.IsAny<IDownloadable>())).Returns(false);

            CreateSetter().SetLocalPathIfDownloaded(_track);

            _analytics.Verify(x => x.LogEvent(MismatchEvent, It.IsAny<IDictionary<string, object>>()), Times.Never);
        }
    }
}
