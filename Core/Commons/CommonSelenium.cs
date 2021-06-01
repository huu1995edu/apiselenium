using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
            List<string> lOptions = new List<string>();
            lOptions.Add("--incognito"); // chạy trong trình ẩn anh 
            lOptions.Add("--start-maximized");          
            chromeOptions.AddArguments(lOptions);
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var chromeService = ChromeDriverService.CreateDefaultService(path);
            driver = new ChromeDriver(chromeService, chromeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            driver.Manage().Window.Maximize();
            ((IJavaScriptExecutor)driver).ExecuteScript("window.resizeTo(1024, 768);");
            return driver;
        }
    }
}
