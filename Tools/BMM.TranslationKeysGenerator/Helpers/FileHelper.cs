using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BMM.TranslationKeysGenerator.Helpers
{
    public static class FileHelper
    {
        public static JObject GetTranslations(string inputFile)
        {
            string inputFileContent = File.ReadAllText(inputFile);
            return JsonConvert.DeserializeObject<JObject>(inputFileContent);
        }
    }
}