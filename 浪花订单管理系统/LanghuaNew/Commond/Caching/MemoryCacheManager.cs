using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Web.Caching;

namespace Commond.Caching
{
    public partial class MemoryCacheManager : ICacheManager
    {
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }
        public DateTime GetLastCacheTime(string key)
        {
            string keystr = key + "-LastCacheTime";
            return (DateTime)Cache[keystr];
        }

        public virtual T Get<T>(string key, int cacheTime, T data)
        {
            if (IsSet(key))
            {
                return Get<T>(key);
            }
            var result = data;
            if (cacheTime > 0)
                Set(key, result, cacheTime);
            return result;
        }
        public virtual T Get<T>(string key, int cacheTime, Func<T> callback)
        {
            if (IsSet(key))
            {
                return Get<T>(key);
            }
            var result = callback();
            if (cacheTime > 0)
                Set(key, result, cacheTime);
            return result;
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key, data), policy);
            Cache.Add(new CacheItem(key + "-LastCacheTime", DateTime.Now), policy);
        }

        public virtual bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }

        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

        public virtual void Dispose()
        {
        }
    }
}
