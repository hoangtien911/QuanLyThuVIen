using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        /// <summary> Đăng nhập </summary>        
        [HttpGet]
        public ActionResult DangNhap()
        {           
            return View();
        }
        [HttpPost]        
        public ActionResult DangNhap(User user)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            Dictionary<string, User> data = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Body.ToString());
            User loginUser = new User();
            foreach (var item in data)
            {               
                if (item.Value.username.Equals(user.username))
                {
                    loginUser.id = item.Value.id;
                    loginUser.username = item.Value.username;
                    loginUser.email = item.Value.email;
                    loginUser.password = item.Value.password;
                    loginUser.fullName = item.Value.fullName;
                    loginUser.dateOfBirth = item.Value.dateOfBirth;
                    loginUser.gender = item.Value.gender;
                    loginUser.adress = item.Value.adress;
                    loginUser.phone = item.Value.phone;
                    loginUser.dateOfRegist = item.Value.dateOfRegist;
                    loginUser.avatar = item.Value.avatar;
                    loginUser.status = item.Value.status;                    
                    break;
                }
                else
                {
                    ViewBag.thongbao = "Tên tài khoản không tồn tại.";                    
                }
            }
            if (!loginUser.password.Equals(user.password)){
                ViewBag.thongbao = "Mật khẩu không đúng";
                return View();
            }else if (loginUser.password.Equals(user.password))
            {
                if (loginUser.status.Equals("Admin"))
                {
                    Session["AdminSession"] = loginUser.id;
                    Response.Cookies["AdminCookies"]["id"] = loginUser.id;
                    Response.Cookies["AdminCookies"]["name"] = loginUser.fullName;
                    Response.Cookies["AdminCookies"]["avatar"] = loginUser.avatar;
                    return RedirectToAction("ListBooks", "ql_Sach", new { area = "Admin" });
                }
                if(loginUser.status.Equals("User"))
                {
                    Session["UserSession"] = loginUser.id;
                    if (user.remember)
                    {
                        Response.Cookies["UserCookies"]["userid"] = loginUser.id;
                        Response.Cookies["UserCookies"]["username"] = loginUser.username;
                        Response.Cookies["UserCookies"]["password"] = loginUser.password;
                        Response.Cookies["UserCookies"].Expires = DateTime.Now.AddDays(15);
                    }
                    return RedirectToAction("HomePage", "TrangChu");
                }
                
            }
            return View();
        }

        /// <summary> Đăng xuất </summary>   
        [HttpGet]
        public ActionResult DangXuat()
        {
            Session["UserSession"] = null;
            Session["AdminSession"] = null;
            Response.Cookies["UserCookies"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["AdminCookies"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("HomePage","TrangChu");
        }      
        /// <summary> Đăng ký </summary>   
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(User user)
        {
            return View();
        }
    }
}