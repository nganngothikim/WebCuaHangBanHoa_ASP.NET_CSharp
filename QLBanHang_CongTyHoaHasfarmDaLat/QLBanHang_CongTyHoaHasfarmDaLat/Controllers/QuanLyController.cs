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
        public ActionResult ThemSanPham(FormCollection form)
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
                sanPham.GiaBan = giaBan;
                sanPham.DonViTinh = donViTinh;
                sanPham.TrangThai = trangThai;

                // Upload hình ảnh và lưu đường dẫn vào CSDL
                // Lưu ý: Cần xử lý lưu hình ảnh vào thư mục và cập nhật đường dẫn trong CSDL
                // Đoạn mã ở đây chỉ mang tính chất minh họa, cần được điều chỉnh theo logic cụ thể của bạn
                for (int i = 1; i <= 4; i++)
                {
                    HttpPostedFileBase file = Request.Files["hinh" + i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string path = Path.Combine(Server.MapPath("~/hinhanh"), fileName);
                        file.SaveAs(path);
                        // Lưu đường dẫn vào đối tượng sanPham
                        switch (i)
                        {
                            case 1:
                                sanPham.HinhAnh.Hinh1 = path;
                                break;
                            case 2:
                                sanPham.HinhAnh.Hinh2 = path;
                                break;
                            case 3:
                                sanPham.HinhAnh.Hinh3 = path;
                                break;
                            case 4:
                                sanPham.HinhAnh.Hinh4 = path;
                                break;
                        }
                    }
                }

                ql.SanPhams.InsertOnSubmit(sanPham);
                ql.SubmitChanges();
                ViewBag.ThongBao_ThemSanPham = "Them San Pham Thanh Cong!";
            }
            else
            {
                ViewBag.ThongBao_ThemSanPham = "Chu De Khong Ton Tai!";
            }

            // Chuyển hướng sau khi thêm sản phẩm
            return RedirectToAction("SanPham");
        }
        public ActionResult SuaSanPham()
        {
            return View();
        }

        //Phiếu Nhập
        public ActionResult PhieuNhap()
        {
            return View();
        }

        public ActionResult ThemDanhSachNhap()
        {
            return View();
        }
        public ActionResult ThemChiTietPN()
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
