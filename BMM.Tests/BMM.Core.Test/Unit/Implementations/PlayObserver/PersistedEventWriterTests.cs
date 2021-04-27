using BMM.Core.Implementations.PlayObserver;
using BMM.Core.Implementations.Startup;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.PlayObserver
{
    [TestFixture]
    public class PersistedEventWriterTests
    {
        /// <summary>
        /// If <see cref="TrackPlayedEvent"/> is written too often it can cause concurrency issues in <see cref="PersistedEventWriter"/>.
        /// </summary>
        [Test]
        public void StartupDelayIsSmallerThanInterval()
        {
            Assert.Less(StartupManager.StartupDelayInSeconds,
                PersistingPlayStatisticsDecorator.IntervalInSeconds,
                "It was built assuming that StartupDelay is less and might cause unexpected problems if not.");
        }
    }
}