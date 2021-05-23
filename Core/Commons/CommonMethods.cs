using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;

namespace DockerApi
{
    public static class CommonMethods
    {

        #region Xử lý IWebDriver
        public static Screenshot TakeScreenshot(IWebDriver driver)
        {
            try
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"Images\SeleniumTestingScreenshot.png");
                ss.SaveAsFile(path, ScreenshotImageFormat.Png);
                return ss;
            }
            catch (Exception e)
            {
                throw new Exception("Không thể chụp lại được màn hình: " + e.Message);
                
            }
        }
        /// <summary>
        /// ReadRecaptcha: Xử lý đọc recaptcha
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idRecaptcha">Id của Recaptcha</param>
        /// <param name="idReload">Id của Reload Recaptcha</param>
        /// <returns></returns>
        public static string ReadRecaptcha(IWebDriver driver, string idRecaptcha, string idReload)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            try
            {
                js.ExecuteScript("return document.getElementsByClassName('meshim_widget_components_chatButton_ButtonBar')[0].remove();");
            }
            catch (System.Exception)
            {
                
            }
            var strResult = "";
            Boolean horzscrollStatus = (Boolean)js.ExecuteScript("return document.documentElement.scrollWidth>document.documentElement.clientWidth;");

            try
            {
                var eleReCaptcha = driver.FindElement(By.Id(idRecaptcha));
                eleReCaptcha.Click();
                int loop = 1000; // tronghuu95 20210325150000 xử lý an toàn nên chuyển từ while sang for
                for (int i = 0; i < loop; i++)
                {
                    eleReCaptcha.Click();
                    Screenshot ss = TakeScreenshot(driver);
                    var arrScreen = ss.AsByteArray;
                    var msScreen = new MemoryStream(arrScreen);
                    Bitmap bitmap = new Bitmap(msScreen);
                    Point location = new Point(eleReCaptcha.Location.X, bitmap.Size.Height - eleReCaptcha.Size.Height - (horzscrollStatus ? 15 : 0));// 15 la thanh cuon
                    Bitmap bn = bitmap.Clone(new Rectangle(location, new Size(eleReCaptcha.Size.Width - 20, eleReCaptcha.Size.Height)), bitmap.PixelFormat);
                    bn = OCR_Recaptcha.FormatImageRecaptcha(bn);
                    var pathFormatImageRecaptcha = Path.Combine(Directory.GetCurrentDirectory(), @"Images\FormatImageRecaptcha.png");
                    bn.Save(pathFormatImageRecaptcha);   
                    Pix img = ConvertBitmapToPix(bn);
                    LogWriter.LogWrite($"LoadFromMemory");
                    strResult = OCR_Recaptcha.OCR(img);
                    LogWriter.LogWrite($"strResult {strResult}");
                    strResult = strResult.Replace("\n", "");
                    if (String.IsNullOrEmpty(strResult) || strResult.Length == 0 || strResult.Length != 4 || strResult.Contains(" "))
                    {
                        strResult = "";
                        var eleReloadCaptcha = driver.FindElement(By.Id(idReload));
                        eleReloadCaptcha.Click();
                    }
                    if (!String.IsNullOrEmpty(strResult)) break;
                }
                if(String.IsNullOrEmpty(strResult))
                {
                    throw new Exception(Messages.ERR_Not_Read_Recaptch);
                }
                return strResult;
            }
            catch (System.Exception ex)
            {
                
                throw new Exception($"errorCatch - {strResult}: {ex.Message}");
            }
            
        }
        /// <summary>
        /// ImageToByteArray chuyển đổi Bitmap hoặc Image to ByteArray
        /// </summary>
        /// <param name="img">Là Image hoặc Bitmap</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Bitmap img)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            }
            catch (System.Exception ex)
            {
                LogWriter.LogWrite($"ImageToByteArray {ex.Message}");
                throw new Exception($"ImageToByteArray: {ex.Message}");
            }
            
        }
        public static Pix ConvertBitmapToPix(Bitmap img)
        {
            try
            {
                return Pix.LoadFromMemory(ImageToByteArray(img));

            }
            catch (System.Exception ex)
            {
                LogWriter.LogWrite($"BitmapToPix: {ex.Message}");
                throw new Exception($"BitmapToPix: {ex.Message}");
            }

        }

        /// <summary>
        /// SetInput gán giá trị vào thẻ input
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idInput"></param>
        /// <param name="value"></param>
        public static void SetInput(IWebDriver driver, string idInput, object value)
        {
            if(value is int)
            {
                value = (int)value > 0 ? value : null;
            }
            var ele = driver.FindElement(By.Id(idInput));
            ele.Clear();
            ele.SendKeys(Convert.ToString(value));
        }
        /// <summary>
        /// SelectLiByText: chọn combobox thẻ li theo text
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idDrop"></param>
        /// <param name="text"></param>
        public static void SelectLiByText(IWebDriver driver, string idDrop, string text)
        {
          
            //if ()
            //{

            //}
            //driver.FindElement(By.Id(idDrop)).Click();
            //var listDrop = driver.FindElement(By.Id(idListDrop));
            //var listLi = listDrop.FindElements(By.TagName("li"));
            //var el = listLi.SingleOrDefault(item =>
            //{
            //    return item.Text == text;
            //});
            //if (el != null)
            //{
            //    el.Click();
            //}
        }

        /// <summary>
        /// SelectLi: chọn combobox thẻ li
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idDrop"></param>
        /// <param name="value"></param>
        public static void SelectLi(IWebDriver driver, string idDrop, object value, string text = null)
        {
            
            string idListDrop = idDrop + "Options";
            var eleDrop = driver.FindElement(By.Id(idDrop));
            eleDrop.Click();
            var listDrop = driver.FindElement(By.Id(idListDrop));
            var listLi = listDrop.FindElements(By.TagName("li"));
            try
            {
                if (value != null && ((value.GetType() == typeof(string) && !string.IsNullOrEmpty((string)value)) || (value.GetType() == typeof(int) && (int)value > 0)))
                {
                    var el = listLi.SingleOrDefault(item =>
                    {
                        var vl = item.GetAttribute("vl");
                        return vl == Convert.ToString(value);

                    });
                    if (el != null) el.Click();

                }
                else
                {
                    text = text.ToLower();
                    var el = listLi.SingleOrDefault(item =>
                    {
                        var itext = item.Text.ToLower();
                        return itext == text;

                    });
                    if (el == null)
                    {
                        var ntext = text.Replace("tỉnh", "")
                            .Replace("thành phố ", "")
                            .Replace("tp.", "")
                            .Replace("quận ", "")
                            .Replace("huyện ", "")
                            .Replace("phường ", "")
                            .Replace("xã ", "").Trim();
                        el = listLi.SingleOrDefault(item =>
                        {
                            var itext = item.Text.ToLower();
                            return itext == ntext;

                        });
                    }
                    if (el != null) el.Click();

                }
            }
            catch (Exception)
            {
                listLi[0].Click();
            }
            
        }
        /// <summary>
        /// SelectOptions: chon combobox thẻ option
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idOption"></param>
        /// <param name="value"></param>
        public static void SelectOptions(IWebDriver driver, string idOption, object value)
        {
            var selectElement = new SelectElement(driver.FindElement(By.Id(idOption)));
            try
            {
                selectElement.SelectByValue(Convert.ToString(value));

            }
            catch (Exception)
            {
                selectElement.SelectByIndex(0);
            }
        }

        /// <summary>
        /// UploadImages upload image
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="nameInputUpload"></param>
        /// <param name="strIds"></param>

        public static void UploadImages(IWebDriver driver, string nameInputUpload, string strIds)
        {
            if (string.IsNullOrEmpty(strIds)) return;
            strIds = strIds.Replace(" ", "");
            List<int> lIds = strIds.Split(',').Select(int.Parse).ToList();
            List<string> lPath = new List<string>();
            foreach (var id in lIds)
            {
                string path = Variables.SELENIUM_PATH_UPLOADS.EndsWith('\\')? Variables.SELENIUM_PATH_UPLOADS :  Variables.SELENIUM_PATH_UPLOADS + '\\';
                path = string.Format(path + "{0}.jpg", id);
                if (File.Exists(path))
                {
                    lPath.Add(path);
                }
            }
            if (lPath.Count > 0)
            {
                IWebElement element = driver.FindElement(By.Name(nameInputUpload));
                element.SendKeys(String.Join("\n ", lPath));
                //Thread.Sleep(300); // Ngừng lại 
            }

        }

        #endregion
        #region SerializeToJSON to String
        public static string SerializeToJSON(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
        }

        private static JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };
       
        public static string SerializeToJSON(object obj, bool ignoreReferenceLoop = true)
        {
            JsonSerializerSettings settings = null;
            if (ignoreReferenceLoop)
            {
                settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            }
            else
            {
                settings = DefaultJsonSerializerSettings;
            }

            return SerializeToJSON(obj, settings);
        }
       
        public static string SerializeToJSON(object obj, JsonSerializerSettings settings)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {

                if (settings == null)
                {
                    settings = DefaultJsonSerializerSettings;
                }

                return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }
        }
        public static void LoadSettings()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile($"appsettings.{Variables.EnvironmentName}.json", optional: true)
           .AddEnvironmentVariables();
            Variables.Configuration = builder.Build();
            Variables.SELENIUM_PATH_UPLOADS = Variables.Configuration.GetSection("AppSettings")["SELENIUM_PATH_UPLOADS"] ?? "path";
        }
        #endregion
    }
    
}
