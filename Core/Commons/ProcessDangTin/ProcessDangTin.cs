using DockerApi.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerApi.Core.Commons.ProcessDangTin
{
    public class ProcessDangTin
    {
        public void dangTin(ReqTinDang reqTinDang)
        {
            try
            {
                var error = "Không tìm thấy nguồn phù hợp";
                if (reqTinDang.Sources.ToString().IndexOf(Variables.NguonTinDang.BatDongSan.ToString()) >= 0)
                {
                    error = null;
                    new ProcessDangTin_BDS().dangTin(reqTinDang.Data);
                }
                if (!String.IsNullOrEmpty(error))
                {
                    throw new Exception(error);
                }
            }
            catch (System.Exception ex)
            {

                throw new Exception("error - dangTin: " + ex.Message);
            }


        }

    }
}
