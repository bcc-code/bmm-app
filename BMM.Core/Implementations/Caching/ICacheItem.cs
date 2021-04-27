using System;

namespace BMM.Core.Implementations.Caching
{
    public interface ICacheItem<T>
    {
        DateTime Created { get; }
        T Value { get; }
    }
}