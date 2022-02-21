using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Constants;

namespace BMM.Api.RequestInterceptor
{
    public class JsonContentTypeHeaderProvider : IHeaderProvider
    {
        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            return new KeyValuePair<string, string>(HeaderNames.Accept, "application/json");
        }
    }
}