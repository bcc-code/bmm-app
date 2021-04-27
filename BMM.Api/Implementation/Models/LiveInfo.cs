using System;
using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    public class LiveInfo
    {
        public Track Track { get; set; }

        /// <summary>
        /// We send this in case the clock of the user's device is not accurate.
        /// </summary>
        public DateTime ServerTime { get; set; }


        /// <summary>
        /// Time when this request was received. Helps us to deal with situations where the device clock is off.
        /// It's actually off from the <see cref="ServerTime"/> by a little bit since it takes ~200ms for the request to be sent over the internet.
        /// </summary>
        [JsonIgnore]
        public DateTime LocalTime { get; set; }
    }
}