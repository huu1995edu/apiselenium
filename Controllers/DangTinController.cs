﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerApi.Core.Commons.ProcessDangTin;
using DockerApi.Core.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Response;

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
                cusRes.StrResult = new ProcessDangTin ().dangTin (value);
                cusRes.Message = Messages.SCS_001;

            } catch (Exception ex) {

                cusRes.SetException (ex);

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
                List<string> accounts = value.GetValue("accounts")?.Values<string>()?.ToList<String>();
                string str_allow_any = value.GetValue("allow_any")?.ToString();
                if (accounts == null && str_allow_any == null)
                {
                    cusRes.SetException(new Exception($"Bạn chưa nhập thông tin: List<string> accounts || bool allow_any"));

                }
                else
                {
                    Variables.SELENIUM_ACCOUNTS = accounts ?? Variables.SELENIUM_ACCOUNTS;
                    Variables.SELENIUM_ACCOUNTS = Variables.SELENIUM_ACCOUNTS.Select(x => x.ToLower()).ToList();
                    Variables.SELENIUM_ALLOW_ANY_ACCOUNT = str_allow_any!=null? bool.Parse(str_allow_any) : Variables.SELENIUM_ALLOW_ANY_ACCOUNT;
                    cusRes.IntResult = 1;
                }                

            } catch (System.Exception ex) {

                cusRes.SetException (ex);
            }
            return Ok (cusRes);

        }
        [HttpPost ("[action]")]
        public IActionResult CheckLinks ([FromBody] JObject value) {
            CustomResult cusRes = new CustomResult ();

            try {
                List<string> links = value.GetValue("links")?.Values<string>()?.ToList<String>();
                int top = value.GetValue("top")!=null ? int.Parse(value.GetValue("top").ToString()): 20;

                new ProcessDangTin ().checkLinks (links, top);
                cusRes.StrResult = "Vui lòng chờ thông báo ở Telegram";
                cusRes.IntResult = 1;
            } catch (Exception ex) {

                cusRes.SetException (ex);

            }
            return Ok (cusRes);

        }

        [HttpPost("[action]")]
        public IActionResult ResetNotify()
        {
            CustomResult cusRes = new CustomResult();

            try
            {
                CommonMethods.ResetNotify();
                cusRes.StrResult = "Đã làm mới thông báo";
                cusRes.IntResult = 1;
            }
            catch (Exception ex)
            {

                cusRes.SetException(ex);

            }
            return Ok(cusRes);

        }

    }
}