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
    public class SachController : Controller
    {
        // GET: Sach
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        public ActionResult KhoSach()
        {
            ViewBag.listbooks = ListBooks();
            ViewBag.listauthor = ListAuthor();
            ViewBag.listcategories = ListCategories();
            int[] MangTheLoai = new int[ListCategories().Count];
            int i = 0;
            foreach(var theLoai in ListCategories())
            {
                foreach(var sach in ListBooks())
                {
                    if (sach.categories != null)
                        sach.categories_temp = sach.categories.Split(',');
                    foreach(var idTheLoai in sach.categories_temp)
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
            return View();           
        }

        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
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
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH TÁC GIẢ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
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
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH THỂ LOẠI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
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
    }
}