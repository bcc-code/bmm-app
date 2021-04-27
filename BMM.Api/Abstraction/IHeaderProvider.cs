using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMM.Api.Abstraction
{
    public interface IHeaderProvider
    {
        Task<KeyValuePair<string, string>> GetHeader();
    }
}