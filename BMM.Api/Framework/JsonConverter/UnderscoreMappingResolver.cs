using Newtonsoft.Json.Serialization;

namespace BMM.Api.Framework.JsonConverter
{
    public class UnderscoreMappingResolver : DefaultContractResolver
    {
        public UnderscoreMappingResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }
}