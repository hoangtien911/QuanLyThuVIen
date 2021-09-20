using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_NguoiDungController : Controller
    {
        // GET: Admin/ql_NguoiDung
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult ListUser(string check)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            Dictionary<string, User> data = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Body.ToString());
            var list = new List<User>();
            foreach (var item in data)
            {
                User user = new User();
                user.id = item.Value.id;
                user.username = item.Value.username;
                user.email = item.Value.email;
                user.password = item.Value.password;
                user.fullName = item.Value.fullName;
                user.dateOfBirth = item.Value.dateOfBirth;
                user.gender = item.Value.gender;
                user.adress = item.Value.adress;
                user.phone = item.Value.phone;
                user.dateOfRegist = item.Value.dateOfRegist;
                user.avatar = item.Value.avatar;
                user.status = item.Value.status;
                list.Add(user);
            }
            if (check != null)
            {
                if (check.Equals("1"))
                {
                    ViewBag.MsEdit = "Sửa thông tin người dùng thành công!";
                    check = null;
                }
                else
                {
                    ViewBag.MsDelete = check;
                    check = null;
                }
            }
            return View(list);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv THÊM MỚI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Create()
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            return View();
        }
        [HttpPost]
        public ActionResult Create(User user)
        {
            if ((user.username != null || user.password != null) && user.password != null)
            {
                client = new FireSharp.FirebaseClient(config);
                PushResponse response = client.Push("User/", user);
                user.id = response.Result.name;
                client.Set("User/" + user.id, user);
                ViewBag.MsCreate = "Thêm mới người dùng thành công!";

                Favorite favorite = new Favorite();
                favorite.userID = user.id;
                client.Set("Favorite/" + user.id, favorite);
                return View();
            }
            else
            {
                ViewBag.MsCreate = "Đã có lỗi xảy ra. Thêm mới người dùng thất bại!";
                return View();
            }           
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv SỬA vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Edit(string id)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User/" + id);
            User data = JsonConvert.DeserializeObject<User>(response.Body);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(User user)
        {
            if ((user.username != null || user.password != null) && user.password != null)
            {
                client = new FireSharp.FirebaseClient(config);
                client.Set("User/" + user.id, user);
                return RedirectToAction("ListUser", new { @check = 1 });
            }
            else
            {
                ViewBag.MsEdit = "Đã có lỗi xảy ra. Sửa thông tin người dùng thất bại!";
                return View();
            }
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv XOÁ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult Delete(string id)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("User/" + id);
                User data = JsonConvert.DeserializeObject<User>(response.Body);
                client.Delete("User/" + id);
                return RedirectToAction("ListUser", new { @check = data.username });
            }
            catch
            {
                ViewBag.MsDelete = "Đã có lỗi xảy ra. Xoá người dùng thất bại!";
                return RedirectToAction("ListUser");
            }
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Detail(string id)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User/" + id);
            User data = JsonConvert.DeserializeObject<User>(response.Body);
            return View(data);
        }      
    }
}