using System.Net.Http;
using System.Threading.Tasks;

namespace BMM.Api.Framework.HTTP
{
    public interface IResponseDeserializer
    {
        Task<T> DeserializeResponse<T>(HttpResponseMessage response);
    }
}