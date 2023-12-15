using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBanHang_CongTyHoaHasfarmDaLat.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeFilter : FilterAttribute, IAuthorizationFilter
    {
        private readonly QLShopHoaDataContext ql = new QLShopHoaDataContext();
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var tk = filterContext.HttpContext.Session["tk"] as TaiKhoan;
            if (tk != null)
            {
                bool isAdmin = tk.NhanViens.FirstOrDefault().ChucVu == "Quản lý";

                if (!isAdmin || tk.Quyen == true)
                {
                    // Kiểm tra quyền và chuyển hướng nếu cần thiết
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            else
            {
                // Session không tồn tại (chưa đăng nhập), xử lý tùy thuộc vào yêu cầu của bạn
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthorization_NhanVien(AuthorizationContext filterContext)
        {
            var tk = filterContext.HttpContext.Session["tk"] as TaiKhoan;
            if (tk != null)
            {
                bool isAdmin = tk.NhanViens.FirstOrDefault().ChucVu == "Quản lý";

                if (!isAdmin || tk.Quyen == true)
                {
                    // Kiểm tra quyền và chuyển hướng nếu cần thiết
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            else
            {
                // Session không tồn tại (chưa đăng nhập), xử lý tùy thuộc vào yêu cầu của bạn
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}