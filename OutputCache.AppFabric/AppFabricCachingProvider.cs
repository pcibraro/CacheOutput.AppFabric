using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.OutputCache.Core.Cache;

namespace CacheOutput.AppFabric
{
    public class AppFabricCachingProvider : IApiOutputCache
    {
        readonly static DataCacheFactory Factory = new DataCacheFactory();
        const string Region = "OutputCache";
        readonly DataCache cache;

        public AppFabricCachingProvider(string cacheName)
        {
            this.cache = Factory.GetCache(cacheName);
            this.cache.CreateRegion(Region);
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            var exp = expiration - DateTime.Now;

            if (dependsOnKey == null)
            {
                dependsOnKey = key;
            }

            cache.Put(key, o, new[] { new DataCacheTag(dependsOnKey) }, Region);
        }

        public bool Contains(string key)
        {
            var result = this.cache.Get(key, Region);
            if (result != null) return true;

            return false;
        }

        public object Get(string key)
        {
            var result = this.cache.Get(key, Region);
            return result; 
        }

        public T Get<T>(string key) where T : class
        {
            var result = this.cache.Get(key, Region) as T;
            return result; 
        }

        public void Remove(string key)
        {
            this.cache.Remove(key, Region);
        }

        public void RemoveStartsWith(string key)
        {
            var objs = this.cache.GetObjectsByTag(new DataCacheTag(key), Region);
            foreach(var o in objs)
            {
                this.cache.Remove(o.Key, Region);
            }
        }
    }
}
