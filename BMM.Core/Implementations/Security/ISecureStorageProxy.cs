using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    public interface ISecureStorageProxy
    {
        Task SetAsync(string key, string value);

        Task SetAsync<TValue>(string key, TValue value);

        Task<string> GetAsync(string key);

        Task<TValue> GetAsync<TValue>(string key, TValue defaultValue = default);

        void RemoveAll();

        Task<DateTime?> GetDateAsync(string key);

        Task SetDateAsync(string key, DateTime date);
    }
}