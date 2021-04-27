using System.IO;
using System.Threading.Tasks;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IFileClient
    {
        /// <summary>Gets the file at  the specified uri.</summary>
        Task<Stream> GetFile(string uri);
    }
}