using System;
using System.IO;
using Microsoft.Framework.Caching;
using Microsoft.Framework.Caching.Redis;

namespace RedisCacheSample
{
    public class Program
    {
        /// <summary>
        /// This sample assumes that a redis server is running on the local machine. You can set this up by doing the following:
        /// Install this chocolatey package: http://chocolatey.org/packages/redis-64/
        /// run "redis-server" from command prompt.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            string key = "myKey";
            byte[] value = new byte[10];

            Console.WriteLine("Connecting to cache");
            var cache = new RedisCache(new RedisCacheOptions
            {
                Configuration = "localhost",
                InstanceName = "SampleInstance"
            });
            Console.WriteLine("Connected");

            Console.WriteLine("Setting");
            cache.Set(key, value, new CacheEntryOptions());
            Console.WriteLine("Set");

            Console.WriteLine("Getting");
            if (cache.TryGetValue(key, out value))
            {
                Console.WriteLine("Retrieved: " + value);
            }
            else
            {
                Console.WriteLine("Not Found");
            }

            Console.WriteLine("Refreshing");
            cache.Refresh(key);
            Console.WriteLine("Refreshed");

            Console.WriteLine("Removing");
            cache.Remove(key);
            Console.WriteLine("Removed");

            Console.WriteLine("Getting");
            if (cache.TryGetValue(key, out value))
            {
                Console.WriteLine("Retrieved: " + value);
            }
            else
            {
                Console.WriteLine("Not Found");
            }

            Console.ReadLine();
        }
    }
}
