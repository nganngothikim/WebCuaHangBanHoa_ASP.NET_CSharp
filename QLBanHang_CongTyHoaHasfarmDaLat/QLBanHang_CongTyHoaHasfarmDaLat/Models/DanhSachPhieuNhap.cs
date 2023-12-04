using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBanHang_CongTyHoaHasfarmDaLat.Models
{

    public class SanPhamNhap
    {
        QLShopHoaDataContext ql = new QLShopHoaDataContext();

        public string maSP { get; set; }
        public int soLuong { get; set; }
        public int donGiaNhap { get; set; }
        public string tenSP { get; set; }
        public int thanhTien { get { return soLuong * donGiaNhap; } }

        public SanPhamNhap(string maSP, int soLuong, int donGiaNhap)
        {
            this.maSP = maSP;
            this.soLuong = soLuong;
            this.donGiaNhap = donGiaNhap;
            this.tenSP = ql.SanPhams.Where(t => t.MaSP == maSP).FirstOrDefault().TenSP ?? "";
        }

    }
    public class DanhSachNhap
    {
        public List<SanPhamNhap> dssp;
        public DanhSachNhap()
        {
            dssp = new List<SanPhamNhap>();
        }

        public void Them(SanPhamNhap x)
        {          
            dssp.Add(x);
        }

        public int SoLuongSP()
        {
            return dssp.Count();
        }


        public int TongTien()
        {
            return dssp.Sum(t => t.thanhTien);
        }


        public int Xoa(string maSP)
        {
            SanPhamNhap sp = dssp.Find(n => n.maSP == maSP);
            if (sp != null)
            {
                dssp.Remove(sp);
                return 1;
            }
            return -1;
        }


    }
}