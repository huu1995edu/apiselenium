using DockerApi.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerApi.Core.Entitys
{
    public class ReqTinDang
    {
        public Variables.NguonTinDang Sources { get; set; }
        public TinDang Data { get; set; }
    }

    public class ObjectTinDang
    {
        public string Attribute { get; set; }
        public string Text { get; set; }
    }
    public class TinDang
    {
        public string TieuDe { get; set; }
        public int HinhThuc { get; set; }
        public int Loai { get; set; }
        public string TinhThanh { get; set; }
        public string TenTinhThanh { get; set; }
        public int QuanHuyen { get; set; }
        public string TenQuanHuyen { get; set; }
        public int PhuongXa { get; set; }
        public string TenPhuongXa { get; set; }
        public string SoNha { get; set; }
        public string DiaChi { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int DienTich { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
        public string ListHinhAnh { get; set; }
        public int MatTien { get; set; }
        public int DuongVao { get; set; }
        public int HuongNha { get; set; }
        public int SoTang { get; set; }
        public int SoPhongNgu { get; set; }
        public int DonViTinh { get; set; }
        public string SoToilet { get; set; }
        public string NoiThat { get; set; }
        public string ThongTinPhapLy { get; set; }
        public string TenLienHe { get; set; }
        public string DienThoaiLienHe { get; set; }
        public string EmailLienHe { get; set; }
        public string DiaChiLienHe { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
    }
}
