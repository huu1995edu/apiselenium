
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.Caching;
using System.Text;

namespace DockerApi
{
    public class FileHelper
    {
        public static JObject data;
        public static string pathDataJson = Path.Combine(Directory.GetCurrentDirectory(), @"data.txt");

        /// <summary>
        /// Get cache value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static JToken GetValue(string key)
        {
             if(data == null)
            {
                if (!File.Exists(pathDataJson))
                {
                    return null;
                }
                string json = File.ReadAllText(pathDataJson);
                data = JsonConvert.DeserializeObject<JObject>(json);

            }
            return data[key];
        }

        /// <summary>
        /// Add a cache object with date expiration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absExpiration"></param>
        /// <returns></returns>
        public async static void Add(string key, object value)
        {

            if (data == null)
            {
                data = new JObject();
            }
            data[key] = JToken.FromObject(value);
            //convert object to json string.
            string json = data.ToStrJson();
           
            if (!File.Exists(pathDataJson))
                File.Create(pathDataJson);

            try
            {
                File.WriteAllText(pathDataJson, json);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Delete cache value from key
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key)
        {
            if (data != null)
            {
                data.Remove(key);
                //convert object to json string.
                string json = data.ToStrJson();

                //export data to json file. 
                using (TextWriter tw = new StreamWriter(pathDataJson))
                {
                    tw.WriteLine(json);
                };
            }
            
        }
    }
}
