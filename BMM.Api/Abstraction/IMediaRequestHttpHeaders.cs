using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMM.Api.Abstraction
{
    public interface IMediaRequestHttpHeaders
    {
        Task<IList<KeyValuePair<string, string>>> GetHeaders();
        IList<IHeaderProvider> HeaderProviders { get; }
    }
}