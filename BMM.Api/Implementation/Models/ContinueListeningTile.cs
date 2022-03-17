using System;
using BMM.Api.Implementation.Models.Interfaces;

namespace BMM.Api.Implementation.Models
{
    public class ContinueListeningTile : Document, ITrackHolder
    {
        public Track Track { get; set; }
        public string BackgroundColor { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }
        public string Subtitle { get; set; }
        public string CoverUrl { get; set; }
        public DateTime? Date { get; set; }
        public int Percentage { get; set; }
        public Uri ShowAllLink { get; set; }
        public int? ShufflePodcastId { get; set; }
        public int LastPositionInMs { get; set; }
    }
}