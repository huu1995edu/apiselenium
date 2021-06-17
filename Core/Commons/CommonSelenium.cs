using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DockerApi
{
    public class CommonSelenium
    {
        public IWebDriver Init()
        {
            IWebDriver driver;
            var chromeOptions = new ChromeOptions();
            var downloadDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            List<string> lOptions = new List<string>();
            lOptions.Add("--incognito"); // chạy trong trình ẩn anh 
            lOptions.Add("--start-maximized");          
            chromeOptions.AddArguments(lOptions);
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var chromeService = ChromeDriverService.CreateDefaultService(path);
            driver = new ChromeDriver(chromeService, chromeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Manage().Window.Maximize();
            
            return driver;
        }
    }
}
