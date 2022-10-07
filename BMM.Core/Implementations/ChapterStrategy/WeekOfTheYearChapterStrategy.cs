using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Localization;

namespace BMM.Core.Implementations.ChapterStrategy
{
    public class WeekOfTheYearChapterStrategy : IChapterStrategy
    {
        private readonly Calendar _calendar;
        private readonly DateTimeFormatInfo _currentInfo;

        public WeekOfTheYearChapterStrategy()
        {
            _currentInfo = DateTimeFormatInfo.CurrentInfo;
            _calendar = _currentInfo.Calendar;
        }

        private IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;

        public IList<Document> AddChapterHeaders(IList<Track> tracks, IEnumerable<IDocumentPO> existingDocs)
        {
            var chapters = existingDocs.OfType<ChapterHeaderPO>().ToList();
            var groupedTracks = tracks.GroupBy(GroupByWeekOfTheYearAndYear);

            var trackWithChapters = new List<Document>();
            foreach (var groupedTrack in groupedTracks)
            {
                var tracksOfWeek = groupedTrack.OfType<Document>().ToList();
                var chapterId = CreateIdForWeekOfTheYearAndYear(groupedTrack.Key);

                if (!ChapterForIdExists(chapterId, chapters))
                {
                    var chapter = new ChapterHeader
                    {
                        Id = chapterId,
                        Title = GetTitleForWeekOfYearInYear(groupedTrack.Key.weekOfTheYear, groupedTrack.Key.year)
                    };
                    tracksOfWeek.Insert(0, chapter);
                }

                trackWithChapters.AddRange(tracksOfWeek);
            }

            return trackWithChapters;
        }

        private (int weekOfTheYear, int year) GroupByWeekOfTheYearAndYear(Track track)
        {
            var weekOfTheYear = _calendar.GetWeekOfYear(track.RecordedAt, _currentInfo.CalendarWeekRule, _currentInfo.FirstDayOfWeek);
            return ValueTuple.Create(
                weekOfTheYear,
                track.RecordedAt.Year
            );
        }

        private int CreateIdForWeekOfTheYearAndYear((int weekOfTheYear, int year) groupedTrackKey)
        {
            return int.Parse(groupedTrackKey.weekOfTheYear.ToString() + groupedTrackKey.year.ToString());
        }

        private bool ChapterForIdExists(int chapterId, List<ChapterHeaderPO> chapters)
        {
            return chapters.Any(c => c.Id == chapterId);
        }

        private string GetTitleForWeekOfYearInYear(int weekOfTheYear, int year)
        {
            if (IsCurrentCalendarWeekInCurrentYear(weekOfTheYear, year))
                return TextSource[Translations.PodcastViewModel_ThisWeek];

            if (IsPreviousCalendarWeekInCurrentYear(weekOfTheYear, year))
                return TextSource[Translations.PodcastViewModel_LastWeek];

            return TextSource.GetText(Translations.PodcastViewModel_Week, weekOfTheYear);
        }

        private bool IsCurrentCalendarWeekInCurrentYear(int week, int year)
        {
            return week == _calendar.GetWeekOfYear(DateTime.Now, _currentInfo.CalendarWeekRule, _currentInfo.FirstDayOfWeek)
                   && year == DateTime.Now.Year;
        }

        private bool IsPreviousCalendarWeekInCurrentYear(int week, int year)
        {
            var currentDateLastWeek = _calendar.AddWeeks(DateTime.Now, -1);
            return week == _calendar.GetWeekOfYear(currentDateLastWeek, _currentInfo.CalendarWeekRule, _currentInfo.FirstDayOfWeek)
                   && year == DateTime.Now.Year;
        }
    }
}