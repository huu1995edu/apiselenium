using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerApi
{
    public class Variables
    {
        public enum NguonTinDang : int
        {
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

        public static string SELENIUM_PATH_UPLOADS = "";
        #endregion

    }
}
