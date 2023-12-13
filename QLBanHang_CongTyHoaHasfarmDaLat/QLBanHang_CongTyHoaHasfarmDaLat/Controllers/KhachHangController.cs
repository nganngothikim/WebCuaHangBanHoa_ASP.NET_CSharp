using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBanHang_CongTyHoaHasfarmDaLat.Models;
namespace QLBanHang_CongTyHoaHasfarmDaLat.Controllers
{
    public class KhachHangController : Controller
    {
        QLShopHoaDataContext ql = new QLShopHoaDataContext();


        public ActionResult Home()
        {
            return View();
        }

        //private void SetupPagination(int totalItems, int pageIndex, int pageSize)
        //{
        //    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    ViewBag.PageSize = pageSize;
        //    ViewBag.PageIndex = pageIndex;
        //    ViewBag.TotalPages = totalPages;
        //    ViewBag.TotalItems = totalItems;
        //}

        //public ActionResult SanPham(int pageIndex = 1, int pageSize = 12)
        //{
        //    // Lấy tổng số sản phẩm
        //    int totalItems = ql.SanPhams.Count();

        //    // Tính tổng số trang
        //    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    // Lấy sản phẩm cho trang hiện tại
        //    List<SanPham> ds = ql.SanPhams
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    // Gán thông số phân trang vào ViewBag 
        //    SetupPagination(totalItems, pageIndex, pageSize);

        //    return View(ds);
        //}

        public ActionResult SanPham()
        {
            List<SanPham> ds = ql.SanPhams.ToList();
            return View(ds);
        }

        public ActionResult MenuChuDe()
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            return PartialView(ds);
        }

        public ActionResult LocTheoChuDe(string id)
        {
            List<SanPham> ds = ql.SanPhams.Where(t => t.MaChuDe == id).ToList();
            return View("SanPham", ds);
        }


        public ActionResult ChiTietSanPham(string id)
        {
            SanPham sp = ql.SanPhams.Where(t => t.MaSP == id).FirstOrDefault();
            return View(sp);
        }

        [HttpPost]
        public ActionResult TimKiemTheoTen(FormCollection fc)
        {
            string tensp = fc["search"];
            List<SanPham> ds = ql.SanPhams.Where(t => t.TenSP.Contains(tensp)).ToList();
            if (ds.Count > 0)
            {
                return View("SanPham", ds);
            }
            else
            {
                return View("SanPham", null);
            }
        }

        public ActionResult LocSanPhamTheoGia(string id)
        {
            if (id == "1")
            {
                List<SanPham> ds = ql.SanPhams.Where(t => t.GiaBan < 500000).ToList();
                return View("SanPham", ds);
            }
            else if (id == "2")
            {
                List<SanPham> ds = ql.SanPhams.Where(t => t.GiaBan >= 500000 && t.GiaBan < 1000000).ToList();
                return View("SanPham", ds);
            }
            else if (id == "3")
            {
                List<SanPham> ds = ql.SanPhams.Where(t => t.GiaBan > 1000000).ToList();
                return View("SanPham", ds);
            }
            else
            {
                return View("SanPham", null);
            }
        }

        public ActionResult GioHang()
        {
            List<GioHang> ds = null;
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if(tk != null)
            {
                ds = ql.GioHangs.Where(t => t.MaKhachHang == tk.KhachHangs.FirstOrDefault().MaKhachHang).ToList();
                return View(ds);

            }
            else
            {
                return View(ds);
            }
        }

        public ActionResult XoaGioHang(string id)
        {
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if (tk != null)
            {
                GioHang gh = ql.GioHangs.Where(t => t.MaKhachHang == tk.KhachHangs.FirstOrDefault().MaKhachHang && t.MaSP == id).FirstOrDefault();
                ql.GioHangs.DeleteOnSubmit(gh);
                ql.SubmitChanges();

            }
            return RedirectToAction("GioHang");
        }

        public ActionResult ThemGioHang(string id)
        {
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if (tk != null)
            {
                GioHang gh = ql.GioHangs.Where(t => t.MaKhachHang == tk.KhachHangs.FirstOrDefault().MaKhachHang && t.MaSP == id).FirstOrDefault();
                if (gh == null)
                {
                    GioHang ghnew = new GioHang();
                    ghnew.MaKhachHang = tk.KhachHangs.FirstOrDefault().MaKhachHang;
                    ghnew.MaSP = id;
                    ghnew.SoLuong = 1;
                    ql.GioHangs.InsertOnSubmit(ghnew);
                    ql.SubmitChanges();
                    return RedirectToAction("GioHang");

                }
                else
                {
                    gh.SoLuong += 1;
                    UpdateModel(gh);
                    ql.SubmitChanges();
                    return RedirectToAction("GioHang");
                }
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");

            }
        }

        [HttpPost]
        public JsonResult CapNhatSoLuongGH(int soluong, string masp)
        {
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if (tk != null)
            {
                //int dangnhap = 1;
                GioHang gh = ql.GioHangs.Where(t => t.MaKhachHang == "KH001" && t.MaSP == masp).FirstOrDefault();
                if (gh != null)
                {
                    gh.SoLuong = soluong;
                    UpdateModel(gh);
                    ql.SubmitChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });

        }

        public ActionResult XacNhanThongTinDatHang()
        {
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            List<GioHang> ds = ql.GioHangs.Where(t => t.MaKhachHang == tk.KhachHangs.FirstOrDefault().MaKhachHang).ToList();
            return View(ds);
        }
        [HttpPost]
        public ActionResult XacNhanThongTinDatHang(FormCollection form)
        {
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if (tk != null)
            {
                ViewBag.TB_GioHang = null;
                int? Tongtien = 0;
                List<GioHang> gio = ql.GioHangs.Where(t => t.MaKhachHang == tk.KhachHangs.FirstOrDefault().MaKhachHang).ToList();
                if (gio.Count <= 0)
                {
                    ViewBag.TB_GioHang = "Hay Them Gio Hang Truoc Khi Thanh Toan!";
                    return View("GioHang", null);
                }
                else
                {
                    Tongtien = gio.Sum(t => t.SanPham.GiaBan.Value * t.SoLuong) ?? 0;
                }
                HoaDon hd = new HoaDon();
                int stt = 1;
                string sohd = "HD00" + (ql.HoaDons.Count() + stt);
                HoaDon kiemtra = ql.HoaDons.Where(kt => kt.SoHoaDon == sohd).FirstOrDefault();
                while (kiemtra != null)
                {
                    stt++;
                    sohd = "HD00" + (ql.HoaDons.Count() + stt);
                    kiemtra = ql.HoaDons.Where(kt => kt.SoHoaDon == sohd).FirstOrDefault();
                }
                hd.SoHoaDon = sohd;
                hd.MaNhanVien = null;
                hd.MaKhachHang = tk.KhachHangs.FirstOrDefault().MaKhachHang;
                hd.NgayLap = DateTime.Now;
                hd.TongTien = Tongtien;
                hd.PhuongThucThanhToan = false;
                ql.HoaDons.InsertOnSubmit(hd);
                ql.SubmitChanges();

                nhapChiTietHoaDon(hd, gio);
                //xóa giỏ hàng khi đã nhập vào hóa đơn
                ql.GioHangs.DeleteAllOnSubmit(gio);
                ql.SubmitChanges();
                //Nhập phiếu giao bằng mã hóa đơn vừa tạo
                GiaoHang kttontai = ql.GiaoHangs.Where(g => g.SoHoaDon == hd.SoHoaDon).FirstOrDefault();
                if (kttontai == null)
                {
                    GiaoHang giao = new GiaoHang();
                    int sttgiao = 1;
                    string magiao = "VD00" + (ql.GiaoHangs.Count() + sttgiao);
                    GiaoHang kiemtragiao = ql.GiaoHangs.Where(kt => kt.MaVanDon == magiao).FirstOrDefault();
                    while (kiemtragiao != null)
                    {
                        sttgiao++;
                        magiao = "VD00" + (ql.GiaoHangs.Count() + sttgiao);
                        kiemtragiao = ql.GiaoHangs.Where(kt => kt.MaVanDon == magiao).FirstOrDefault();
                    }
                    giao.MaVanDon = magiao;
                    giao.SoHoaDon = hd.SoHoaDon;
                    giao.NgayGiao = null;
                    giao.TrangThai = false;
                    giao.DiaChiGiaoHang = form["diachi"];
                    giao.SoDienThoai = form["sdt"];
                    ql.GiaoHangs.InsertOnSubmit(giao);
                    ql.SubmitChanges();
                    return View("DatHangThanhCong");
                }
                else
                {
                    ViewBag.TB_GioHang = "Hoa Don Da Duoc Giao!";
                    return View("GioHang", null);
                }
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }
        public int nhapChiTietHoaDon(HoaDon hd, List<GioHang> gio)//thêm chi tiết hóa đơn cùng mã hóa đơn
        {
            if (hd != null)
            {
                foreach (GioHang i in gio)
                {
                    ChiTietHoaDon ct = new ChiTietHoaDon();
                    ct.SoHoaDon = hd.SoHoaDon;
                    ct.MaSP = i.MaSP;
                    ct.SoLuongBan = i.SoLuong;
                    ct.GiaBan = i.SanPham.GiaBan;
                    ct.ThanhTien = (i.SanPham.GiaBan * i.SoLuong);
                    ql.ChiTietHoaDons.InsertOnSubmit(ct);
                    ql.SubmitChanges();
                    //SanPham sp = ql.SanPhams.Where(t => t.MaSP == i.MaSP).FirstOrDefault();
                    //if (sp != null)
                    //{
                    //    // Trừ số lượng của chi tiết kích thước
                    //    sp.SoLuongTon -= i.SoLuong;
                    //    ql.SubmitChanges();
                    //}
                }
                return 1;
            }
            return -1;
        }

        public ActionResult DatHangThanhCong()
        {
            return View();
        }


    }
}
