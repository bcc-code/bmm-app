using System.Collections.Generic;

namespace BMM.Api.Utils
{
    public static class SongbookUtils
    {
        private static IDictionary<string, string> NameMappingDictionary { get; } = new Dictionary<string, string>()
        {
            { "herrens_veier", "HV" },
            { "mandelblomsten", "FMB" },
        };

        public static string GetShortName(string name)
        {
            return NameMappingDictionary.TryGetValue(name, out string shortName)
                ? shortName
                : string.Empty;
        }
    }
}