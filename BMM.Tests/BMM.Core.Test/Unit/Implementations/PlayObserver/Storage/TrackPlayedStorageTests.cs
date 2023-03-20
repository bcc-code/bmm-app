using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver.Storage;
using BMM.Core.Implementations.Storage;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.PlayObserver.Storage
{
    [TestFixture]
    public class TrackPlayedStorageTests
    {
        private IPreferences _preferencesMock;

        [SetUp]
        public void Setup()
        {
            _preferencesMock = Substitute.For<IPreferences>();
            AppSettings.SetImplementation(_preferencesMock);
        }

        [Test]
        public async Task AddAndRemoveDoRemoveTheRightElements()
        {
            var storage = new TrackPlayedStorage();

            var event1 = new TrackPlayedEvent { Id = Guid.NewGuid(), Track = new Track() };
            var event2 = new TrackPlayedEvent { Id = Guid.NewGuid(), Track = new Track() };
            var event3 = new TrackPlayedEvent { Id = Guid.NewGuid(), Track = new Track() };

            _preferencesMock
                .Get<string>(nameof(AppSettings.FinishedTrackPlayedEvents), null)
                .Returns(JsonConvert.SerializeObject(new[] { event1, event2, event3 }));

            await storage.DeleteEvents(new List<TrackPlayedEvent> { event2, event3 });
            
            _preferencesMock
                .Received(1)
                .Set(
                    Arg.Is<string>(a => a == nameof(AppSettings.FinishedTrackPlayedEvents)),
                    Arg.Is<string>(v => CheckRemainingItems(v, 1)));
            
        }

        private bool CheckRemainingItems(string itemsJson, int expectedCount)
        {
            var remainingItems = JsonConvert.DeserializeObject<IList<TrackPlayedEvent>>(itemsJson);
            return remainingItems.Count == expectedCount;
        }
    }
}