namespace DockerApi
{
    public class Messages
    {
        public const string ERR_001 = "{0} không tồn tại.";
        public const string ERR_002 = "Vui lòng nhập dữ liệu.";
        public const string ERR_003 = "Vui lòng chọn dữ liệu.";
        public const string ERR_004 = "Không thể kết nối đến server.";
        public const string ERR_005 = "Chưa nhập {0}";
        public const string ERR_006 = "{0} đã tồn tại.";
        public const string ERR_007 = "Production only. Chức năng đã bị chặn ở bản Development.";
        public const string ERR_008 = "{0} không được empty.";
        public const string ERR_Http_BadRequest = "BadRequest";
        public const string ERR_Http_UnAuthorized = "UnAuthorized";
        public const string ERR_Http_Error = "HttpErrorCode:{0}"; // nguyencuongcs 20181230 Có thể split : lấy vị trí thứ 2 để get http error code
        public const string ERR_Http_NoConnection = "NoConnectionCouldBeMade:{0}"; // nguyencuongcs 20181230 Có thể split : lấy vị trí thứ 2 để get domain
        public const string ERR_Not_Read_Recaptch = "Không thể đọc được giá trị recaptcha"; // nguyencuongcs 20181230 Có thể split : lấy vị trí thứ 2 để get domain

        public const string SCS_001 = "Đăng tin thành công. Bạn sẽ nhận email phản hồi sau.";

    }
}