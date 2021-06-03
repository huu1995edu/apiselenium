
using System;
using System.Runtime.Caching;
namespace DockerApi
{
    public class MemoryCacheHelper
    {

        /// <summary>
        /// Get cache value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(string key)
        {
           var value = MemoryCache.Default.Get(key);
            LogSystem.Write($"Get key:{key} -value: {value}");

            return value;
        }

        /// <summary>
        /// Add a cache object with date expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absExpiration"></param>
        /// <returns></returns>
        public static bool Add(string key, object value)
        {
            Delete(key);
            var absExpiration = DateTimeOffset.UtcNow.AddHours(24);
            LogSystem.Write($"Add key:{key} -value: {string.Join(",", value)}");
            return MemoryCache.Default.Add(key, value, absExpiration);
        }

        /// <summary>
        /// Delete cache value from key
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }
    }
}
