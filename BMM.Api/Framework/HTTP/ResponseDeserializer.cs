using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework.JsonConverter;
using Newtonsoft.Json;

namespace BMM.Api.Framework.HTTP
{
    public class ResponseDeserializer : IResponseDeserializer
    {
        public async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            if (response?.Content == null || response.StatusCode == HttpStatusCode.NoContent)
            {
                return default(T);
            }

            // Try to parse the response into the requested object
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new UnderscoreMappingResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var jsonString = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString, settings);
            }
            catch (Exception ex)
            {
                throw new DeserializationException("An error occurred while deserializing a http response to " + response.RequestMessage.RequestUri, ex);
            }
        }
    }
}