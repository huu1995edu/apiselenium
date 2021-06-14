﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using DockerApi.Core.Commons.ProcessDangTin;
using DockerApi.Core.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Response;
using SevenZipExtractor;

namespace DockerApi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class DangTinController : ControllerBase {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get () {
            return "Bắt đầu nào";
        }
        // GET api/values/5
        [HttpGet ("{id}")]
        public ActionResult<string> Get (int id) {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post ([FromBody] ReqTinDang value) {
            CustomResult cusRes = new CustomResult ();

            try {
                if (string.IsNullOrEmpty (value.Data.Ma) || string.IsNullOrEmpty (value.Data.DiaChi) || string.IsNullOrEmpty (value.Data.TenLoai)) {
                    cusRes.SetException ("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin dự án");
                
                } else {
                    string path = CommonMethods.getPathImages (value.Data.Ma);
                    if (path == string.Empty) {
                        cusRes.SetException ("Dự án không tồn tại thư mục ảnh");

                    } else {
                        cusRes.StrResult = new ProcessDangTin ().dangTin (value);
                        cusRes.Message = Messages.SCS_001;
                        cusRes.IntResult = 1;
                    }

                }

            } catch (Exception ex) {

                cusRes.SetException (ex.Message);

            }
            return Ok (cusRes);

        }

        // PUT api/values/5
        [HttpPut ("{id}")]
        public void Put (int id, [FromBody] string value) { }

        // DELETE api/values/5
        [HttpDelete ("{id}")]
        public void Delete (int id) { }

        [HttpPut ("[action]")]
        public IActionResult Settings ([FromBody] JObject value) {
            CustomResult cusRes = new CustomResult ();
            try {
                List<string> accounts = value.GetValue ("accounts")?.Values<string> ()?.ToList<String> ();
                string str_allow_any = value.GetValue ("allow_any")?.ToString ();
                if (accounts == null && str_allow_any == null) {
                    cusRes.SetException (new Exception ($"Bạn chưa nhập thông tin: List<string> accounts || bool allow_any"));

                } else {
                    var acc = DataMasterHelper.getAccounts ();
                    DataMasterHelper.setAccounts (accounts ?? acc);
                    Variables.SELENIUM_ALLOW_ANY_ACCOUNT = str_allow_any != null? bool.Parse (str_allow_any) : Variables.SELENIUM_ALLOW_ANY_ACCOUNT;
                    cusRes.IntResult = 1;
                }

            } catch (System.Exception ex) {

                cusRes.SetException (ex.InnerException.ToString ());
            }
            return Ok (cusRes);

        }

        [HttpPost ("[action]")]
        public IActionResult CheckLinks ([FromBody] JObject value) {
            CustomResult cusRes = new CustomResult ();

            try {
                List<string> links = value.GetValue ("links")?.Values<string> ()?.ToList<String> ();
                int top = value.GetValue ("top") != null ? int.Parse (value.GetValue ("top").ToString ()) : 20;
                cusRes.DataResult = new List<Object> { new ProcessDangTin ().checkLinks (links, top) };
                cusRes.StrResult = "Vui lòng chờ thông báo ở Telegram";
                cusRes.IntResult = 1;
            } catch (Exception ex) {

                cusRes.SetException (ex);

            }
            return Ok (cusRes);

        }

        [HttpPost ("[action]")]
        public IActionResult ResetNotify () {
            CustomResult cusRes = new CustomResult ();

            try {
                CommonMethods.ResetNotify ();
                cusRes.StrResult = "Đã làm mới thông báo";
                cusRes.IntResult = 1;
            } catch (Exception ex) {

                cusRes.SetException (ex);

            }
            return Ok (cusRes);

        }

        [HttpPost ("[action]")]
        public IActionResult GetBalanceInfo ([FromBody] JObject value) {
            CustomResult cusRes = new CustomResult ();

            try {
                Account ac = new Account ();
                ac.TenDangNhap = value.GetValue ("TenDangNhap").ToString ();
                ac.MatKhau = value.GetValue ("MatKhau").ToString ();
                if (String.IsNullOrEmpty (ac.TenDangNhap) || String.IsNullOrEmpty (ac.MatKhau)) {
                    cusRes.SetException (new Exception ("Tên đăng nhập hoặc mật khẩu không hợp lệ"));
                } else {
                    cusRes.DataResult = new List<Object> { new ProcessDangTin ().getBalanceInfo (ac) };
                    cusRes.IntResult = 1;
                }

            } catch (Exception ex) {

                cusRes.SetException (ex);

            }
            return Ok (cusRes);

        }

        [HttpPost ("[action]")]
        public IActionResult GetStatusLink ([FromBody] JObject value) {
            CustomResult cusRes = new CustomResult ();

            try {
                Account ac = new Account ();
                ac.TenDangNhap = value.GetValue ("TenDangNhap").ToString ();
                ac.MatKhau = value.GetValue ("MatKhau").ToString ();
                String id = value.GetValue ("Id").ToString ();
                if (String.IsNullOrEmpty (ac.TenDangNhap) || String.IsNullOrEmpty (ac.MatKhau)) {
                    cusRes.SetException (new Exception ("Tên đăng nhập hoặc mật khẩu không hợp lệ"));
                } else {
                    if (String.IsNullOrEmpty (id)) {
                        cusRes.SetException (new Exception ("Id tin đăng không hợp lệ"));
                    } else {
                        cusRes.DataResult = new List<Object> { new ProcessDangTin ().getStatusLink (ac, id) };
                        cusRes.IntResult = 1;
                    }

                }

            } catch (Exception ex) {

                cusRes.SetException (ex);

            }
            return Ok (cusRes);

        }

        [HttpPost ("[action]")]
        public IActionResult Recharge ([FromBody] JObject value) {
            CustomResult cusRes = new CustomResult ();

            try {
                Account ac = new Account ();
                ac.TenDangNhap = value.GetValue ("TenDangNhap").ToString ();
                ac.MatKhau = value.GetValue ("MatKhau").ToString ();
                List<List<object>> data = value.GetValue ("Data").ToObject<List<List<object>>> ();

                if (String.IsNullOrEmpty (ac.TenDangNhap) || String.IsNullOrEmpty (ac.MatKhau)) {
                    cusRes.SetException (new Exception ("Tên đăng nhập hoặc mật khẩu không hợp lệ"));
                } else {
                    if (data.Count == 0) {
                        cusRes.SetException (new Exception ("data nạp tiền không hợp lệ"));
                    } else {
                        cusRes.DataResult = new List<Object> { new ProcessDangTin ().recharge (ac, data) };
                        cusRes.IntResult = 1;
                    }

                }

            } catch (Exception ex) {

                cusRes.SetException (ex);

            }
            return Ok (cusRes);

        }

        [HttpPost ("[action]")]
        public IActionResult FileDownloaderDrive ([FromBody] DuAn duAn) {
            CustomResult cusRes = new CustomResult ();
            try {

                if (String.IsNullOrEmpty (duAn.UrlDrive) || String.IsNullOrEmpty (duAn.DiaChi) || String.IsNullOrEmpty (duAn.TenLoai) || String.IsNullOrEmpty (duAn.Ma)) {
                    cusRes.SetException (new Exception ("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin dự án"));
                    CommonMethods.notifycation_tele ($"Đã tải ảnh thất bại T.T:%0ADự án: {duAn.Name ?? duAn.Ma}%0ALỗi: Vui lòng kiểm tra lại thông tin dự án.");
                } else {
                    var fileName = duAn.Ma;
                    fileName = CommonMethods.convertToNameFolder (fileName);
                    string zipPath = Variables.SELENIUM_PATH_UPLOADS + $"\\{fileName}.7z";
                    string extractPath = Variables.SELENIUM_PATH_UPLOADS + $"\\{fileName}";
                    if (Directory.Exists (extractPath)) {
                        Directory.Delete (extractPath, true);

                    }
                    if (!Directory.Exists (extractPath)) {
                        Directory.CreateDirectory (extractPath);
                    }

                    var infoFile = FileDownloader.DownloadFileFromURLToPath (duAn.UrlDrive, zipPath);
                    if (infoFile != null) {
                        using (ArchiveFile archiveFile = new ArchiveFile (zipPath)) {
                            archiveFile.Extract (extractPath); // extract all
                        }

                        if (System.IO.File.Exists (zipPath)) {
                            System.IO.File.Delete (zipPath);
                        }
                        CommonMethods.notifycation_tele ($"Đã tải ảnh thành công :))%0ADự án: {duAn.Name ?? duAn.Ma}");

                    }
                    cusRes.IntResult = 1;
                }

            } catch (Exception ex) {
                CommonMethods.notifycation_tele ($"Đã tải ảnh thất bại T.T:%0ADự án: {duAn.Name ?? duAn.Ma}%0ALỗi: {ex.Message} ");
                cusRes.SetException (ex);

            }
            return Ok (cusRes);
        }

    }
}