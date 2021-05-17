using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerApi.Core.Commons.ProcessDangTin;
using DockerApi.Core.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Response;

namespace DockerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DangTinController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Bắt đầu nào";
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] ReqTinDang value)
        {
            CustomResult cusRes = new CustomResult();
            try
            {
                new ProcessDangTin().dangTin(value);
                cusRes.Message = Messages.SCS_001;

            }
            catch (Exception ex)
            {

                cusRes.SetException(ex);

            }
            return Ok(cusRes);

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}