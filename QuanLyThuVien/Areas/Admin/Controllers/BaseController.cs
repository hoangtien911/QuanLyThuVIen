using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Mvc;
using QuanLyThuVien.Areas.Admin.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        protected override void OnActionExecuting(ActionExecutingContext filterContex)
        {
            if(Session["UserSession"] != null)
            {
                if(Data_Users.GetSingleData(Session["UserSession"].ToString()).status == "Admin")
                {
                    User admin = Data_Users.GetSingleData(Session["UserSession"].ToString());
                    Response.Cookies["AdminCookies"]["username"] = admin.username;
                    Response.Cookies["AdminCookies"]["userid"] = admin.id;
                    Response.Cookies["AdminCookies"]["avatar"] = admin.avatar;
                    ViewBag.adminAvatar = admin.avatar;
                    base.OnActionExecuting(filterContex);
                }
                else
                {                   
                    filterContex.Result = 
                        new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new {area = "", controller = "Error", action = "PageNotFound", status = "Bạn không có quyền truy cập vào trang này." }));
                }
            }
            else
            {
                filterContex.Result = 
                    new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new {area = "", controller = "TaiKhoan", action = "DangNhap" }));
            }
        }
    }
}