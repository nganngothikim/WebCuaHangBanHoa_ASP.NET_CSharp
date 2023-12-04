using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBanHang_CongTyHoaHasfarmDaLat.Models;
using System.IO;
namespace QLBanHang_CongTyHoaHasfarmDaLat.Controllers
{
    public class QuanLyController : Controller
    {
        //
        // GET: /QuanLy/
        QLShopHoaDataContext ql = new QLShopHoaDataContext();

        public ActionResult Dashboard()
        {
            return View();
        }

        //Danh mục Chủ Đề
        public ActionResult ChuDe()
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            ViewBag.ThongBao_XoaChuDe = TempData["ThongBao_XoaChuDe"];
            return View(ds);
        }

        public ActionResult Dropdown_ChuDe()
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            return View(ds);
        }

        public ActionResult ThemChuDe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemChuDe(FormCollection form)
        {
            ViewBag.ThongBao_ChuDe = null;
            ChuDe cd = ql.ChuDes.Where(t => t.TenChuDe == form["ten"]).FirstOrDefault();
            if (cd == null)
            {
                ChuDe chude = new ChuDe();
                int stt = 1;
                string macd = "CD0" + (ql.ChuDes.Count() + stt);
                ChuDe kiemtra = ql.ChuDes.Where(kt => kt.MaChuDe == macd).FirstOrDefault();
                while (kiemtra != null)
                {
                    stt++;
                    macd = "CD0" + (ql.ChuDes.Count() + stt);
                    kiemtra = ql.ChuDes.Where(kt => kt.MaChuDe == macd).FirstOrDefault();
                }
                chude.MaChuDe = macd;
                chude.TenChuDe = form["ten"];
                ql.ChuDes.InsertOnSubmit(chude);
                ql.SubmitChanges();
                return RedirectToAction("ChuDe");
            }
            else
            {
                ViewBag.ThongBao_ChuDe = "Ten Chu De Da Ton Tai!";
                return View();
            }
        }
        public ActionResult SuaChuDe(string id)
        {
            ChuDe cd = ql.ChuDes.Where(t => t.MaChuDe == id).FirstOrDefault();
            return View(cd);
        }
        [HttpPost]
        public ActionResult SuaChuDe(FormCollection form)
        {
            ViewBag.ThongBao_SuaChuDe = null;
            ChuDe cd = ql.ChuDes.Where(t => t.MaChuDe == form["macd"]).FirstOrDefault();
            if (cd != null)
            {
                ChuDe kttrung = ql.ChuDes.Where(kt => kt.TenChuDe == form["tencd"]).FirstOrDefault();
                if (kttrung == null)
                {
                    cd.TenChuDe = form["tencd"];
                    ql.SubmitChanges();
                    return RedirectToAction("ChuDe");
                }
                else
                {
                    ViewBag.ThongBao_SuaChuDe = "Ten Chu De Bi Trung !";
                    return View();
                }
            }
            else
            {
                ViewBag.ThongBao_SuaChuDe = "Khong Tim Thay Chu De Can Sua!";
                return RedirectToAction("ChuDe");
            }
        }

        public ActionResult XoaChuDe(string id)
        {
            ViewBag.ThongBao_XoaChuDe = null;
            ChuDe cd = ql.ChuDes.Where(t => t.MaChuDe == id).FirstOrDefault();
            if (cd != null && !cd.SanPhams.Any())
            {
                ql.ChuDes.DeleteOnSubmit(cd);
                ql.SubmitChanges();
                TempData["ThongBao_XoaChuDe"] = "Xoa Thanh Cong!";
            }
            else
            {
                TempData["ThongBao_XoaChuDe"] = "Khong The Xoa Chu De!";
            }

            // Chuyển hướng sau khi xóa
            return RedirectToAction("ChuDe");
        }
        //Danh mục Sản phẩm
        public ActionResult SanPham()
        {
            List<SanPham> ds = ql.SanPhams.ToList();
            return View(ds);
        }

        public ActionResult ThemSanPham()
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            return View(ds);
        }
        [HttpPost]
        public ActionResult ThemSanPham(FormCollection form, HttpPostedFileBase hinh1, HttpPostedFileBase hinh2, HttpPostedFileBase hinh3, HttpPostedFileBase hinh4)
        {
            ViewBag.ThongBao_ThemSanPham = null;
            // Lấy thông tin từ form
            string tenSanPham = form["tensp"];
            string maChuDe = form["chude"];
            string moTa = form["mota"];
            int giaBan = int.Parse(form["gia"]);
            string donViTinh = form["dvt"];
            bool trangThai = form["tt"] != null && form["tt"].Equals("on"); // Kiểm tra checkbox trạng thái

            // Kiểm tra chủ đề tồn tại
            ChuDe chuDe = ql.ChuDes.Where(t => t.TenChuDe == maChuDe).FirstOrDefault();
            if (chuDe != null)
            {
                SanPham sanPham = new SanPham();
                int stt = 1;
                string masp = "SP00" + (ql.SanPhams.Count() + stt);
                SanPham kiemtra = ql.SanPhams.Where(kt => kt.MaSP == masp).FirstOrDefault();
                while (kiemtra != null)
                {
                    stt++;
                    masp = "SP00" + (ql.SanPhams.Count() + stt);
                    kiemtra = ql.SanPhams.Where(kt => kt.MaSP == masp).FirstOrDefault();
                }
                sanPham.MaSP = masp;
                sanPham.TenSP = tenSanPham;
                sanPham.MaChuDe = chuDe.MaChuDe;
                sanPham.MoTa = moTa;
                sanPham.SoLuongTon = 0;
                sanPham.GiaBan = giaBan;
                sanPham.DonViTinh = donViTinh;
                sanPham.TrangThai = trangThai;
                ql.SanPhams.InsertOnSubmit(sanPham);
                ql.SubmitChanges();
                HinhAnh ha = new HinhAnh();
                ha.MaSP = sanPham.MaSP;
                if (hinh1 != null)
                {
                    hinh1.SaveAs(Server.MapPath("/Image/" + hinh1.FileName));
                    ha.Hinh1 = hinh1.FileName;
                }
                if (hinh2 != null)
                {
                    hinh2.SaveAs(Server.MapPath("/Image/" + hinh2.FileName));
                    ha.Hinh2 = hinh2.FileName;
                }
                if (hinh3 != null)
                {
                    hinh3.SaveAs(Server.MapPath("/Image/" + hinh3.FileName));
                    ha.Hinh3 = hinh3.FileName;
                }
                if (hinh4 != null)
                {
                    hinh4.SaveAs(Server.MapPath("/Image/" + hinh4.FileName));
                    ha.Hinh4 = hinh4.FileName;
                }
                ql.HinhAnhs.InsertOnSubmit(ha);
                ql.SubmitChanges();

                // Chuyển hướng sau khi thêm sản phẩm
                return RedirectToAction("SanPham");
            }
            else
            {
                ViewBag.ThongBao_ThemSanPham = "Chu De Khong Ton Tai!";
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult SuaSanPham(string id)
        {
            SanPham sp = ql.SanPhams.Where(t => t.MaSP == id).FirstOrDefault();
            return View(sp);
        }
        [HttpPost]
        public ActionResult SuaSanPham(FormCollection form)
        {
            ViewBag.ThongBao_SuaSanPham = null;
            SanPham sp = ql.SanPhams.Where(t => t.MaSP == form["masp"]).FirstOrDefault();
            if (sp != null)
            {
                SanPham kttrung = ql.SanPhams.Where(kt => kt.TenSP == form["tensp"]).FirstOrDefault();
                if (kttrung == null)
                {
                    sp.TenSP = form["tensp"];
                    sp.MaChuDe = form["chude"];
                    sp.MoTa = form["mota"];
                    sp.DonViTinh = form["dvt"];
                    sp.TrangThai = form["tt"] != null && form["tt"].Equals("on");
                    try
                    {
                        ql.SubmitChanges();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        // Log chi tiết lỗi để phân tích
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                    return RedirectToAction("SanPham");
                }
                else
                {
                    ViewBag.ThongBao_SuaSanPham = "Ten San Pham Bi Trung !";
                    return View();
                }
            }
            else
            {
                ViewBag.ThongBao_SuaSanPham = "Khong Tim Thay San Pham Can Sua!";
                return RedirectToAction("SanPham");
            }
        }
        //Phiếu Nhập
        public ActionResult PhieuNhap()
        {
            List<PhieuNhap> ds = ql.PhieuNhaps.ToList();
            return View(ds);
        }

        public ActionResult ThemDanhSachNhap()
        {
            List<SanPham> ds = ql.SanPhams.ToList();
            return View(ds);
        }

        [HttpPost]
        public ActionResult ThemChiTietPN(FormCollection form)
        {
            DanhSachNhap dsNhap = Session["dsNhap"] as DanhSachNhap;
            if (dsNhap == null)
            {
                dsNhap = new DanhSachNhap();
            }
            string[] maSPDuocChon = form.GetValues("chon");
            DanhSachNhap dsnhap = new DanhSachNhap();
            if (maSPDuocChon != null)
            {
                foreach (string item in maSPDuocChon)
                {
                    SanPhamNhap sp = new SanPhamNhap(item, 0, 0);
                    dsnhap.Them(sp);
                }
                Session["dsNhap"] = dsnhap;
            }
            return View(dsnhap);
        }

        [HttpPost]
        public ActionResult XuLy_ThemChiTietPN()
        {
            return View();
        }


        public ActionResult ChiTietPhieuNhap()
        {
            return View();
        }

        //Hoá đơn
        public ActionResult HoaDon()
        {
            return View();
        }

        public ActionResult ChiTietHoaDon()
        {
            return View();
        }

        //Nhà cung cấp 
        public ActionResult NhaCungCap()
        {
            return View();
        }

        //Khách Hàng 
        public ActionResult KhachHang()
        {
            return View();
        }

        public ActionResult SuaKhachHang()
        {
            return View();
        }

        //Nhân viên 
        public ActionResult NhanVien()
        {
            List<NhanVien> ds = ql.NhanViens.ToList();
            return View(ds);
        }

    }
}
