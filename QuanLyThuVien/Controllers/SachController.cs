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

namespace QuanLyThuVien.Controllers
{
    [HandleError]
    public class SachController : Controller
    {
        // GET: Sach
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv HIỂN THỊ TẤT CẢ SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult KhoSach()
        {
            ViewBag.listbooks = ListBooks();
            ViewBag.listauthor = ListAuthor();
            ViewBag.listcategories = ListCategories();

            int[] MangTheLoai = new int[ListCategories().Count];
            int i = 0;
            foreach (var theLoai in ListCategories())
            {
                foreach (var sach in ListBooks())
                {
                    if (sach.categories != null)
                        sach.categories_temp = sach.categories.Split(',');
                    foreach (var idTheLoai in sach.categories_temp)
                    {
                        if (theLoai.id == idTheLoai)
                        {
                            MangTheLoai[i]++;
                        }
                    }
                }
                i++;
            }
            ViewBag.soLuongSach = MangTheLoai;
            ViewBag.lenghtBooks = ListBooks().Count / 12 + 1;

            if (Request.Cookies["UserCookies"] != null)
            {
                Session["UserSession"] = Request.Cookies["UserCookies"]["userid"];
                return RedirectToAction("HomePage", "TrangChu");
            }
            return View();
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult ChiTietSach(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books/" + id);
            Books data = JsonConvert.DeserializeObject<Books>(response.Body);

            foreach (var TacGia in ListAuthor())
            {
                if (data.authors.Equals(TacGia.id))
                {
                    ViewBag.authors = TacGia.name;
                    break;
                }
            }

            ViewBag.listbooks = ListBooks();
            ViewBag.listauthor = ListAuthor();
            ViewBag.listcategories = ListCategories();
            int[] MangTheLoai = new int[ListCategories().Count];
            int i = 0;
            foreach (var theLoai in ListCategories())
            {
                foreach (var sach in ListBooks())
                {
                    if (sach.categories != null)
                        sach.categories_temp = sach.categories.Split(',');
                    foreach (var idTheLoai in sach.categories_temp)
                    {
                        if (theLoai.id == idTheLoai)
                        {
                            MangTheLoai[i]++;
                        }
                    }
                }
                i++;
            }
            ViewBag.soLuongSach = MangTheLoai;
            if (Session["UserSession"] != null)
            {
                FirebaseResponse response1 = client.Get("Favorite/" + Session["UserSession"]);
                Favorite favorite = JsonConvert.DeserializeObject<Favorite>(response1.Body.ToString());
                if (favorite.booksID != null)
                {                   
                    favorite.booksID_temp = favorite.booksID.Split(',');
                    for (int j = 0; j < favorite.booksID_temp.Length; j++)
                    {
                        if (favorite.booksID_temp[j].Equals(data._id))
                            ViewBag.checkFavorite = true;
                    }
                }

            }
            return View(data);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT TÁC GIẢ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult ChiTietTacGia(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Author/" + id);
            Author data = JsonConvert.DeserializeObject<Author>(response.Body);

            ViewBag.listbooks = ListBooks();
            ViewBag.listcategories = ListCategories();
            int[] MangTheLoai = new int[ListCategories().Count];
            int i = 0;
            foreach (var theLoai in ListCategories())
            {
                foreach (var sach in ListBooks())
                {
                    if (sach.categories != null)
                        sach.categories_temp = sach.categories.Split(',');
                    foreach (var idTheLoai in sach.categories_temp)
                    {
                        if (theLoai.id == idTheLoai)
                        {
                            MangTheLoai[i]++;
                        }
                    }
                }
                i++;
            }
            ViewBag.soLuongSach = MangTheLoai;
            List<Books> listBooksOfAuthor = new List<Books>();
            foreach (var item in ListBooks())
            {
                if (data.id.Equals(item.authors))
                {
                    listBooksOfAuthor.Add(item);
                }
            }
            foreach (var TacGia in ListAuthor())
            {
                if (data.id.Equals(TacGia.id))
                {
                    ViewBag.authors = TacGia.name;
                    break;
                }
            }
            ViewBag.listBooksOfAuthor = listBooksOfAuthor;
            return View(data);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT TÁC GIẢ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult ChiTietTheLoai(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Categories/" + id);
            Categories data = JsonConvert.DeserializeObject<Categories>(response.Body);

            ViewBag.listbooks = ListBooks();
            ViewBag.listcategories = ListCategories();
            int[] MangTheLoai = new int[ListCategories().Count];
            int i = 0;
            foreach (var theLoai in ListCategories())
            {
                foreach (var sach in ListBooks())
                {
                    if (sach.categories != null)
                        sach.categories_temp = sach.categories.Split(',');
                    foreach (var idTheLoai in sach.categories_temp)
                    {
                        if (theLoai.id == idTheLoai)
                        {
                            MangTheLoai[i]++;
                        }
                    }
                }
                i++;
            }
            ViewBag.soLuongSach = MangTheLoai;
            List<Books> listBooksOfCategories = new List<Books>();
            foreach (var item in ListBooks())
            {
                if (data.id.Equals(item.categories))
                {
                    foreach (var TacGia in ListAuthor())
                    {
                        if (item.authors.Equals(TacGia.id))
                        {
                            item.authors = TacGia.name;
                            break;
                        }
                    }
                    listBooksOfCategories.Add(item);
                }
            }
            ViewBag.listBooksOfCategories = listBooksOfCategories;
            return View(data);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv LẤY DANH SÁCH SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public List<Books> ListBooks()
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
            return list;
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv LẤY DANH SÁCH TÁC GIẢ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public List<Author> ListAuthor()
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
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv LẤY DANH SÁCH THỂ LOẠI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public List<Categories> ListCategories()
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

        public JsonResult addFavorite(string userID, string bookID)
        {
            client = new FireSharp.FirebaseClient(config);
            try
            {
                if (Session["UserSession"] != null)
                {
                    FirebaseResponse response = client.Get("Favorite/" + userID);
                    Favorite data = JsonConvert.DeserializeObject<Favorite>(response.Body);
                    if (data == null)
                        return Json(new { status = false });
                    if (data.booksID == null)
                        data.booksID = bookID;
                    else
                        data.booksID += "," + bookID;
                    client.Set("Favorite/" + userID, data);
                    return Json(new
                    {
                        status = true
                    });
                }
                return Json(new
                {
                    status = false
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }
        }
        public JsonResult removeFavorite(string userID, string bookID)
        {
            client = new FireSharp.FirebaseClient(config);
            try
            {
                if (Session["UserSession"] != null)
                {
                    FirebaseResponse response = client.Get("Favorite/" + userID);
                    Favorite data = JsonConvert.DeserializeObject<Favorite>(response.Body);
                    List<string> list = data.booksID.Split(',').ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Equals(bookID))
                        {
                            list.RemoveAt(i);
                            break;
                        }
                    }
                    data.booksID_temp = list.ToArray();
                    data.booksID = string.Join(",", list);
                    data.booksID_temp = null;
                    client.Set("Favorite/" + userID, data);
                    return Json(new
                    {
                        status = true
                    });
                }
                return Json(new
                {
                    status = false
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }
        }
    }
}