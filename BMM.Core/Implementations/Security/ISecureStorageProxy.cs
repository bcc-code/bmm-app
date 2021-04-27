using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface ISecureStorageProxy
    {
        Task SetAsync(string key, string value);

        Task<string> GetAsync(string key);

        void RemoveAll();

        Task<DateTime?> GetDateAsync(string key);

        Task SetDateAsync(string key, DateTime date);
    }
}