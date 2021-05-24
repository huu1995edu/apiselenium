using DockerApi.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerApi.Core.Commons.ProcessDangTin
{
    public class ProcessDangTin
    {
        public string dangTin(ReqTinDang reqTinDang)
        {
             var error = "Không tìm thấy nguồn phù hợp";
                if (reqTinDang.Sources.ToString().IndexOf(Variables.NguonTinDang.BatDongSan.ToString()) >= 0)
                {
                    error = null;
                    return new ProcessDangTin_BDS().dangTin(reqTinDang.Data);
                }
                if (!String.IsNullOrEmpty(error))
                {
                    throw new Exception(error);
                }
                return "";


        }

    }
}
