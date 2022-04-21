using System;

namespace BMM.Core.Implementations.PlayObserver.Model
{
    public class ListenedPortion
    {
        public double Start { get; set; }
        public DateTime StartTime { get; set; }
        public double End { get; set; }
        public DateTime EndTime { get; set; }
        public decimal PlaybackRate { get; set; }
    }
}