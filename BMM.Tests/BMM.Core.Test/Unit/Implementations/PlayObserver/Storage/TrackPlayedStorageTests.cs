using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver.Storage;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.PlayObserver.Storage
{
    [TestFixture]
    public class TrackPlayedStorageTests
    {
        [Test]
        public async Task AddAndRemoveDoRemoveTheRightElements()
        {
            var storage = new TrackPlayedStorage(BlobCache.InMemory);

            var event1 = new TrackPlayedEvent {Id = Guid.NewGuid()};
            var event2 = new TrackPlayedEvent {Id = Guid.NewGuid()};
            var event3 = new TrackPlayedEvent {Id = Guid.NewGuid()};

            var eventsToBeAdded = new List<TrackPlayedEvent>
            {
                event1, event2, event3
            };

            await storage.Add(eventsToBeAdded);

            await storage.DeleteEvents(new List<TrackPlayedEvent> {event2, event3});

            var remainingEvents = await storage.GetExistingEvents();

            Assert.IsNotEmpty(remainingEvents);
            Assert.AreEqual(event1.Id, remainingEvents.First().Id);
        }

    }
}