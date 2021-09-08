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
    public class TrangChuController : Controller
    {
        // GET: TrangChu
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        public ActionResult HomePage()
        {
            ViewBag.listbooks = ListBooks();
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
    }
}