using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBanHang_CongTyHoaHasfarmDaLat.Models;
using log4net;
using PagedList;
using System.Configuration;
namespace QLBanHang_CongTyHoaHasfarmDaLat.Controllers
{
    public class KhachHangController : Controller
    {
        QLShopHoaDataContext ql = new QLShopHoaDataContext();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult SanPham(int? page)
        {
            List<SanPham> ds = ql.SanPhams.ToList();
            int pageNumber = (page ?? 1);
            ViewBag.URL = "SanPham";
            return View(ds.ToPagedList(pageNumber, 6));
        }

        public ActionResult MenuChuDe()
        {
            List<ChuDe> ds = ql.ChuDes.ToList();
            return PartialView(ds);
        }

        public ActionResult LocTheoChuDe(string id, int? page)
        {
            List<SanPham> ds = ql.SanPhams.Where(t => t.MaChuDe == id).ToList();
            int pageNumber = (page ?? 1);
            ViewBag.URL = "LocTheoChuDe" + "/" + id;
            return View("SanPham", ds.ToPagedList(pageNumber, 6));
        }


        public ActionResult ChiTietSanPham(string id)
        {
            SanPham sp = ql.SanPhams.Where(t => t.MaSP == id).FirstOrDefault();
            return View(sp);
        }

        public ActionResult TimKiemTheoTen(string tensp, int? page)
        {
            List<SanPham> ds = ql.SanPhams.Where(t => t.TenSP.Contains(tensp)).ToList();
            int pageNumber = page ?? 1;
            ViewBag.URL = "/TimKiemTheoTen";
            ViewBag.Param = tensp;

            if (ds.Count > 0)
            {
                return View("SanPham", ds.ToPagedList(pageNumber, 6));
            }
            else
            {
                return View("SanPham", null);
            }
        }

        public ActionResult LocSanPhamTheoGia(string id, int? page)
        {
            if (id == "1")
            {
                List<SanPham> ds = ql.SanPhams.Where(t => t.GiaBan < 500000).ToList();
                int pageNumber = page ?? 1;
                ViewBag.URL = "LocSanPhamTheoGia" + "/" + id;
                return View("SanPham", ds.ToPagedList(pageNumber, 6));
            }
            else if (id == "2")
            {
                List<SanPham> ds = ql.SanPhams.Where(t => t.GiaBan >= 500000 && t.GiaBan < 1000000).ToList();
                int pageNumber = page ?? 1;
                ViewBag.URL = "LocSanPhamTheoGia" + "/" + id;
                return View("SanPham", ds.ToPagedList(pageNumber, 6));
            }
            else if (id == "3")
            {
                List<SanPham> ds = ql.SanPhams.Where(t => t.GiaBan > 1000000).ToList();
                int pageNumber = page ?? 1;
                ViewBag.URL = "LocSanPhamTheoGia" + "/" + id;
                return View("SanPham", ds.ToPagedList(pageNumber, 6));
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
            if (tk != null)
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
            ViewBag.DiaChi = tk.KhachHangs.FirstOrDefault().DiaChi;
            ViewBag.SDT = tk.KhachHangs.FirstOrDefault().SoDienThoai;
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
                if (form["thanhtoan"] == "online")
                {
                    hd.PhuongThucThanhToan = false;
                }
                else
                {
                    hd.PhuongThucThanhToan = true;
                }
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
                    if (form["thanhtoan"] == "online")
                    {
                        return RedirectToAction("ThanhToanOnline", new { soHoaDon = hd.SoHoaDon, tongTien = hd.TongTien });
                    }
                    else
                    {
                        return View("DatHangThanhCong");
                    }
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

                }
                return 1;
            }
            return -1;
        }

        public ActionResult DatHangThanhCong()
        {
            return View();
        }

        public ActionResult ThanhToanOnline(string soHoaDon, int tongTien)
        {
            Session["HoaDonSession"] = soHoaDon;
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            //Get payment input
            OrderInfo order = new OrderInfo();
            order.OrderId = DateTime.Now.Ticks;
            order.Amount = tongTien;
            order.Status = "0";
            order.CreatedDate = DateTime.Now;

            //Save order to db

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            //vnpay.AddRequestData("SoHD", soHoaDon);

            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            // ... (Các dòng code xử lý radio button và thêm các tham số khác)

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");

            // ... (Các dòng code xử lý radio button và thêm các tham số khác)
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

            // ... (Các dòng code xử lý radio button và thêm các tham số khác)

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            log.InfoFormat("VNPAY URL: {0}", paymentUrl);

            // Redirect tới trang thanh toán
            return Redirect(paymentUrl);
        }

        //[HttpGet]
        public ActionResult vnpay_return()
        {
            ViewBag.Error = null;
            string soHoaDon = Session["HoaDonSession"] as string;
            log.Info("Begin VNPAY Return");

            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string key in Request.QueryString)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, Request.QueryString[key]);
                    }
                }

                // VnPayReturnModel model = new VnPayReturnModel();
                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");

                string vnp_BankTranNo = vnpay.GetResponseData("vnp_BankTranNo") ?? "";


                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                string TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                //string soHD = vnpay.GetResponseData("SoHD");

                string bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        // Thanh toán thành công
                        ThemThongTinThanhToan(vnpayTranId.ToString(), soHoaDon, bankCode, vnp_BankTranNo);
                        Session.Remove("HoaDonSession");
                        ViewBag.Message = "Giao Dich Thanh Cong!";
                        log.Info("Thanh toan thanh cong, OrderId={orderId}, VNPAY TranId={vnpayTranId}");
                    }
                    else
                    {
                        // Thanh toán không thành công
                        ViewBag.Error = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                        log.Info("Thanh toan loi, OrderId={orderId}, VNPAY TranId={vnpayTranId}, ResponseCode={vnp_ResponseCode}");
                    }
                    ViewBag.vnp_BankTranNo = "Mã giao dịch ngân hàng: " + vnp_BankTranNo;
                    ViewBag.VnpayTranNo = "Mã giao dịch tại hệ thống VNPAY: " + vnpayTranId.ToString();
                    ViewBag.Amount = "Số tiền thanh toán (VND): " + vnp_Amount.ToString();
                    ViewBag.BankCode = "Ngân hàng thanh toán: " + bankCode;
                }
                else
                {
                    log.Info("Invalid signature");
                    ViewBag.Error = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            else
            {
                log.Info("No query string data");
                ViewBag.Error = "Không có dữ liệu trả về từ VNPAY";
            }

            return View();
        }

        public bool ThemThongTinThanhToan(string maGDHeThong, string soHD, string maNH, string maGDNganHang)
        {
            HoaDon hd = ql.HoaDons.Where(h => h.SoHoaDon == soHD).FirstOrDefault();
            if (hd != null)
            {
                ThongTinThanhToan kiemtratt = ql.ThongTinThanhToans.Where(tt => tt.SoHoaDon == soHD).FirstOrDefault();
                if(kiemtratt == null)
                {
                    ThongTinThanhToan thanhtoan = new ThongTinThanhToan();
                    thanhtoan.MaGiaoDichHeThong = maGDHeThong;
                    thanhtoan.MaNganHang = maNH;
                    thanhtoan.MaGiaoDichNganHang = maGDNganHang;
                    thanhtoan.SoHoaDon = soHD;
                    ql.ThongTinThanhToans.InsertOnSubmit(thanhtoan);
                    ql.SubmitChanges();
                    return true;
                }
            }
            return false;

        }



    }
}
