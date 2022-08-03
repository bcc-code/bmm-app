using System.Linq;
using BMM.Api.Framework;

namespace BMM.Core.Utils
{
    public static class AnalyticsUtils
    {
        private const string NoNetwork = "None";
        
        public static string GetConnectionType(IConnection connection)
        {
            var currentStatus = connection
                .GetActiveConnectionProfiles()
                .ToList();
            
            if (!currentStatus.Any())
                return NoNetwork;
            
            return string.Join(" & ", currentStatus);
        }
    }
}