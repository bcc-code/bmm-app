using System.Threading.Tasks;

namespace BMM.Core.Implementations.Persistence.Interfaces
{
    public interface IBlobCacheWrapper
    {
        Task<T> GetObject<T>(string key);
        Task InsertObject<T>(string key, T obj);
    }
}