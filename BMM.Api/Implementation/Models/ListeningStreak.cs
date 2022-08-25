using System;
using BMM.Api.Implementation.Models.Enums;
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
        
        public int DaysInARow { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public HomeScreenText HomeScreenText { get; set; }

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
        public string MondayColor => Monday == true ? "#DBE459" : null;

        [JsonIgnore]
        public string TuesdayColor => Tuesday == true ? "#B9CC68" : null;

        [JsonIgnore]
        public string WednesdayColor => Wednesday == true ? "#83A174" : null;

        [JsonIgnore]
        public string ThursdayColor => Thursday == true ? "#4E7780" : null;

        [JsonIgnore]
        public string FridayColor => Friday == true ? "#265789" : null;

        public string ToText()
        {
            return $"{Monday},{Tuesday},{Wednesday},{Thursday},{Friday}";
        }
    }
}