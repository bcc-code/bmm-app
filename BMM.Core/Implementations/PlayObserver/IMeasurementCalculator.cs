using System.Collections.Generic;
using BMM.Core.Implementations.PlayObserver.Model;

namespace BMM.Core.Implementations.PlayObserver
{
    public interface IMeasurementCalculator
    {
        PlayMeasurements Calculate(long trackDuration, IList<ListenedPortion> portions, bool skippedTrack = false);
    }
}