using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Tesseract;

namespace DockerApi {
    public static class CommonMethods {
        public static void notifycation_tele (string message) {
            string URL = $"https://api.telegram.org/bot1823763238:AAHc6-Dn80jdakWSbSIn938ElitKy2CpdsY/sendMessage";
            string urlParameters = $"?chat_id=-563181299&text={message}";

            try {

                WebClient webclient = new WebClient ();
                webclient.DownloadString (URL + urlParameters);
            } catch (Exception) {

            }
        }

        #region Xử lý IWebDriver
        public static Screenshot TakeScreenshot (IWebDriver driver) {
            try {
                Screenshot ss = ((ITakesScreenshot) driver).GetScreenshot ();
                var path = Path.Combine (Directory.GetCurrentDirectory (), @"Images\SeleniumTestingScreenshot.png");
                ss.SaveAsFile (path, ScreenshotImageFormat.Png);
                return ss;
            } catch (Exception e) {
                throw new Exception ("Không thể chụp lại được màn hình: " + e.Message);

            }
        }
        public static void RemovePopupChat (IWebDriver driver) {
            IJavaScriptExecutor js = (IJavaScriptExecutor) driver;

            try {
                Thread.Sleep (300);
                js.ExecuteScript ("const elements = document.getElementsByClassName('zopim'); while (elements.length > 0) elements[0].remove(); window.$zopim.livechat.window.hide(); return true;");
            } catch (System.Exception ex) {
                LogSystem.Write ($"ReadRecaptcha: {ex.Message}");

            }
        }

        /// <summary>
        /// ReadRecaptcha: Xử lý đọc recaptcha
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idRecaptcha">Id của Recaptcha</param>
        /// <param name="idReload">Id của Reload Recaptcha</param>
        /// <returns></returns>
        public static string ReadRecaptcha (IWebDriver driver, string idRecaptcha, string idReload) {
            RemovePopupChat (driver);
            IJavaScriptExecutor js = (IJavaScriptExecutor) driver;

            var strResult = "";
            Boolean horzscrollStatus = (Boolean) js.ExecuteScript ("return document.documentElement.scrollWidth>document.documentElement.clientWidth;");

            try {
                var eleReCaptcha = driver.FindElement (By.Id (idRecaptcha));
                eleReCaptcha.Click ();
                int loop = 1000; // tronghuu95 20210325150000 xử lý an toàn nên chuyển từ while sang for
                for (int i = 0; i < loop; i++) {
                    eleReCaptcha.Click ();
                    Screenshot ss = TakeScreenshot (driver);
                    var arrScreen = ss.AsByteArray;
                    var msScreen = new MemoryStream (arrScreen);
                    Bitmap bitmap = new Bitmap (msScreen);
                    Point location = new Point (eleReCaptcha.Location.X, bitmap.Size.Height - eleReCaptcha.Size.Height - (horzscrollStatus ? 15 : 0)); // 15 la thanh cuon
                    Bitmap bn = bitmap.Clone (new Rectangle (location, new Size (eleReCaptcha.Size.Width - 20, eleReCaptcha.Size.Height)), bitmap.PixelFormat);
                    bn = OCR_Recaptcha.FormatImageRecaptcha (bn);
                    var pathFormatImageRecaptcha = Path.Combine (Directory.GetCurrentDirectory (), @"Images\FormatImageRecaptcha.png");
                    bn.Save (pathFormatImageRecaptcha);
                    Pix img = ConvertBitmapToPix (bn);
                    strResult = OCR_Recaptcha.OCR (img);
                    strResult = strResult.Replace ("\n", "");
                    if (String.IsNullOrEmpty (strResult) || strResult.Length == 0 || strResult.Length != 4 || strResult.Contains (" ")) {
                        strResult = "";
                        var eleReloadCaptcha = driver.FindElement (By.Id (idReload));
                        eleReloadCaptcha.Click ();
                    }
                    if (!String.IsNullOrEmpty (strResult)) break;
                }
                if (String.IsNullOrEmpty (strResult)) {
                    throw new Exception (Messages.ERR_Not_Read_Recaptch);
                }
                return strResult;
            } catch (System.Exception ex) {

                throw new Exception ($"{strResult}: {ex.Message}");
            }

        }
        /// <summary>
        /// ImageToByteArray chuyển đổi Bitmap hoặc Image to ByteArray
        /// </summary>
        /// <param name="img">Là Image hoặc Bitmap</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray (Bitmap img) {
            using (var stream = new MemoryStream ()) {
                img.Save (stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray ();
            }

        }
        public static Pix ConvertBitmapToPix (Bitmap img) {
            return Pix.LoadFromMemory (ImageToByteArray (img));

        }

        /// <summary>
        /// SetInput gán giá trị vào thẻ input
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idInput"></param>
        /// <param name="value"></param>
        public static void SetInput (IWebDriver driver, string idInput, object value) {
            if (value is int) {
                value = (int) value > 0 ? value : null;
            }
            var ele = driver.FindElement (By.Id (idInput));
            ele.Clear ();
            ele.SendKeys (Convert.ToString (value));
        }

        public static void SetDateTime_BatDongSan (IWebDriver driver, string idInput, DateTime value) {
            try {
                var ele = driver.FindElement (By.Id (idInput));
                var valueOld = ele.GetAttribute ("value").Split ('/').ToList ();
                var ddOld = int.Parse (valueOld[0]);
                var mmOld = int.Parse (valueOld[1]);

                if (ele.Text == value.ToString ("dd/MM/yyyy")) return;
                ele.Click ();
                if (mmOld != value.Month) {
                    var isPrev = mmOld > value.Month;
                    var steps = Math.Abs (mmOld - value.Month);
                    var element = driver.FindElement (By.ClassName (isPrev ? "ui-datepicker-prev" : "i-datepicker-next"));
                    for (int i = 0; i < steps; i++) {
                        element.Click ();

                    }
                }
                if (ddOld != value.Day) {
                    var listDate = driver.FindElements (By.ClassName ("ui-state-default"));
                    foreach (var item in listDate) {
                        var day = int.Parse (item.Text);
                        if (value.Day == day) {
                            var parent = item.FindElement (By.XPath ("./.."));
                            if (parent != null) parent.Click ();

                        }
                    }

                }
            } catch (Exception) {

            }

            //ele.Clear();
            //ele.SendKeys(Convert.ToString(value));
        }
        /// <summary>
        /// SelectLiByText: chọn combobox thẻ li theo text
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idDrop"></param>
        /// <param name="text"></param>
        public static void SelectLiByText (IWebDriver driver, string idDrop, string text) {

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
        public static void SelectLi (IWebDriver driver, string idDrop, object value, string text = null) {
            string idListDrop = idDrop + "Options";
            var eleDrop = driver.FindElement (By.Id (idDrop));
            eleDrop.Click ();
            var listDrop = driver.FindElement (By.Id (idListDrop));
            var listLi = listDrop.FindElements (By.TagName ("li"));
            dynamic el = null;
            try {
                if (value != null && ((value.GetType () == typeof (string) && !string.IsNullOrEmpty ((string) value)) || (value.GetType () == typeof (int) && (int) value > 0))) {
                    el = listLi.FirstOrDefault (item => {
                        var vl = item.GetAttribute ("vl");
                        return vl == Convert.ToString (value);

                    });

                } else {
                    text = text.ToLower ();
                    el = listLi.FirstOrDefault (item => {
                        var itext = item.Text.ToLower ();
                        return itext == text;

                    });
                    if (el == null) {
                        var ntext = text.Replace ("tỉnh", "")
                            .Replace ("thành phố ", "")
                            .Replace ("tp.", "")
                            .Replace ("quận ", "")
                            .Replace ("huyện ", "")
                            .Replace ("phường ", "")
                            .Replace ("xã ", "").Trim ();

                        el = listLi.FirstOrDefault (item => {
                            var itext = item.Text.ToLower ().Trim ();
                            return itext == ntext;

                        });
                        if (el == null) {
                            el = listLi.FirstOrDefault (item => {
                                var itext = item.Text.ToLower ().Trim ();
                                return itext.IndexOf (ntext) >= 0;

                            });
                        }
                    }

                }
            } catch (Exception ex) {
                LogSystem.Write ($"SelectLi - {idDrop}: {ex.InnerException.ToString()}");

            }
            if (el != null) {
                try {
                    try {
                        el.Click ();
                    } catch (Exception) {
                        Actions action = new Actions (driver);
                        action.MoveToElement (el).Click ().Perform ();
                    }
                } catch (Exception) {
                    LogSystem.Write ($"SelectLi-{idDrop}: Không tìm thấy");
                    if (listLi.Count >= 2)
                        listLi[1].Click ();
                }

            } else {
                    LogSystem.Write ($"SelectLi-{idDrop}: Không tìm thấy");
                if (listLi.Count >= 2)
                    listLi[1].Click ();
            }

        }
        /// <summary>
        /// SelectOptions: chon combobox thẻ option
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="idOption"></param>
        /// <param name="value"></param>
        public static void SelectOptions (IWebDriver driver, string idOption, object value) {
            var selectElement = new SelectElement (driver.FindElement (By.Id (idOption)));
            try {
                selectElement.SelectByValue (Convert.ToString (value));

            } catch (Exception) {
                selectElement.SelectByIndex (0);
            }
        }

        /// <summary>
        /// UploadImages upload image
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="nameInputUpload"></param>
        /// <param name="strIds"></param>

        public static void UploadImages (IWebDriver driver, string nameInputUpload, string strIds) {
            try {
                string path = Variables.SELENIUM_PATH_UPLOADS.EndsWith ('\\') ? Variables.SELENIUM_PATH_UPLOADS : Variables.SELENIUM_PATH_UPLOADS + '\\';
                string[] filePaths = Directory.GetFiles (path);
                if (filePaths.Length == 0) return;
                List<string> lPath = new List<string> ();
                List<int> lIds = new List<int> ();
                if (string.IsNullOrEmpty (strIds)) {
                    lPath = filePaths.Length <= Variables.SELENIUM_MAX_RAND_UPLOADS ? filePaths.ToList () : filePaths.GetListRandom (Variables.SELENIUM_MAX_RAND_UPLOADS);
                } else {
                    strIds = strIds.Replace (" ", "");
                    lIds = strIds.Split (',').Select (int.Parse).ToList ();
                    foreach (var id in lIds) {
                        path = string.Format (path + "{0}.jpg", id);
                        if (File.Exists (path)) {
                            lPath.Add (path);
                        }
                    }
                }

                if (lPath.Count > 0) {
                    IWebElement element = driver.FindElement (By.Name (nameInputUpload));
                    element.SendKeys (String.Join ("\n ", lPath));
                    //Thread.Sleep(300); // Ngừng lại 
                }
            } catch (Exception) {

            }

        }

        #endregion
        #region SerializeToJSON to String
        public static string SerializeToJSON (object obj) {
            if (obj == null) {
                return string.Empty;
            } else {
                return Newtonsoft.Json.JsonConvert.SerializeObject (obj);
            }
        }

        private static JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        public static string SerializeToJSON (object obj, bool ignoreReferenceLoop = true) {
            JsonSerializerSettings settings = null;
            if (ignoreReferenceLoop) {
                settings = new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            } else {
                settings = DefaultJsonSerializerSettings;
            }

            return SerializeToJSON (obj, settings);
        }

        public static string SerializeToJSON (object obj, JsonSerializerSettings settings) {
            if (obj == null) {
                return string.Empty;
            } else {

                if (settings == null) {
                    settings = DefaultJsonSerializerSettings;
                }

                return JsonConvert.SerializeObject (obj, Formatting.Indented, settings);
            }
        }
        public static void LoadSettings () {
            var builder = new ConfigurationBuilder ()
                .SetBasePath (Environment.CurrentDirectory)
                .AddJsonFile ($"appsettings.{Variables.EnvironmentName}.json", optional : true)
                .AddEnvironmentVariables ();
            Variables.Configuration = builder.Build ();
            var appSettings = Variables.Configuration.GetSection ("AppSettings");
            Variables.SELENIUM_PATH_UPLOADS = appSettings["SELENIUM_PATH_UPLOADS"] ?? "C:\\Images";
            Variables.SELENIUM_MAX_RAND_UPLOADS = int.Parse (appSettings["SELENIUM_MAX_RAND_UPLOADS"] ?? "3");
            LogSystem.Write ($"AppSettings: ${JsonConvert.SerializeObject(Variables.Configuration.GetSection("AppSettings"))}");
        }
        public static List<string> Split (string str, int chunkSize) {
            if (str.Length <= chunkSize) return new List<string> { str };
            return Enumerable.Range (0, str.Length / chunkSize)
                .Select (i => str.Substring (i * chunkSize, chunkSize)).ToList<string> ();
        }

        public static IWebElement FindElement (dynamic web, By by) {
            try {
                return web.FindElement (by);

            } catch (Exception) {

            }
            return null;
        }

        public static string GetText (dynamic web, string attributeName, string def = null) {
            def = def?? string.Empty;
            try {
                return web.GetAttribute (attributeName);

            } catch (Exception) {

            }
            return def;
        }

        public static bool IsDayNow () {
            string strNow = DateTime.Now.ToString ("dd/MM/yyyy");
            var isDayNow = Variables.SELENIUM_STR_YESTERDAY == strNow;
            if (!isDayNow) {
                Variables.SELENIUM_STR_YESTERDAY = strNow;
            }
            return isDayNow;
        }

        public static void ResetNotify () {
            DataMasterHelper.setLinkChecked(new List<string>());
        }

        public static string convertToUnSign(string s) {
            Regex regex = new Regex ("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize (NormalizationForm.FormD).ToString();
            return regex.Replace (temp, String.Empty).Replace ('\u0111', 'd').Replace ('\u0110', 'D').Replace(" ", "-");
        }
        #endregion
    }

}