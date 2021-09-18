using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_TacGiaController : Controller
    {
        // GET: Admin/ql_TacGia
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH TÁC GIẢ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult ListAuthor(string check)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Author");
            Dictionary<string, Author> data = JsonConvert.DeserializeObject<Dictionary<string, Author>>(response.Body.ToString());
            var list = new List<Author>();
            foreach (var item in data)
            {
                Author author = new Author();
                author.id = item.Value.id;
                author.email = item.Value.email;
                author.website = item.Value.website;
                author.name = item.Value.name;
                author.note = item.Value.note;
                author.avatar = item.Value.avatar;
                list.Add(author);
            }
            if (check != null)
            {
                if (check.Equals("1"))
                {
                    ViewBag.MsEdit = "Sửa thông tin tác giả thành công!";
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
        public ActionResult Create(Author author)
        {
            if (author.name != null)
            {
                client = new FireSharp.FirebaseClient(config);
                PushResponse response = client.Push("Author/", author);
                author.id = response.Result.name;
                client.Set("Author/" + author.id, author);
                ViewBag.MsCreate = "Thêm mới tác giả thành công!";
                return View();
            }
            else
            {
                ViewBag.MsCreate = "Đã có lỗi xảy ra. Thêm mới tác giả thất bại!";
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
            FirebaseResponse response = client.Get("Author/" + id);
            Author data = JsonConvert.DeserializeObject<Author>(response.Body);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Author author)
        {
            if (author.name != null)
            {
                client = new FireSharp.FirebaseClient(config);
                client.Set("Author/" + author.id, author);
                return RedirectToAction("ListAuthor", new { @check = 1 });
            }
            else
            {
                ViewBag.MsEdit = "Đã có lỗi xảy ra. Sửa thông tin tác giả thất bại!";
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
                FirebaseResponse response = client.Get("Author/" + id);
                Author data = JsonConvert.DeserializeObject<Author>(response.Body);
                client.Delete("Author/" + id);
                return RedirectToAction("ListAuthor", new { @check = data.name });
            }
            catch
            {
                ViewBag.MsDelete = "Đã có lỗi xảy ra. Xoá tác giả thất bại!";
                return RedirectToAction("ListAuthor");
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
            FirebaseResponse response = client.Get("Author/" + id);
            Author data = JsonConvert.DeserializeObject<Author>(response.Body);
            return View(data);
        }
    }
}