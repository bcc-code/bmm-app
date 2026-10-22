using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Test.Unit.Implementations.Downloading;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Extensions
{
    [TestFixture]
    public class TrackExtensionsTests
    {
        private readonly FakeTrackFactory _fakeTrackFactory = new FakeTrackFactory();

        private static Track TrackWithMedia(int id, params TrackMedia[] media)
        {
            return new Track
            {
                Id = id,
                Language = "nb",
                Media = media
            };
        }

        private static TrackMedia AudioMedia(params TrackMediaFile[] files)
        {
            return new TrackMedia { Type = TrackMediaType.Audio, Files = files };
        }

        private static TrackMediaFile File(long size, string url = "https://example.org/track.mp3")
        {
            return new TrackMediaFile { Size = size, Url = url, MimeType = "audio/mpeg" };
        }

        #region SumApproximateDownloadSize

        [Test]
        public void SumApproximateDownloadSize_Adds_Up_Every_File()
        {
            var tracks = new[]
            {
                TrackWithMedia(1, AudioMedia(File(100), File(200))),
                TrackWithMedia(2, AudioMedia(File(300)))
            };

            Assert.AreEqual(600, tracks.SumApproximateDownloadSize());
        }

        [Test]
        public void SumApproximateDownloadSize_Of_Nothing_Is_Zero()
        {
            Assert.AreEqual(0, Enumerable.Empty<Track>().SumApproximateDownloadSize());
        }

        [Test]
        public void SumApproximateDownloadSize_Handles_A_Null_Collection()
        {
            IEnumerable<Track> tracks = null;

            Assert.AreEqual(0, tracks.SumApproximateDownloadSize());
        }

        /// <summary>
        /// "media" is optional in the API. A single track without it used to throw a
        /// NullReferenceException out of the download button's size check, which the user saw as
        /// "An unknown error occurred" with nothing downloaded.
        /// </summary>
        [Test]
        public void SumApproximateDownloadSize_Skips_Tracks_Without_Media()
        {
            var tracks = new[]
            {
                TrackWithMedia(1, AudioMedia(File(100))),
                new Track { Id = 2, Language = "nb", Media = null }
            };

            Assert.AreEqual(100, tracks.SumApproximateDownloadSize());
        }

        [Test]
        public void SumApproximateDownloadSize_Skips_Media_Without_Files()
        {
            var tracks = new[]
            {
                TrackWithMedia(1, AudioMedia(File(100))),
                TrackWithMedia(2, new TrackMedia { Type = TrackMediaType.Audio, Files = null })
            };

            Assert.AreEqual(100, tracks.SumApproximateDownloadSize());
        }

        [Test]
        public void SumApproximateDownloadSize_Skips_Null_Tracks()
        {
            var tracks = new[]
            {
                TrackWithMedia(1, AudioMedia(File(100))),
                null
            };

            Assert.AreEqual(100, tracks.SumApproximateDownloadSize());
        }

        [Test]
        public void SumApproximateDownloadSize_Counts_A_Track_That_Has_No_Downloadable_Url()
        {
            // The size check is about free space, not about what we will end up fetching, so a file
            // without a url still occupies the estimate rather than throwing.
            var tracks = new[] { TrackWithMedia(1, AudioMedia(File(100, url: null))) };

            Assert.AreEqual(100, tracks.SumApproximateDownloadSize());
        }

        #endregion

        #region WhereDownloadable

        [Test]
        public void WhereDownloadable_Keeps_Ordinary_Audio_Tracks()
        {
            var tracks = new[]
            {
                _fakeTrackFactory.CreateTrackWithId(1),
                _fakeTrackFactory.CreateTrackWithId(2)
            };

            CollectionAssert.AreEquivalent(
                new[] { 1, 2 },
                tracks.WhereDownloadable().Select(track => track.Id));
        }

        /// <summary>
        /// Mirrors the filter in TrackCollectionOfflineTrackProvider. A video left in the list would make
        /// "is everything downloaded" wait forever for a file that is never requested.
        /// </summary>
        [Test]
        public void WhereDownloadable_Excludes_Videos()
        {
            var video = _fakeTrackFactory.CreateTrackWithId(2);
            video.Subtype = TrackSubType.Video;

            var tracks = new[] { _fakeTrackFactory.CreateTrackWithId(1), video };

            CollectionAssert.AreEquivalent(
                new[] { 1 },
                tracks.WhereDownloadable().Select(track => track.Id));
        }

        /// <summary>
        /// Mirrors the filter in GlobalTrackProvider: a track with no url is never enqueued, so it must
        /// not be expected on disk either.
        /// </summary>
        [Test]
        public void WhereDownloadable_Excludes_Tracks_Without_A_Url()
        {
            var tracks = new[]
            {
                _fakeTrackFactory.CreateTrackWithId(1),
                TrackWithMedia(2, AudioMedia(File(100, url: null))),
                new Track { Id = 3, Language = "nb", Media = null }
            };

            CollectionAssert.AreEquivalent(
                new[] { 1 },
                tracks.WhereDownloadable().Select(track => track.Id));
        }

        [Test]
        public void WhereDownloadable_Excludes_Tracks_With_An_Empty_Url()
        {
            var tracks = new[] { TrackWithMedia(1, AudioMedia(File(100, url: string.Empty))) };

            CollectionAssert.IsEmpty(tracks.WhereDownloadable());
        }

        [Test]
        public void WhereDownloadable_Skips_Null_Tracks()
        {
            var tracks = new[] { _fakeTrackFactory.CreateTrackWithId(1), null };

            CollectionAssert.AreEquivalent(
                new[] { 1 },
                tracks.WhereDownloadable().Select(track => track.Id));
        }

        [Test]
        public void WhereDownloadable_Handles_A_Null_Collection()
        {
            IEnumerable<Track> tracks = null;

            CollectionAssert.IsEmpty(tracks.WhereDownloadable());
        }

        #endregion
    }
}
