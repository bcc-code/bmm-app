using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class ApiInfo
    {
        public ApiInfo()
        {
            SystemStatus = new Dictionary<string, bool>();
            Languages = new List<string>();
        }

        public string Documentation { get; set; }

        public IEnumerable<string> Languages { get; set; }

        public string Name { get; set; }

        public IDictionary<string, bool> SystemStatus { get; set; }
    }
}