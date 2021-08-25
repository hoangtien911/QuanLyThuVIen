using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using Newtonsoft.Json.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_SachController : Controller
    {
        // GET: Admin/ql_Sach
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv SHOW vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult ListBooks()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books");            
            Dictionary<string, Books> data = JsonConvert.DeserializeObject<Dictionary<string, Books>>(response.Body.ToString());
            var list = new List<Books>();
            foreach (var item in data)
            {
                Books book = new Books();
                book._id = item.Value._id;
                book.title = item.Value.title;
                book.authors = item.Value.authors;
                book.categories = item.Value.categories;
                book.shortDescription = item.Value.shortDescription;
                book.longDescription = item.Value.longDescription;
                book.count = item.Value.count;
                book.publishedDate = item.Value.publishedDate;
                book.status = item.Value.status;
                book.thumbnailUrl = item.Value.thumbnailUrl;
                list.Add(book);
            }
            return View(list);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DETAIL vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books/" + id);
            Books data = JsonConvert.DeserializeObject<Books>(response.Body);
            return View(data);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CREATE vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Books book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    client = new FireSharp.FirebaseClient(config);
                    PushResponse response = client.Push("Books/", book);
                    book._id = response.Result.name;
                    client.Set("Books/" + book._id, book);
                    ViewBag.MSCreate = "Test create alert";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Log >>> " + ex);
            }
            ViewBag.MSCreate = "Test Tesst Test";
            return RedirectToAction("ListBooks");
        }

        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv UPDATE vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books/" + id);
            Books data = JsonConvert.DeserializeObject<Books>(response.Body);           
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Books book)
        {
            string ms;
            try
            {
                client = new FireSharp.FirebaseClient(config);
                client.Set("Books/" + book._id, book);
                ms = "Lưu thành công!";
                ViewBag.MSEdit = ms;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Log >>> " + ex);
                ms = "Đã có lỗi sảy ra. Lưu thất bại!";
                ViewBag.MSEdit = ms;
            }
            return RedirectToAction("ListBooks");
        }


        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DELETE vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            client.Delete("Books/" + id);
            return RedirectToAction("ListBooks");
        }
    }
}