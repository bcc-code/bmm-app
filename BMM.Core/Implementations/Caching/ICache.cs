using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Caching
{
    public interface ICache
    {
        Task Clear();

        Task<CachedItem<T>> GetOrFetchObject<T>(string key, Func<Task<T>> fetchFunc, DateTimeOffset? absoluteExpiration = null);

        Task UpdateItem<T>(string key, CachedItem<T> item);

        Task<T> GetObject<T>(string key);

        Task InsertObject<T>(string key, T obj);
    }

    public class CachedItem<T>
    {
        /// <summary>
        /// This constructor is needed for deserializing from Json
        /// </summary>
        public CachedItem(T item)
        {
            Item = item;
        }

        private CachedItem(T item, DateTime dateCreated)
        {
            DateCreated = dateCreated;
            Item = item;
        }

        public T Item { get; }

        public DateTime DateCreated
        {
            get;
            // ReSharper disable All
            // This setter is necessary to exist and be public because in GetOrFetchObject we deserialize the CachedItem
            // and set DateCreated to be the value from the Local Storage, which is the date when this item was cached
            set;
        }

        public static CachedItem<T> New(T item)
        {
            return new CachedItem<T>(item, DateTime.UtcNow);
        }
    }
}