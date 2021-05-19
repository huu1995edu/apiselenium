using DockerApi.Core.Entitys;
using GoogleMaps.LocationServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;

namespace DockerApi.Core.Commons.ProcessDangTin
{
    public class ProcessDangTin_BDS
    {
        string pathDangTin = "https://batdongsan.com.vn/dang-tin-rao-vat-ban-nha-dat";
        public void dangTin(TinDang tinDang)
        {
            IWebDriver driver;
            var path = String.Empty;
            try
            {
                var chromeOptions = new ChromeOptions();
                List<string> lOptions = new List<string>();
                lOptions.Add("--incognito"); // chạy trong trình ẩn anh           
                chromeOptions.AddArguments(lOptions);
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var chromeService = ChromeDriverService.CreateDefaultService(path);
                driver = new ChromeDriver(chromeService, chromeOptions);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver.Manage().Window.Maximize();
            }
            catch (System.Exception ex)
            {

                throw new Exception(@"Error - ChromeDriver: " + ex.Message + " - Path: " + path);
            }

            try
            {

                //B1 Login
                login(driver, tinDang);
                //B2 Đăng tin
                driver.Navigate().GoToUrl(pathDangTin);
                CommonMethods.SetInput(driver, "txtProductTitle20180807", tinDang.TieuDe);
                Thread.Sleep(500);
                var hinhThuc = tinDang.HinhThuc > 0 ? tinDang.HinhThuc : 38;
                var loai = tinDang.Loai > 0 ? tinDang.HinhThuc : 283;
                CommonMethods.SelectLi(driver, "divProductType", hinhThuc);
                Thread.Sleep(100);
                CommonMethods.SelectLi(driver, "divProductCate", loai);
                Thread.Sleep(100);
                CommonMethods.SelectLi(driver, "divCity", tinDang.TinhThanh, tinDang.TenTinhThanh);
                Thread.Sleep(100);
                CommonMethods.SelectLi(driver, "divDistrict", tinDang.QuanHuyen, tinDang.TenQuanHuyen);
                Thread.Sleep(100);
                CommonMethods.SelectLi(driver, "divWard", tinDang.PhuongXa, tinDang.TenPhuongXa);
                CommonMethods.SetInput(driver, "txtArea", tinDang.DienTich);
                CommonMethods.SetInput(driver, "txtPrice", tinDang.Gia);
                CommonMethods.SelectOptions(driver, "ddlPriceType", tinDang.DonViTinh == 1 ? 7 : 1);//set đơn vị của giá               
                CommonMethods.SetInput(driver, "txtDescription", tinDang.MoTa);
                CommonMethods.SetInput(driver, "txtWidth", tinDang.MatTien);
                CommonMethods.SetInput(driver, "txtLandWidth", tinDang.DuongVao);
                CommonMethods.SelectOptions(driver, "ddlHomeDirection", tinDang.HuongNha);
                CommonMethods.SelectOptions(driver, "ddlHomeDirection", tinDang.HuongNha);
                CommonMethods.SetInput(driver, "txtLegality", tinDang.ThongTinPhapLy);
                tinDang.DiaChi = String.IsNullOrEmpty(tinDang.DiaChi) ? driver.FindElement(By.Id("txtAddress")).GetAttribute("value") : tinDang.DiaChi;
                CommonMethods.SetInput(driver, "txtAddress", tinDang.DiaChi);
                //B3: upload load hình
                CommonMethods.UploadImages(driver, "file", tinDang.ListHinhAnh);
                //B4: set maps
                CommonMethods.SetInput(driver, "txtBrName", tinDang.TenLienHe);
                CommonMethods.SetInput(driver, "txtBrAddress", tinDang.DiaChiLienHe);
                CommonMethods.SetInput(driver, "txtBrEmail", tinDang.EmailLienHe);
                CommonMethods.SelectLi(driver, "divBrMobile", tinDang.DienThoaiLienHe);
                if (tinDang.TuNgay != null && tinDang.TuNgay != DateTime.MinValue)
                {
                    var tuNgay = tinDang.TuNgay.ToString("dd/MM//yy");
                    CommonMethods.SetInput(driver, "txtStartDate", tinDang.TuNgay);

                }
                if (tinDang.DenNgay != null && tinDang.DenNgay != DateTime.MinValue)
                {
                    var tuNgay = tinDang.DenNgay.ToString("dd/MM//yy");
                    CommonMethods.SetInput(driver, "txtEndDate", tinDang.DenNgay);

                }
                var error = getError(driver) ?? "Bạn nhập mã an toàn không hợp lệ";
                while (error != null && error == "Bạn nhập mã an toàn không hợp lệ")
                {
                    string strResult = CommonMethods.ReadRecaptcha(driver, "img_CAPTCHA_RESULT_314", "reloadCaptcha");
                    driver.FindElement(By.Id("secode")).SendKeys(strResult);
                    driver.FindElement(By.Name("ctl00$MainContent$_userPage$ctl00$btnSave")).Click();
                    //Bắt lỗi error lần 
                    error = getError(driver);
                }
                if (!String.IsNullOrEmpty(error))
                {
                    throw new Exception("error - ProcessDangTin_BDS: " + error);

                }

            }
            catch (Exception ex)
            {
                driver.Close();
                throw new Exception("errorCatch - ProcessDangTin_BDS: " + ex.Message);

            }
            driver.Close();


        }
        public void login(IWebDriver driver, TinDang tinDang)
        {
            string pathLogin = "https://batdongsan.com.vn/trang-dang-nhap";
            //Login
            driver.Navigate().GoToUrl(pathLogin);
            driver.FindElement(By.Id("MainContent__login_LoginUser_UserName")).SendKeys(tinDang.TenDangNhap);
            driver.FindElement(By.Id("MainContent__login_LoginUser_Password")).SendKeys(tinDang.MatKhau + Keys.Enter);
            if (driver.Url == pathLogin)
            {
                var login_err_msgs = driver.FindElements(By.ClassName("login-err-msg"));
                var loginerror = driver.FindElement(By.ClassName("loginerror"));
                if (loginerror != null && loginerror.Displayed)
                {
                    throw new Exception(loginerror.Text);
                }
                if (login_err_msgs != null)
                {
                    foreach (var item in login_err_msgs)
                    {
                        if (item.Displayed)
                        {
                            throw new Exception(item.Text);
                        }
                    }
                }
            }



        }
        public static IWebElement GetParent(IWebElement e)
        {
            return e.FindElement(By.XPath(".."));
        }
        public string getError(IWebDriver driver)
        {
            try
            {
                if (driver.Url == pathDangTin)
                {
                    var login_err_msgs = driver.FindElements(By.ClassName("errorMessage"));
                    foreach (var item in login_err_msgs)
                    {
                        if (item.Displayed)
                        {
                            try
                            {
                                var lbl = GetParent(GetParent(item)).FindElement(By.TagName("label"));
                                if (lbl != null) return String.Format("{0} {1}", lbl.Text, item.Text);
                            }
                            catch (Exception)
                            {

                            }

                            return item.Text;
                        }
                    }
                    var login_err_msg = driver.FindElement(By.Id("MainContent__userPage_ctl00_lblServerErrorMsg"));
                    if (login_err_msg.Displayed)
                    {
                        return login_err_msg.Text;
                    }
                }
            }
            catch (Exception)
            {

            }


            return null;
        }

    }
}
