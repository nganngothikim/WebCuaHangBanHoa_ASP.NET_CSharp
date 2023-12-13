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
        public ActionResult Dropdown_NhaCungCap()
        {
            List<NhaCungCap> ds = ql.NhaCungCaps.ToList();
            return View(ds);
        }
        //Danh mục Chủ Đề--------------------------------
        public ActionResult ChuDe()
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            ViewBag.ThongBao_XoaChuDe = TempData["ThongBao_XoaChuDe"];
            ViewBag.ThongBao_ThemChuDe = TempData["ThongBao_ThemChuDe"];
            ViewBag.ThongBao_SuaChuDe = TempData["ThongBao_SuaChuDe"];
            return View(ds);
        }

        [HttpPost]
        public ActionResult TimKiem_ChuDe(FormCollection form)
        {
            string search = form["search"];
            List<ChuDe> ds = ql.ChuDes.Where(s => s.TenChuDe.Contains(search) || s.MaChuDe.Contains(search)).ToList();
            if (ds.Count <= 0)
            {
                return View("ChuDe", null);
            }
            else
            {
                return View("ChuDe", ds);
            }
        }

        //hỗ trợ view sửa sản phẩm
        public ActionResult Dropdown_ChuDe(string id)
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            ViewBag.ChuDe_Selected = id;
            return View(ds);
        }

        public ActionResult ThemChuDe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemChuDe(FormCollection form)
        {
            TempData["ThongBao_ThemChuDe"] = null;
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
                TempData["ThongBao_ThemChuDe"] = "Them Thanh Cong !";
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
            TempData["ThongBao_SuaChuDe"] = null;
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
                TempData["ThongBao_SuaChuDe"] = "Khong Tim Thay Chu De Can Sua!";
                return RedirectToAction("ChuDe");
            }
        }

        public ActionResult XoaChuDe(string id)
        {
            TempData["ThongBao_XoaChuDe"] = null;
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
        //Hết chủ đề-----------------


        //Sản phẩm-------------------
        public ActionResult SanPham()
        {
            List<SanPham> ds = ql.SanPhams.ToList();
            ViewBag.ThongBao_ThemSanPham = TempData["ThongBao_ThemSanPham"];
            ViewBag.ThongBao_SuaSanPham = TempData["ThongBao_SuaSanPham"];
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
            TempData["ThongBao_ThemSanPham"] = null;
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
                TempData["ThongBao_ThemSanPham"] = "Them San Pham Thanh Cong!";
                return RedirectToAction("SanPham");
            }
            else
            {
                TempData["ThongBao_ThemSanPham"] = "Chu De Khong Ton Tai!";
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
            TempData["ThongBao_SuaSanPham"] = null;
            SanPham sp = ql.SanPhams.Where(t => t.MaSP == form["masp"]).FirstOrDefault();
            if (sp != null)
            {
                SanPham ktTrung = ql.SanPhams
                           .Where(kt => kt.TenSP == form["tensp"] && kt.MaSP != form["masp"])
                           .FirstOrDefault();
                if (ktTrung == null)
                {
                    sp.TenSP = form["tensp"];
                    sp.MaChuDe = form["chude"];
                    sp.MoTa = form["mota"];
                    sp.DonViTinh = form["dvt"];
                    sp.TrangThai = form["tt"] != null && form["tt"].Equals("on");
                    ql.SubmitChanges();
                    TempData["ThongBao_SuaSanPham"] = "Sua San Pham Thanh Cong !";
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
                TempData["ThongBao_SuaSanPham"] = "Khong Tim Thay San Pham Can Sua!";
                return RedirectToAction("SanPham");
            }
        }

        //public ActionResult XoaSanPham(string id)
        //{
        //    TempData["ThongBao_XoaSanPham"] = null;
        //    SanPham sp = ql.SanPhams.Where(t => t.MaSP== id).FirstOrDefault();
        //    if (sp != null)
        //    {
        //        ql.SanPhams.DeleteOnSubmit(sp);
        //        ql.SubmitChanges();
        //        TempData["ThongBao_XoaSanPham"] = "Xoa Thanh Cong!";
        //    }
        //    else
        //    {
        //        TempData["ThongBao_XoaSanPham"] = "Khong The Xoa Chu De!";
        //    }

        //    // Chuyển hướng sau khi xóa
        //    return RedirectToAction("SanPham");
        //}

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
        public ActionResult XuLy_ThemChiTietPN(FormCollection form)
        {
            DanhSachNhap dsNhap = Session["dsNhap"] as DanhSachNhap;
            foreach (SanPhamNhap sp in dsNhap.dssp)
            {
                string maSPKey = string.Format(sp.maSP);
                string donGiaNhapKey = string.Format("donGiaNhap_{0}", sp.maSP);
                string soLuongNhapKey = string.Format("soLuongNhap_{0}", sp.maSP);

                // Lấy giá trị từ form
                string maSPValue = Request.Form[maSPKey];
                string donGiaNhapValue = Request.Form[donGiaNhapKey];
                string soLuongNhapValue = Request.Form[soLuongNhapKey];

                // Kiểm tra sự trùng khớp mã sản phẩm
                if (maSPValue == sp.maSP)
                {
                    // Tiến hành gán đơn giá và số lượng vào đối tượng sp
                    sp.donGiaNhap = Convert.ToInt32(donGiaNhapValue);
                    sp.soLuong = Convert.ToInt32(soLuongNhapValue);
                }
            }

            Session["dsNhap"] = dsNhap;
            TaiKhoan tk = (TaiKhoan)Session["tk"];
            if (tk != null)
            {
                ViewBag.TB_GioHang = null;
                int? Tongtien = 0;
                if (dsNhap.SoLuongSP() <= 0)
                {
                    ViewBag.TB_GioHang = "Hay Them Gio Hang Truoc Khi Thanh Toan!";
                    return View("GioHang", null);
                }
                else
                {
                    Tongtien = dsNhap.TongTien();
                }
                PhieuNhap pn = new PhieuNhap();
                int stt = 1;
                string mapn = "PN00" + (ql.PhieuNhaps.Count() + stt);
                PhieuNhap kiemtra = ql.PhieuNhaps.Where(kt => kt.MaPhieuNhap == mapn).FirstOrDefault();
                while (kiemtra != null)
                {
                    stt++;
                    mapn = "PN00" + (ql.PhieuNhaps.Count() + stt);
                    kiemtra = ql.PhieuNhaps.Where(kt => kt.MaPhieuNhap == mapn).FirstOrDefault();
                }
                pn.MaPhieuNhap = mapn;
                pn.MaNhanVien = tk.NhanViens.FirstOrDefault().MaNhanVien;
                pn.NgayNhap = DateTime.Now;
                pn.TongTien = Tongtien;
                //pn.MaNhaCungCap = form["ncc"];
                pn.MaNhaCungCap = "NCC001";
                ql.PhieuNhaps.InsertOnSubmit(pn);
                ql.SubmitChanges();

                nhapChiTietPhieuNhap(pn, dsNhap);
                if (Session["dsNhap"] != null)
                {
                    // Xóa session với key "dsNhap"
                    Session.Remove("dsNhap");
                }
                return RedirectToAction("PhieuNhap");
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
        }

        public int nhapChiTietPhieuNhap(PhieuNhap pn, DanhSachNhap dsnhap)//thêm chi tiết hóa đơn cùng mã hóa đơn
        {
            if (pn != null)
            {
                foreach (SanPhamNhap i in dsnhap.dssp)
                {
                    ChiTietPhieuNhap ct = new ChiTietPhieuNhap();
                    ct.MaPhieuNhap = pn.MaPhieuNhap;
                    ct.MaSP = i.maSP;
                    ct.SoLuongNhap = i.soLuong;
                    ct.DonGiaNhap = i.donGiaNhap;
                    ct.ThanhTien = i.thanhTien;
                    ql.ChiTietPhieuNhaps.InsertOnSubmit(ct);
                    ql.SubmitChanges();
                    SanPham sp = ql.SanPhams.Where(t => t.MaSP == i.maSP).FirstOrDefault();
                    if (sp != null)
                    {
                        // Cộng số lượng của sản phẩm
                        sp.SoLuongTon -= i.soLuong;
                        ql.SubmitChanges();
                    }
                }
                return 1;
            }
            return -1;
        }


        public ActionResult HuyDanhSachNhap()
        {
            if (Session["dsNhap"] != null)
            {
                // Xóa session với key "dsNhap"
                Session.Remove("dsNhap");
            }
            return RedirectToAction("PhieuNhap");
        }
        public ActionResult ChiTietPhieuNhap(string id)
        {
            List<ChiTietPhieuNhap> ct = ql.ChiTietPhieuNhaps.Where(t => t.MaPhieuNhap == id).ToList();
            ViewBag.maPhieuNhap = id;
            return View(ct);
        }
        //Hết Phiếu Nhập----------------
        //Hoá đơn-----------------------
        public ActionResult HoaDon()
        {
            List<HoaDon> ds = ql.HoaDons.ToList();
            return View(ds);
        }

        public ActionResult ChiTietHoaDon(string id)
        {
            List<ChiTietHoaDon> ct = ql.ChiTietHoaDons.Where(t => t.SoHoaDon == id).ToList();
            ViewBag.maHoaDon = id;
            return View(ct);
        }


        public ActionResult PhieuGiao()
        {
            List<GiaoHang> ds = ql.GiaoHangs.ToList();
            return View(ds);
        }
        public ActionResult DuyetPhieuGiao()
        {
            List<GiaoHang> ds = ql.GiaoHangs.Where(g => g.TrangThai == false).ToList();
            return View(ds);
        }
        [HttpPost]
        public ActionResult DuyetPhieuGiao(FormCollection form)
        {
            ViewBag.ThongBao_DuyetPhieuGiao = null;
            string[] maSPDuocChon = form.GetValues("chon");
            if (maSPDuocChon != null)
            {
                foreach (string item in maSPDuocChon)
                {
                    GiaoHang giao = ql.GiaoHangs.Where(g => g.MaVanDon == item).FirstOrDefault();
                    if (giao != null)
                    {
                        giao.TrangThai = true;
                        giao.NgayGiao = DateTime.Now;
                        List<ChiTietHoaDon> lst = ql.ChiTietHoaDons.Where(l => l.SoHoaDon == giao.SoHoaDon).ToList();
                        if (lst.Count > 0)
                        {
                            foreach (ChiTietHoaDon ct in lst)
                            {
                                SanPham sp = ql.SanPhams.Where(t => t.MaSP == ct.MaSP).FirstOrDefault();
                                if (sp != null)
                                {
                                    // Trừ số lượng của chi tiết kích thước
                                    sp.SoLuongTon -= ct.SoLuongBan;
                                    ql.SubmitChanges();
                                }
                            }
                        }
                    }
                }
                ViewBag.ThongBao_DuyetPhieuGiao = "Cap Nhat Phieu Giao Thanh Cong !";
                return View();
            }
            else
            {
                ViewBag.ThongBao_DuyetPhieuGiao = "Cap Nhat Phieu Giao That Bai!";
                return View();
            }
        }




        //Hết Hóa Đơn----------------------

        //Nhà cung cấp---------------------
        public ActionResult NhaCungCap()
        {
            List<NhaCungCap> ds = ql.NhaCungCaps.ToList();
            return View(ds);
        }
        //Hết Nhà Cung Cấp------------------

        //Khách Hàng------------------------
        public ActionResult KhachHang()
        {
            List<KhachHang> ds = ql.KhachHangs.ToList();
            ViewBag.ThongBao_SuaKhachHang = TempData["ThongBao_SuaKhachHang"];
            return View(ds);
        }

        public ActionResult SuaKhachHang(string id)
        {
            ViewBag.ThongBao_SuaKhachHang = null;
            TempData["ThongBao_SuaKhachHang"] = null;
            KhachHang kh = ql.KhachHangs.Where(n => n.MaKhachHang == id).FirstOrDefault();
            if (kh != null)
            {
                return View(kh);
            }
            else
            {
                TempData["ThongBao_SuaKhachHang"] = "Khong Tim Thay Nhan Vien De Sua !";
                return RedirectToAction("KhachHang");
            }
        }
        [HttpPost]
        public ActionResult SuaKhachHang(FormCollection form)
        {
            TempData["ThongBao_SuaKhachHang"] = null;
            KhachHang kh = ql.KhachHangs.Where(t => t.MaKhachHang == form["makh"]).FirstOrDefault();
            if (kh != null)
            {
                kh.HoatDong = form["tt"] != null && form["tt"].Equals("on");
                ql.SubmitChanges();
                TempData["ThongBao_SuaKhachHang"] = "Sua Khach Hang Thanh Cong !";
                return RedirectToAction("KhachHang");
            }
            else
            {
                TempData["ThongBao_SuaKhachHang"] = "Khong Tim Thay San Pham Can Sua!";
                return RedirectToAction("SanPham");
            }
        }

        //Hết khách hàng--------------
        //Nhân viên--------------------
        public ActionResult NhanVien()
        {
            List<NhanVien> ds = ql.NhanViens.ToList();
            ViewBag.ThongBao_SuaNhanVien = TempData["ThongBao_SuaNhanVien"];
            ViewBag.ThongBao_ThemNhanVien = TempData["ThongBao_ThemNhanVien"];
            ViewBag.ThongBao_XoaNhanVien = TempData["ThongBao_XoaNhanVien"];

            return View(ds);
        }

        public ActionResult ThemNhanVien()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemNhanVien(FormCollection form)
        {
            TempData["ThongBao_ThemNhanVien"] = null;
            TaiKhoan kiemtratt = ql.TaiKhoans.Where(tk => tk.Username == form["username"]).FirstOrDefault();
            if (kiemtratt == null)
            {
                TaiKhoan taikhoan = new TaiKhoan();
                taikhoan.Username = form["username"];
                taikhoan.Pass = form["pass"];
                taikhoan.Quyen = false;
                //thêm nhân viên sau khi thêm tài khoản
                bool trungUsername = ql.NhanViens.Any(t => t.Username == form["username"]);
                bool trungSoDienThoai = ql.NhanViens.Any(t => t.SoDienThoai == form["sdt"]);
                bool trungEmail = ql.NhanViens.Any(t => t.Email == form["email"]);
                if (!trungUsername && !trungSoDienThoai && !trungEmail)
                {
                    NhanVien nhanvien = new NhanVien();
                    int stt = 1;
                    string manv = "NV00" + (ql.NhanViens.Count() + stt);
                    NhanVien kiemtra = ql.NhanViens.Where(kt => kt.MaNhanVien == manv).FirstOrDefault();
                    while (kiemtra != null)
                    {
                        stt++;
                        manv = "NV00" + (ql.NhanViens.Count() + stt);
                        kiemtra = ql.NhanViens.Where(kt => kt.MaNhanVien == manv).FirstOrDefault();
                    }
                    nhanvien.MaNhanVien = manv;
                    nhanvien.TenNhanVien = form["tennv"];
                    nhanvien.DiaChi = form["diachi"];
                    nhanvien.SoDienThoai = form["sdt"];
                    nhanvien.NgaySinh = DateTime.Parse(form["ngaysinh"]);
                    nhanvien.Email = form["email"];
                    nhanvien.Username = form["username"];
                    nhanvien.HoatDong = form["tt"] != null && form["tt"].Equals("on");
                    ql.TaiKhoans.InsertOnSubmit(taikhoan);
                    ql.SubmitChanges();
                    //thêm nhân viên sau tài khoản
                    ql.NhanViens.InsertOnSubmit(nhanvien);
                    ql.SubmitChanges();
                    TempData["ThongBao_ThemNhanVien"] = "Them Thanh Cong !";
                    return RedirectToAction("NhanVien");
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
                    ViewBag.ThongBao_ThemNhanVien = trungTruong + "da ton tai!";
                    return View();
                }
            }
            else
            {
                TempData["ThongBao_ThemNhanVien"] = "Ten Dang Nhap Da Ton Tai!";
                return RedirectToAction("NhanVien");
            }
        }


        public ActionResult SuaNhanVien(string id)
        {
            ViewBag.ThongBao_SuaNhanVien = null;
            TempData["ThongBao_SuaNhanVien"] = null;
            NhanVien nv = ql.NhanViens.Where(n => n.MaNhanVien == id).FirstOrDefault();
            if (nv != null)
            {
                return View(nv);
            }
            else
            {
                TempData["ThongBao_SuaNhanVien"] = "Khong Tim Thay Nhan Vien De Sua !";
                return RedirectToAction("NhanVien");
            }
        }

        [HttpPost]
        public ActionResult SuaNhanVien(FormCollection form)
        {
            TempData["ThongBao_SuaNhanVien"] = null;
            NhanVien nv = ql.NhanViens.Where(t => t.MaNhanVien == form["manv"]).FirstOrDefault();
            if (nv != null)
            {
                bool trungSoDienThoai = ql.NhanViens.Any(t => t.SoDienThoai == form["sdt"]);
                if (!trungSoDienThoai)
                {
                    nv.TenNhanVien = form["tennv"];
                    nv.DiaChi = form["diachi"];
                    nv.SoDienThoai = form["sdt"];
                    nv.HoatDong = form["tt"] != null && form["tt"].Equals("on");
                    ql.SubmitChanges();
                    TempData["ThongBao_SuaNhanVien"] = "Sua Thanh Cong Nhan Vien!";
                    return RedirectToAction("NhanVien");
                }
                else
                {
                    ViewBag.ThongBao_SuaNhanVien = "So Dien Thoai Da Ton Tai!";
                    return View(nv);
                }
            }
            else
            {
                TempData["ThongBao_SuaNhanVien"] = "Khong Tim Thay San Pham Can Sua!";
                return RedirectToAction("NhanVien");
            }
        }

        public ActionResult XoaNhanVien(string id)
        {
            TempData["ThongBao_XoaNhanVien"] = null;
            NhanVien nv = ql.NhanViens.Where(t => t.MaNhanVien == id).FirstOrDefault();
            if (nv != null && !nv.PhieuDats.Any() && !nv.PhieuNhaps.Any())
            {
                TaiKhoan tk = ql.TaiKhoans.Where(k => k.Username == nv.Username).FirstOrDefault();
                ql.NhanViens.DeleteOnSubmit(nv);
                ql.SubmitChanges();
                if (tk != null)
                {
                    ql.TaiKhoans.DeleteOnSubmit(tk);
                    ql.SubmitChanges();
                }
                TempData["ThongBao_XoaNhanVien"] = "Xoa Thanh Cong!";
            }
            else
            {
                TempData["ThongBao_XoaNhanVien"] = "Khong The Xoa Nhan Vien!";
            }

            // Chuyển hướng sau khi xóa
            return RedirectToAction("NhanVien");
        }

    }
}
