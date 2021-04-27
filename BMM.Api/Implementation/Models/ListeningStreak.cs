using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BMM.Api.Implementation.Models
{
    public class ListeningStreak : Document
    {
        public ListeningStreak()
        {
            DocumentType = DocumentType.ListeningStreak;
        }

        public int Year { get; set; }

        public int WeekOfTheYear { get; set; }

        public int TodaysFraKaareTrackId { get; set; }

        public int NumberOfPerfectWeeks { get; set; }

        public bool IsPerfectWeek { get; set; }

        public int Total { get; set; }

        public bool? Monday { get; set; }

        public bool? Tuesday { get; set; }

        public bool? Wednesday { get; set; }

        public bool? Thursday { get; set; }

        public bool? Friday { get; set; }

        public DateTime LastChanged { get; set; }
        public DateTime EligibleUntil { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DayOfWeek DayOfTheWeek { get; set; }

        [JsonIgnore]
        public bool ShowMonday => Monday != null;
        [JsonIgnore]
        public bool ShowTuesday => Tuesday != null;
        [JsonIgnore]
        public bool ShowWednesday => Wednesday != null;
        [JsonIgnore]
        public bool ShowThursday => Thursday != null;
        [JsonIgnore]
        public bool ShowFriday => Friday != null;

        [JsonIgnore]
        public string MondayColor => Monday == true ? "#B2474E" : null;

        [JsonIgnore]
        public string TuesdayColor => Tuesday == true ? "#874A6C" : null;

        [JsonIgnore]
        public string WednesdayColor => Wednesday == true ? "#543B79" : null;

        [JsonIgnore]
        public string ThursdayColor => Thursday == true ? "#364AA8" : null;

        [JsonIgnore]
        public string FridayColor => Friday == true ? "#255BD0" : null;

        public string ToText()
        {
            return $"{Monday},{Tuesday},{Wednesday},{Thursday},{Friday}";
        }
    }
}