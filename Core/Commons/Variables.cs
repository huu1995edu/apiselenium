using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace DockerApi {
    
    public static class Variables {
        public enum NguonTinDang : int {
            BatDongSan = 1,
            ChotTot = 2
        }
        public static bool EnvironmentIsProduction = false;

        #region "Messages"

        public const string MessageSessionTimeOut = "Phiên làm việc đã hết, bạn vui lòng đăng nhập lại để tiếp tục";
        public const string MessageSessionInvalidRole = "Không có quyền truy cập vào trang này. Bạn vui lòng đăng nhập lại";

        #endregion
        #region "app setting"
        public static string EnvironmentName = "Development";
        public static IConfiguration Configuration;
        public static string SELENIUM_PATH_UPLOADS = "C:\\Images";
        public static int SELENIUM_MAX_RAND_UPLOADS = 3;
        public static List<String> SELENIUM_ACCOUNTS = new List<String> {
            "charlienguyen.bds@gmail.com",
            "jackiehoang.bds@gmail.com",
            "johnnynguyen.bds@gmail.com",
            "thomashuynh.bds@gmail.com",
            "kyduyendang.mg.96@gmail.com",
            "tienminh.Mgnd.93@gmail.com",
            "ngocthao.bds.mg@gmail.com",
            "hieupham.mg.94@gmail.com",
            "dungthanhtran.2808@gmail.com",
            "kien.ngoctran.2808@gmail.com",
            "quoclap.2808@gmail.com",
            "chuongnt.1980@gmail.com",
            "tienchau.2808@gmail.com",
            "legialinh163@gmail.com",
            "phamthaohuong1909@gmail.com",
            "truongyennhi30390@gmail.com",
            "nguyenannhien171@gmail.com",
            "dinhthaotrang0603@gmail.com",
            "tranleminhanh1101@gmail.com",
            "nguyenlephuongnhiii@gmail.com",
            "tranthanhtu247@gmail.com",
            "nguyenbaominhchau1101@gmail.com",
            "trantruclinh396@gmail.com",
            "tranhoai.hc21@gmail.com",
            "tranphuong.hc21@gmail.com",
            "tranhong.hc21@gmail.com",
            "tranhanh.hc21@gmail.com",
            "tranhoa.hc21@gmail.com",
            "trangdang1579@gmail.com",
            "huyenle1298@gmail.com",
            "tranhuynh1278@gmail.com",
            "transen1907@gmail.com",
            "annguyen.bds1202@gmail.com"
        };
        public static bool SELENIUM_ALLOW_ANY_ACCOUNT = false;
        public static string SELENIUM_STR_YESTERDAY = string.Empty;

        #endregion

    }
}