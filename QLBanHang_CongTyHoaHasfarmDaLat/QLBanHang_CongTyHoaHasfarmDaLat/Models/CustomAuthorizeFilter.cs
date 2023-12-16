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
                if (tk.Quyen == true)
                {
                    // Kiểm tra quyền và chuyển hướng nếu cần thiết
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
                else
                {
                    string actionName = filterContext.ActionDescriptor.ActionName;
                    // Bạn có thể sử dụng tên controller để thực hiện logic kiểm tra cụ thể nếu cần
                    if (actionName == "NhanVien" || actionName == "ThemNhanVien" || actionName == "SuaNhanVien" || actionName == "XoaNhanVien")
                    {
                        if (tk.NhanViens.FirstOrDefault().ChucVu != "Quản lý")
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                        }
                    }

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