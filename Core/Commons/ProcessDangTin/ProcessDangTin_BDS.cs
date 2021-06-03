using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DockerApi.Core.Entitys;
using GoogleMaps.LocationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Tesseract;

namespace DockerApi.Core.Commons.ProcessDangTin {
    public class ProcessDangTin_BDS {
        IWebDriver driver = new CommonSelenium ().Init ();
        string pathDangTin = "https://batdongsan.com.vn/dang-tin-rao-vat-ban-nha-dat";
        public string dangTin (TinDang tinDang) {

            string link = String.Empty;
            try {
                //B1 Login
                login (driver, tinDang.TenDangNhap, tinDang.MatKhau);
                //B2 Đăng tin
                driver.Navigate ().GoToUrl (pathDangTin);
                Thread.Sleep (2000);

                CommonMethods.SetInput (driver, "txtProductTitle20180807", tinDang.TieuDe);
                var hinhThuc = tinDang.HinhThuc > 0 ? tinDang.HinhThuc : 38;
                var loai = tinDang.Loai > 0 ? tinDang.HinhThuc : 283;
                CommonMethods.SelectLi (driver, "divProductType", hinhThuc);
                Thread.Sleep (300);
                CommonMethods.RemovePopupChat (driver);
                CommonMethods.SelectLi (driver, "divProductCate", loai);
                Thread.Sleep (300);
                CommonMethods.SelectLi (driver, "divCity", tinDang.TinhThanh, tinDang.TenTinhThanh);
                Thread.Sleep (300);
                CommonMethods.RemovePopupChat (driver);
                CommonMethods.SelectLi (driver, "divDistrict", tinDang.QuanHuyen, tinDang.TenQuanHuyen);
                Thread.Sleep (500);
                CommonMethods.SelectLi (driver, "divWard", tinDang.PhuongXa, tinDang.TenPhuongXa);
                Thread.Sleep (300);

                CommonMethods.SetInput (driver, "txtArea", tinDang.DienTich);
                CommonMethods.SetInput (driver, "txtPrice", (tinDang.Gia / 1000000));
                CommonMethods.SelectOptions (driver, "ddlPriceType", tinDang.DonViTinh == 1 ? 7 : 1); //set đơn vị của giá               
                CommonMethods.SetInput (driver, "txtDescription", tinDang.MoTa);
                CommonMethods.SetInput (driver, "txtWidth", tinDang.MatTien);
                CommonMethods.SetInput (driver, "txtLandWidth", tinDang.DuongVao);
                if (tinDang.HuongNha != null && tinDang.HuongNha > 0) CommonMethods.SelectOptions (driver, "ddlHomeDirection", tinDang.HuongNha);
                CommonMethods.SetInput (driver, "txtLegality", tinDang.ThongTinPhapLy);
                if (!String.IsNullOrEmpty (tinDang.SoNha)) {
                    tinDang.DiaChi = tinDang.SoNha + ", " + driver.FindElement (By.Id ("txtAddress")).GetAttribute ("value");
                    CommonMethods.SetInput (driver, "txtAddress", tinDang.DiaChi);
                }

                //B3: upload load hình
                CommonMethods.UploadImages (driver, "file", tinDang.ListHinhAnh);
                //B4: set maps
                CommonMethods.SetInput (driver, "txtBrName", tinDang.TenLienHe);
                CommonMethods.SetInput (driver, "txtBrAddress", tinDang.DiaChiLienHe);
                CommonMethods.SetInput (driver, "txtBrEmail", tinDang.EmailLienHe);
                CommonMethods.SelectLi (driver, "divBrMobile", tinDang.DienThoaiLienHe);
                if (tinDang.TuNgay != null && tinDang.TuNgay != DateTime.MinValue) {
                    CommonMethods.SetDateTime_BatDongSan (driver, "txtStartDate", tinDang.TuNgay);

                }
                if (tinDang.DenNgay != null && tinDang.DenNgay != DateTime.MinValue) {
                    CommonMethods.SetDateTime_BatDongSan (driver, "txtEndDate", tinDang.DenNgay);

                }
                var error = getError (driver) ?? "Bạn nhập mã an toàn không hợp lệ";
                while (error != null && error == "Bạn nhập mã an toàn không hợp lệ") {
                    try {
                        string strResult = CommonMethods.ReadRecaptcha (driver, "img_CAPTCHA_RESULT_314", "reloadCaptcha");
                        driver.FindElement (By.Id ("secode")).SendKeys (strResult);
                        driver.FindElement (By.Name ("ctl00$MainContent$_userPage$ctl00$btnSave")).Click ();
                        //Bắt lỗi error lần 
                        error = getError (driver);
                        LogSystem.Write (error);
                    } catch (System.Exception ex) {
                        throw new Exception ($"ReadRecaptcha: {ex.Message}");
                    }

                }
                if (!String.IsNullOrEmpty (error)) {
                    throw new Exception (error);

                }

                var eleItem = driver.FindElement (By.Id ("MainContent__userPage_ctl00_rpItems_lnkEdit_0"));
                if (eleItem != null) {
                    link = eleItem.GetAttribute ("href")?.ToString ();
                    link = link.IndexOf ("https://batdongsan.com.vn") >= 0 ? link : $"https://batdongsan.com.vn{link}";
                }

            } catch (Exception ex) {
                driver.Quit ();
                throw new Exception (ex.Message);

            }
            driver.Quit ();

            return link;

        }
        public void login (IWebDriver driver, string tenDangNhap, string matKhau) {
            try {
                string pathLogin = "https://batdongsan.com.vn/trang-dang-nhap";
                //Login
                driver.Navigate ().GoToUrl (pathLogin);
                driver.FindElement (By.Id ("MainContent__login_LoginUser_UserName")).SendKeys (tenDangNhap);
                driver.FindElement (By.Id ("MainContent__login_LoginUser_Password")).SendKeys (matKhau + Keys.Enter);
                if (driver.Url == pathLogin) {
                    var loginerror = driver.FindElement (By.ClassName ("loginerror"));
                    if (loginerror != null && loginerror.Displayed && loginerror.Text.Length > 0) {
                        if (loginerror.Text == "Bạn đang sử dụng 2 tài khoản có thông tin giống nhau, vì vậy hãy chọn 1 tài khoản để tiếp tục đăng nhập.") {
                            var etknv = driver.FindElement (By.Id ("MainContent__login_lnkEmployee"));
                            if (etknv != null) {
                                etknv.Click ();
                                return;
                            }

                        }
                        throw new Exception (loginerror.Text);
                    }
                    var login_err_msgs = driver.FindElements (By.ClassName ("login-err-msg"));
                    if (login_err_msgs != null) {
                        foreach (var item in login_err_msgs) {
                            if (item.Displayed && item.Text.Length > 0) {
                                throw new Exception (item.Text);
                            }
                        }
                    }
                }
            } catch (Exception ex) {

                throw;
            }

        }
        public static IWebElement GetParent (IWebElement e) {
            return e.FindElement (By.XPath (".."));
        }
        public string getError (IWebDriver driver) {
            try {
                if (driver.Url == pathDangTin) {
                    var login_err_msgs = driver.FindElements (By.ClassName ("errorMessage"));
                    foreach (var item in login_err_msgs) {
                        if (item.Displayed) {
                            try {
                                var lbl = GetParent (GetParent (item)).FindElement (By.TagName ("label"));
                                if (lbl != null) return String.Format ("{0} {1}", lbl.Text, item.Text);
                            } catch (Exception) {

                            }

                            return item.Text;
                        }
                    }
                    var login_err_msg = driver.FindElement (By.Id ("MainContent__userPage_ctl00_lblServerErrorMsg"));
                    if (login_err_msg.Displayed) {
                        return login_err_msg.Text;
                    }
                }
            } catch (Exception) {

            }

            return null;
        }

        public void checkLinks (List<String> links, int top) {
            JObject ob = new JObject ();
            String mess = String.Empty;
            mess += $"DANH SÁCH TIN MỚI - TOP {top}%0A";
            if (!CommonMethods.IsDayNow ()) {
                CommonMethods.ResetNotify ();
            }
            try {
                foreach (var link in links) {

                    try {
                        driver.Navigate ().GoToUrl (link);
                        Thread.Sleep (300);
                        var ewrapplinks = driver.FindElements (By.ClassName ("wrap-plink"));
                        List<String> wrapplinks = ewrapplinks.Select (item => item.GetAttribute ("href")).ToList ();
                        String tenNguoiDang = String.Empty;
                        String tenDangNhap = String.Empty;
                        String gia = String.Empty;
                        String dienTich = String.Empty;
                        int index = 1;
                        var obWrappLink = new JObject ();
                        top = top <= wrapplinks.Count ? top : wrapplinks.Count;
                        LogSystem.Write ($"Variables.SELENIUM_LINKS_CHECKED: {string.Join(",", Variables.SELENIUM_LINKS_CHECKED)}");
                        for (int i = 0; i < top; i++) {
                            var wrapplink = wrapplinks[i];
                            var isExist = Variables.SELENIUM_LINKS_CHECKED.IndexOf (wrapplink) >= 0;
                            if (!isExist) {
                                try {
                                    LogSystem.Write ($"isExist: {isExist} - wrapplink: {wrapplink}");
                                    driver.Navigate ().GoToUrl (wrapplink);
                                    var user = CommonMethods.FindElement (driver, By.ClassName ("user"));
                                    var email = String.Empty;
                                    try {
                                        var elEmail = CommonMethods.FindElement (user, By.Id ("email"));
                                        string linkmail = CommonMethods.GetText (elEmail, "href");
                                        string pattern = "mailto:(.+)\\?subject(.+)";
                                        email = string.IsNullOrEmpty (linkmail) ? string.Empty : Regex.Replace (linkmail, pattern, "$1")?.ToLower ();
                                    } catch (Exception) {

                                    }

                                    var elPhone = CommonMethods.FindElement (user, By.ClassName ("phoneEvent"));
                                    var phone = CommonMethods.GetText (elPhone, "raw");
                                    tenNguoiDang = user == null ? String.Empty : user.FindElement (By.ClassName ("name"))?.Text;
                                    var detail = driver.FindElement (By.ClassName ("short-detail-wrap"));
                                    var detailLi = detail.FindElements (By.TagName ("li"));
                                    if (detailLi != null && detailLi.Count >= 2) {
                                        var elGia = CommonMethods.FindElement (detailLi[0], By.ClassName ("sp2"));
                                        gia = elGia == null ? string.Empty : elGia.Text;
                                        var elDienTich = CommonMethods.FindElement (detailLi[1], By.ClassName ("sp2"));
                                        dienTich = elDienTich == null ? string.Empty : elDienTich.Text;
                                    }

                                    tenDangNhap = string.IsNullOrEmpty (email) ? phone : email;
                                    tenDangNhap = string.IsNullOrEmpty (tenDangNhap) ? String.Empty : tenDangNhap.ToLower ();
                                    int indexEmail = Variables.SELENIUM_ACCOUNTS.IndexOf (tenDangNhap);
                                    if (string.IsNullOrEmpty (tenDangNhap) || indexEmail < 0) {
                                        obWrappLink[wrapplink] = $"{index++}. {tenNguoiDang} - {gia} - {dienTich} - {phone} ";
                                        Variables.SELENIUM_LINKS_CHECKED.Add (wrapplink);
                                    }
                                } catch (Exception) {
                                    obWrappLink[wrapplink] = $"+ Chưa xác định: {wrapplink}";

                                }
                            }

                        }
                        
                        if (obWrappLink.HasValues) {
                            ob[link] = $"Có {obWrappLink.Keys().Count} tin đăng mới.%0A" + string.Join ("%0A", obWrappLink.Values ().ToList ());
                        }
                    } catch (Exception ex) {

                        ob[link] = ex.InnerException.ToString ();
                    }

                }

                if (ob.HasValues) {

                    foreach (var item in ob) {
                        mess += $"{item.Key}: {item.Value}%0A";
                    }
                    List<string> lMessage = CommonMethods.Split (mess, 4000);
                    foreach (var m in lMessage) {
                        CommonMethods.notifycation_tele (m);

                    }

                }
            } catch (System.Exception ex) {
                mess += ex.InnerException.ToString ();
                LogSystem.Write ($"checkLinks: {mess}");
                CommonMethods.notifycation_tele (mess);

            }
            driver.Quit ();

        }

        public JObject getBalanceInfo (Account ac) {
            JObject balanceinfo = new JObject ();
            try {
                login (driver, ac.TenDangNhap, ac.MatKhau);
                driver.Navigate ().GoToUrl ("https://batdongsan.com.vn/trang-ca-nhan/uspg-balanceinfo");
                var ltd = driver.FindElements (By.XPath ("//div[@class='balanceinfo']/table/tbody/tr/td"));
                if (ltd != null && ltd.Count > 2) {
                    for (int i = 0; i < ltd.Count; i = i + 2) {
                        var key = CommonMethods.convertToUnSign (ltd[i].Text.ToLower ());
                        var value = CommonMethods.convertToUnSign (ltd[i + 1].Text);
                        balanceinfo[key] = value;

                    }
                }
            } catch (System.Exception ex) {
                driver.Quit ();
                throw ex;
            }
            driver.Quit ();
            return balanceinfo;

        }
    }
}