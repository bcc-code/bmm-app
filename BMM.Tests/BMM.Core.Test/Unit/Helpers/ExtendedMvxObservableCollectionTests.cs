using System.Collections.Generic;
using BMM.Core.Helpers;
using BMM.Core.Test.Helpers;
using MvvmCross.Base;
using MvvmCross.Tests;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Helpers
{
    [TestFixture]
    public class ExtendedMvxObservableCollectionTests: MvxIoCSupportingTest
    {

        [Test]
        public  void RemoveAt_ShouldNotTriggerOnCollectionChanged()
        {
            // Arrange
            LocalSetup();
            int collectionChangedCalls = 0;
            var collection = new ExtendedMvxObservableCollection<string>(new List<string>()
            {
                "one", "two", "three"
            });

            collection.CollectionChanged += (sender, e) =>
            {
                collectionChangedCalls++;
            };

            // Act
            collection.RemoveAtWithoutTriggeringEvents(2);

            // Assert
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(0, collectionChangedCalls);
        }

        [Test]
        public void Move_ShouldNotTriggerOnCollectionChanged()
        {
            // Arrange
            LocalSetup();
            int collectionChangedCalls = 0;
            var collection = new ExtendedMvxObservableCollection<string>(new List<string>()
            {
                "one", "two", "three"
            });

            collection.CollectionChanged += (sender, e) =>
            {
                collectionChangedCalls++;
            };

            // Act
            collection.MoveWithoutTriggeringEvents(0,2);

            // Assert
            Assert.AreEqual("one", collection[2]);
            Assert.AreEqual(0, collectionChangedCalls);
        }

        private void LocalSetup()
        {
            base.Setup();
            base.AdditionalSetup();
            var mockDispatcher = new MockDispatcher();
            Ioc.RegisterSingleton<IMvxMainThreadDispatcher>(mockDispatcher);
            Ioc.RegisterSingleton<IMvxMainThreadAsyncDispatcher>(mockDispatcher);
        }
    }
}
