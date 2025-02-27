using System.Runtime.Caching;
using eTactWeb.Services.Interface;

namespace eTactWeb.Data.Common
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private ObjectCache _memoryCache = MemoryCache.Default;

        public T GetDataCache<T>(string key)
        {
            try
            {
                T item = (T)_memoryCache.Get(key);
                return item;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public object RemoveDataCache(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    return _memoryCache.Remove(key);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return false;
        }

        public bool SetDataCache<T>(string key, T value, DateTimeOffset expirationTime)
        {
            bool res = true;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _memoryCache.Set(key, value, expirationTime);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return res;
        }
    }
}