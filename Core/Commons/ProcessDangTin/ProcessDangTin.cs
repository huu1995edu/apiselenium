using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerApi.Core.Entitys;
using Newtonsoft.Json.Linq;

namespace DockerApi.Core.Commons.ProcessDangTin {
    public class ProcessDangTin {
        public string dangTin (ReqTinDang reqTinDang) {
            var indexaccount = DataMasterHelper.getAccounts().IndexOf(reqTinDang.Data.TenDangNhap.ToLower());
            if (indexaccount < 0) {
                if (!Variables.SELENIUM_ALLOW_ANY_ACCOUNT) {
                    string er = $"Tài khoản {reqTinDang.Data.TenDangNhap} không thể đăng tin do không nằm trong danh sách tài khoản cho phép";
                    CommonMethods.notifycation_tele(er);
                    throw new Exception (er);

                } else {
                    CommonMethods.notifycation_tele ($"Tài khoản {reqTinDang.Data.TenDangNhap} không nằm trong danh sách cho phép đang thực hiện đăng tin");

                }
            }
            var error = "Không tìm thấy nguồn phù hợp";
            if (reqTinDang.Sources.ToString ().IndexOf (Variables.NguonTinDang.BatDongSan.ToString ()) >= 0) {
                error = null;
                return new ProcessDangTin_BDS ().dangTin (reqTinDang.Data);
            }
            if (!String.IsNullOrEmpty (error)) {
                throw new Exception (error);
            }
            return "";

        }

        public JObject checkLinks (List<String> links, int top) {
            return new ProcessDangTin_BDS ().checkLinks (links, top);
        }

        public JObject getBalanceInfo (Account ac) {
            return new ProcessDangTin_BDS ().getBalanceInfo(ac);
        }

        public JObject getStatusLink (Account ac, string id) {
            return new ProcessDangTin_BDS ().getStatusLink(ac, id);
        }

    }
}