using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using System.Linq;

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
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult ListBooks(string check)
        {           
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body.ToString());
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
            if (check != null)
            {
                if (check.Equals("1"))
                {
                    ViewBag.MsEdit = "Sửa thông tin sách thành công!";
                    check = null;
                }
                else
                {
                    ViewBag.MsDelete = check;
                    check = null;
                }               
            }
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            return View(list);
        }       
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books/" + id);
            Books data = JsonConvert.DeserializeObject<Books>(response.Body);
            return View(data);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv THÊM MỚI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            return View();
        }
        [HttpPost]
        public ActionResult Create(Books book)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            if (book.title != null && book.author_temp != null)
            {
                client = new FireSharp.FirebaseClient(config);
                // gộp mảng tác giả, thể loại sang chuỗi
                book.authors = string.Join(",", book.author_temp);
                book.categories = string.Join(",", book.categories_temp);
                book.author_temp = null;
                book.categories_temp = null;
                //
                PushResponse response = client.Push("Books/", book);
                book._id = response.Result.name;
                client.Set("Books/" + book._id, book);
                ViewBag.MsCreate = "Thêm mới sách thành công!";                            
                return View();
            }
            else
            {
                ViewBag.MsCreate = "Đã có lỗi xảy ra. Thêm mới sách thất bại!";
                return View();
            }
        }

        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv SỬA vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books/" + id);
            Books data = JsonConvert.DeserializeObject<Books>(response.Body);
            //Tách chuỗi tác giả, thể loại sang mảng
            if (data.authors != null)
                data.author_temp = data.authors.Split(',');   
            if (data.categories != null)
                data.categories_temp = data.categories.Split(',');
            //
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Books book)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            if (book.title != null && book.author_temp != null)
            {
                // gộp mảng tác giả, thể loại sang chuỗi
                book.authors = string.Join(",", book.author_temp);
                book.categories = string.Join(",", book.categories_temp);
                book.author_temp = null;
                book.categories_temp = null;
                //
                client = new FireSharp.FirebaseClient(config);
                client.Set("Books/" + book._id, book);             
                return RedirectToAction("ListBooks", new { @check = 1 });
            }
            else
            {
                ViewBag.MsEdit = "Đã có lỗi xảy ra. Sửa thông tin sách thất bại!";
                return View();
            }

        }

        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv XOÁ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult Delete(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Books/" + id);
                Books data = JsonConvert.DeserializeObject<Books>(response.Body);
                client.Delete("Books/" + id);
                return RedirectToAction("ListBooks", new { @check = data.title });
            }
            catch
            {
                ViewBag.MsDelete = "Đã có lỗi xảy ra. Xoá sách thất bại!";
                return RedirectToAction("ListBooks");
            }
            
        }
      
        /// <returns></returns>
        public List<Author> getAuthor()
        {
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
            return list;
        }
        public List<Categories> getCategories()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Categories");
            Dictionary<string, Categories> data = JsonConvert.DeserializeObject<Dictionary<string, Categories>>(response.Body.ToString());
            var list = new List<Categories>();
            foreach (var item in data)
            {
                Categories categories = new Categories();
                categories.id = item.Value.id;
                categories.name = item.Value.name;
                categories.shortDescription = item.Value.shortDescription;
                categories.longDescription = item.Value.longDescription;
                categories.image = item.Value.image;
                list.Add(categories);
            }
            return list;
        }
    }
}