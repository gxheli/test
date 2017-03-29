using System;
using System.Collections.Generic;
using System.Text;

namespace Commond.Caching
{
    public interface ICacheManager : IDisposable
    {
        T Get<T>(string key);

        DateTime GetLastCacheTime(string key);

        T Get<T>(string key, int cacheTime, T data);

        T Get<T>(string key, int cacheTime, Func<T> callback);

        void Set(string key, object data, int cacheTime);

        bool IsSet(string key);

        void Remove(string key);

        void Clear();
    }
}
