using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace DockerApi {
    public class FileHelper {
        public static JObject data;
        public static string pathDataJson = Path.Combine (Directory.GetCurrentDirectory (), @"data.txt");

        /// <summary>
        /// Get cache value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static JToken GetValue (string key) {
            if (data == null) {
                if (!File.Exists (pathDataJson)) {
                    return null;
                }
                string json = File.ReadAllText (pathDataJson);
                data = JsonConvert.DeserializeObject<JObject> (json);

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
        public async static void Add (string key, object value) {

            if (data == null) {
                data = new JObject ();
            }
            data[key] = JToken.FromObject (value);
            //convert object to json string.
            string json = data.ToStrJson ();

            if (!File.Exists (pathDataJson))
                File.Create (pathDataJson);

            try {
                File.WriteAllText (pathDataJson, json);

            } catch (System.Exception ex) {
                Console.WriteLine (ex.Message);
            }

        }

        /// <summary>
        /// Delete cache value from key
        /// </summary>
        /// <param name="key"></param>
        public static void Delete (string key) {
            if (data != null) {
                data.Remove (key);
                //convert object to json string.
                string json = data.ToStrJson ();

                //export data to json file. 
                using (TextWriter tw = new StreamWriter (pathDataJson)) {
                    tw.WriteLine (json);
                };
            }

        }

        public static void CreateExcel (string nameFile, List<List<object>> data) {
            if (File.Exists(nameFile)) File.Delete(nameFile);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage ()) {
                excel.Workbook.Worksheets.Add ("Mẫu file");
                int indexRow = 1;
                var worksheet = excel.Workbook.Worksheets["Mẫu file"];
                foreach (var row in data)
                {
                    var values = new List<object[]> () {row.ToArray()};
                    string rowRange = $"A{indexRow}:" + Char.ConvertFromUtf32 (row.Count + 64) + indexRow;
                    worksheet.Cells[rowRange].LoadFromArrays (values);
                    indexRow++;
                }
                FileInfo excelFile = new FileInfo (nameFile);
                excel.SaveAs (excelFile);
            }
        }
    }
}