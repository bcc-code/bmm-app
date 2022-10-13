using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                PreparePortion(0, 10000),
                PreparePortion(5000, 10000)
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
                PreparePortion(0, 90),
                PreparePortion(10000, 60000)
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
                PreparePortion(0, 5000),
                PreparePortion(50000, 60000)
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
                PreparePortion(0, 10000),
                PreparePortion(0, 60000),
                PreparePortion(50000, 60000),
                PreparePortion(90000, 100000)
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
                PreparePortion(0, 3000),
                PreparePortion(6000, 9000),
                PreparePortion(10000, 14000),
                PreparePortion(14000, 18000),
                PreparePortion(18000, 20000),
                PreparePortion(25000, 29000),
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
                PreparePortion(0, 0),
                PreparePortion(0, 0)
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
                PreparePortion(24085, 24915),
                PreparePortion(0, 2384),
                PreparePortion(16348, 24429)
            };

            // Act
            var result = _measurementCalculator.CalculateUniqueSecondsListened(listenedPortions, out _);

            // Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void CalculateSpentTime_VariousPlaybackSpeed()
        {
            // Arrange
            double expectedSpentTime = 18;
            
            var listenedPortions = new List<ListenedPortion>
            {
                PreparePortion(0, 6000, 1m),
                PreparePortion(6000, 12000, 1.5m),
                PreparePortion(12000, 22000, 1.25m)
            };

            // Act
            double result = _measurementCalculator.CalculateSpentTime(listenedPortions);

            // Assert
            Assert.AreEqual(expectedSpentTime, result);
        }
        
        private ListenedPortion PreparePortion(double start, double end, decimal playbackRate = 1)
        {
            return new ListenedPortion
            {
                Start = start,
                End = end,
                PlaybackRate = playbackRate
            };
        }
    }
}
