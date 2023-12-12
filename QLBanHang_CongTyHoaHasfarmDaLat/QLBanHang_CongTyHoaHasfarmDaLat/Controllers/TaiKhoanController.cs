using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBanHang_CongTyHoaHasfarmDaLat.Models;
namespace QLBanHang_CongTyHoaHasfarmDaLat.Controllers
{
    public class TaiKhoanController : Controller
    {
        //
        // GET: /TaiKhoan/

        QLShopHoaDataContext ql = new QLShopHoaDataContext();
        public ActionResult TaiKhoan_Dropdown()
        {
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if (tk != null)
            {
                return PartialView(tk);

            }
            else
            {
                return PartialView(null);
            }
        }


        public ActionResult DangNhap()
        {
            ViewBag.ThongBao_DangKy = null;
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection form)
        {
            TempData["ThongBao_DangNhap"] = null;
            string tendn = form["tendn"];
            string mk = form["mk"];
            TaiKhoan taikhoan = ql.TaiKhoans.Where(t => t.Username == tendn && t.Pass == mk).FirstOrDefault();
            if (taikhoan != null)
            {
                if (taikhoan.Quyen == true)
                {
                    if (taikhoan.KhachHangs.FirstOrDefault().HoatDong == true)
                    {
                        TaiKhoan tk = (TaiKhoan)Session["tk"];
                        Session["tk"] = taikhoan;
                        return RedirectToAction("Home", "KhachHang");
                    }
                    else
                    {
                        TempData["ThongBao_DangNhap"] = "Tai Khoan Da Dung Hoat Dong!";
                        return View();
                    }
                }
                else
                {
                    if (taikhoan.NhanViens.FirstOrDefault().HoatDong == true)
                    {
                        TaiKhoan tk = (TaiKhoan)Session["tk"];
                        Session["tk"] = taikhoan;
                        return RedirectToAction("Dashboard", "QuanLy");
                    }
                    else
                    {
                        TempData["ThongBao_DangNhap"] = "Tai Khoan Da Dung Hoat Dong!";
                        return View();
                    }
                }
            }
            else
            {
                TempData["ThongBao_DangNhap"] = "Sai Mat Khau Hoac Ten Dang Nhap!";
                return View();
            }
        }

        public ActionResult DangKy()
        {
            ViewBag.ThongBao_DangKy = TempData["ThongBao_DangKy"];
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection form)
        {
            TempData["ThongBao_DangKy"] = null;
            TaiKhoan ktra = ql.TaiKhoans.Where(t => t.Username == form["tendn"]).FirstOrDefault();
            if (ktra == null)
            {
                //tạo tài khoản bằng username trước
                TaiKhoan taikhoan = new TaiKhoan();
                taikhoan.Username = form["tendn"];
                taikhoan.Pass = form["mk"];
                taikhoan.Quyen = true;
                bool trungUsername = ql.KhachHangs.Any(t => t.Username == form["tendn"]);
                bool trungSoDienThoai = ql.KhachHangs.Any(t => t.SoDienThoai == form["sdt"]);
                bool trungEmail = ql.KhachHangs.Any(t => t.Email == form["email"]);
                if (!trungUsername && !trungSoDienThoai && !trungEmail)
                {
                    //thêm khách hàng bằng user trong tài khoản
                    KhachHang khach = new KhachHang();
                    int stt = 1;
                    string makh = "KH00" + (ql.KhachHangs.Count() + stt);
                    KhachHang kiemtra = ql.KhachHangs.Where(kt => kt.MaKhachHang == makh).FirstOrDefault();
                    while (kiemtra != null)
                    {
                        stt++;
                        makh = "KH00" + (ql.KhachHangs.Count() + stt);
                        kiemtra = ql.KhachHangs.Where(kt => kt.MaKhachHang == makh).FirstOrDefault();
                    }
                    khach.MaKhachHang = makh;
                    khach.TenKhachHang = form["hoten"];
                    khach.Email = form["email"];
                    khach.DiaChi = form["diachi"];
                    khach.SoDienThoai = form["sdt"];
                    khach.Username = form["tendn"];
                    khach.HoatDong = true;
                    ql.TaiKhoans.InsertOnSubmit(taikhoan);
                    ql.SubmitChanges();
                    //thêm khách hàng sau tài khoản
                    ql.KhachHangs.InsertOnSubmit(khach);
                    ql.SubmitChanges();
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }
                else
                {
                    string trungTruong = "";
                    if (trungUsername)
                        trungTruong += "Ten Dang Nhap ";
                    if (trungSoDienThoai)
                        trungTruong += "So Dien Thoai ";
                    if (trungEmail)
                        trungTruong += "Email ";
                    TempData["ThongBao_DangKy"] = trungTruong + "da ton tai!";
                    return View();
                }
            }
            else
            {
                TempData["ThongBao_DangKy"] = "Ten Dang Nhap Da Ton Tai! Vui Long Su Dung Ten Khac.";
                return View();

            }
        }


        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("DangNhap", "TaiKhoan");
        }


    }
}
