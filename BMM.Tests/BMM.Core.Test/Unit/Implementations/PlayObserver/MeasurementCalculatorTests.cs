using System.Collections.Generic;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.PlayObserver;
using BMM.Core.Implementations.PlayObserver.Model;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.PlayObserver
{
    [TestFixture]
    public class MeasurementCalculatorTests
    {
        private MeasurementCalculator _measurementCalculator;

        [SetUp]
        public void Init()
        {
            _measurementCalculator = new MeasurementCalculator();
        }

        [Test]
        public void CalculateSpentTime_Sum()
        {
            // Arrange
            var listenedPortion = new List<ListenedPortion>
            {
                new ListenedPortion {Start = 0, End = 10000},
                new ListenedPortion {Start = 5000, End = 10000}
            };

            // Act
            var result = _measurementCalculator.CalculateSpentTime(listenedPortion);

            // Assert
            Assert.AreEqual(15, result);
        }

        [Test]
        public void CalculateUniqueSecondsListened_Status_SkippedBeginning()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>
            {
                new ListenedPortion {Start = 0, End = 90},
                new ListenedPortion {Start = 10000, End = 60000}
            };

            // Act
            var result = _measurementCalculator.CalculateUniqueSecondsListened(listenedPortions, out ListenedStatus status);

            // Assert
            Assert.AreEqual(ListenedStatus.SkippedBeginning, status);
            Assert.AreEqual(50, result);
        }

        [Test]
        public void CalculateUniqueSecondsListened_Jumped1()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>
            {
                new ListenedPortion {Start = 0, End = 5000},
                new ListenedPortion {Start = 50000, End = 60000}
            };

            // Act
            var result = _measurementCalculator.CalculateUniqueSecondsListened(listenedPortions, out ListenedStatus status);

            // Assert
            Assert.AreEqual(ListenedStatus.Jumped, status);
            Assert.AreEqual(15, result);
        }

        [Test]
        public void CalculateUniqueSecondsListened_Jumped2()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>
            {
                new ListenedPortion {Start = 0, End = 10000},
                new ListenedPortion {Start = 0, End = 60000},
                new ListenedPortion {Start = 50000, End = 60000},
                new ListenedPortion {Start = 90000, End = 100000}
            };

            // Act
            var result = _measurementCalculator.CalculateUniqueSecondsListened(listenedPortions, out ListenedStatus status);

            // Assert
            Assert.AreEqual(ListenedStatus.Jumped, status);
            Assert.AreEqual(70, result);
        }

        [Test]
        public void CalculateUniqueSecondsListened_SmallPortions()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>
            {
                new ListenedPortion {Start = 0, End = 3000},
                new ListenedPortion {Start = 6000, End = 9000},
                new ListenedPortion {Start = 10000, End = 14000},
                new ListenedPortion {Start = 14000, End = 18000},
                new ListenedPortion {Start = 18000, End = 20000},
                new ListenedPortion {Start = 25000, End = 29000}
            };

            // Act
            var result = _measurementCalculator.CalculateUniqueSecondsListened(listenedPortions, out ListenedStatus status);

            // Assert
            Assert.AreEqual(20, result);
        }

        [Test]
        public void Calculate_NoPortionReturnNull()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>();

            // Act
            var result = _measurementCalculator.Calculate(100, listenedPortions);

            // Assert
            Assert.AreEqual(null, result);
        }

        [Test]
        public void Calculate_EmptyPortionReturnsNull()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>
            {
                new ListenedPortion(),
                new ListenedPortion {Start = 0, End = 0}
            };

            // Act
            var result = _measurementCalculator.Calculate(100, listenedPortions);

            // Assert
            Assert.AreEqual(null, result);
        }

        [Test]
        public void CalculateUniqueSecondsListened_PortionsWhenPlayingATrackAfterQueueWasCompleted()
        {
            // Arrange
            var listenedPortions = new List<ListenedPortion>
            {
                new ListenedPortion {Start = 24085, End = 24915},
                new ListenedPortion {Start = 0, End = 2384},
                new ListenedPortion {Start = 16348, End = 24429},
            };

            // Act
            var result = _measurementCalculator.CalculateUniqueSecondsListened(listenedPortions, out _);

            // Assert
            Assert.AreEqual(11, result);
        }
    }
}
