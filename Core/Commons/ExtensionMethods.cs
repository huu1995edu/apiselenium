using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DockerApi {
    public static class ExtensionMethods {
        // The Random object this method uses.
        private static Random Rand = null;

        // Return num_items random values.
        public static List<T> GetListRandom<T> (
            this T[] values, int num_values) {
            // Create the Random object if it doesn't exist.
            if (Rand == null) Rand = new Random ();

            // Don't exceed the array's length.
            if (num_values >= values.Length)
                num_values = values.Length - 1;

            // Make an array of indexes 0 through values.Length - 1.
            int[] indexes =
                Enumerable.Range (0, values.Length).ToArray ();

            // Build the return list.
            List<T> results = new List<T> ();

            // Randomize the first num_values indexes.
            for (int i = 0; i < num_values; i++) {
                // Pick a random entry between i and values.Length - 1.
                int j = Rand.Next (i, values.Length);

                // Swap the values.
                int temp = indexes[i];
                indexes[i] = indexes[j];
                indexes[j] = temp;

                // Save the ith value.
                results.Add (values[indexes[i]]);
            }

            // Return the selected items.
            return results;
        }

        public static List<string> Keys (this JObject ob) {
            List<string> lKey = new List<string>();
            foreach (var item in ob)
            {
                lKey.Add(item.Key);
            }
            return lKey;
        }

        public static string ToStrJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

    }
}