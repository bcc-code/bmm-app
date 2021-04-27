using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Caching
{
    public class CachedResponseMessage : ICacheItem<byte[]>
    {
        public Dictionary<string, IEnumerable<string>> Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public DateTime Created { get; set; }
        public byte[] Value { get; set; }

        public async Task Initialize(HttpResponseMessage response)
        {
            await Initialize(response, DateTime.UtcNow);
        }

        public async Task Initialize(HttpResponseMessage response, DateTime created)
        {
            Created = created;
            Value = await response.Content.ReadAsByteArrayAsync();

            StatusCode = response.StatusCode;
            Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value);
        }

        public HttpResponseMessage ToResponseMessage()
        {
            var message = new HttpResponseMessage(StatusCode);

            if (Value != null)
            {
                message.Content = new ByteArrayContent(Value);
            }

            if (Headers != null)
            {
                foreach (var header in Headers)
                {
                    message.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return message;
        }
    }
}