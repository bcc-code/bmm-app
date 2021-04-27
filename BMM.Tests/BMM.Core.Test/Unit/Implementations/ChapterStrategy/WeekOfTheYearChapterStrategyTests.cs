using System;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.ChapterStrategy;
using BMM.Core.Models;
using Moq;
using MvvmCross.Localization;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.ChapterStrategy
{
    public class WeekOfTheYearChapterStrategyTests
    {
        private WeekOfTheYearChapterStrategy _chapterStrategy;
        private Mock<IMvxLanguageBinder> _textSource = new Mock<IMvxLanguageBinder>();


        [SetUp]
        public void Setup()
        {
            _chapterStrategy = new WeekOfTheYearChapterStrategy(_textSource.Object);
        }

        [Test]
        public void TestCreatesHeaderForDifferentWeeks()
        {
            var newTracks = new List<Track>
            {
                new Track {RecordedAt = DateTime.Parse("2010-08-20")},
                new Track {RecordedAt = DateTime.Parse("2010-01-20")},
                new Track {RecordedAt = DateTime.Parse("2010-05-20")}
            };

            var result = _chapterStrategy.AddChapterHeaders(newTracks, new List<Document>());

            var numberOfChapters = result.Count(r => r is ChapterHeader);

            Assert.AreEqual(3, numberOfChapters);
        }

        [Test]
        public void TestCreatesHeaderForSameCalendarWeekInDifferentYears()
        {
            var newTracks = new List<Track>
            {
                new Track {RecordedAt = DateTime.Parse("2011-05-21")},
                new Track {RecordedAt = DateTime.Parse("2009-05-17")}
            };

            var result = _chapterStrategy.AddChapterHeaders(newTracks, new List<Document>());

            var numberOfChapters = result.Count(r => r is ChapterHeader);

            Assert.AreEqual(2, numberOfChapters);
        }
    }
}