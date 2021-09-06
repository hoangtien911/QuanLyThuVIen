using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using System;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_PhieuMuonController : Controller
    {
        // GET: Admin/ql_PhieuMuon
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH SÁCH vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult ListCallCard(string check)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("CallCard");
            Dictionary<string, CallCard> data = JsonConvert.DeserializeObject<Dictionary<string, CallCard>>(response.Body.ToString());
            var list = new List<CallCard>();
            foreach (var item in data)
            {
                CallCard callCard = new CallCard();
                callCard.id = item.Value.id;
                callCard.books_id = item.Value.books_id;
                callCard.user_id = item.Value.user_id;
                callCard.date_issued = item.Value.date_issued;
                callCard.date_return = item.Value.date_return;
                callCard.date_returned = item.Value.date_returned;
                if(callCard.date_return != null)
                {
                    if (DateTime.Compare(callCard.date_return, DateTime.Now) < 0)
                        callCard.status = "Quá hạn";
                    if (DateTime.Compare(callCard.date_return, DateTime.Now) > 0)
                        callCard.status = "Đang mượn";
                    if (DateTime.Compare(callCard.date_return, new DateTime(0001, 01, 01)) == 0)
                        callCard.status = "Chưa cập nhật";              
                }
                if (DateTime.Compare(callCard.date_returned, new DateTime(0001, 01, 01)) != 0)
                    callCard.status = "Đã trả";
                list.Add(callCard);
            }
            if (check != null)
            {
                if (check.Equals("1"))
                {
                    ViewBag.MsEdit = "Sửa thông tin phiếu mượn thành công!";
                    check = null;
                }
                else
                {
                    ViewBag.MsDelete = check;
                    check = null;
                }
            }
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            return View(list);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("CallCard/" + id);
            CallCard data = JsonConvert.DeserializeObject<CallCard>(response.Body);
            return View(data);
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv THÊM MỚI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            return View();
        }
        [HttpPost]
        public ActionResult Create(CallCard callCard)
        {
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            if (callCard.user_id != null && callCard.books_id_temp != null)
            {
                client = new FireSharp.FirebaseClient(config);
                // gộp mảng sách sang chuỗi
                callCard.books_id = string.Join(",", callCard.books_id_temp);
                callCard.books_id_temp = null;
                //
                PushResponse response = client.Push("CallCard/", callCard);
                callCard.id = response.Result.name;
                client.Set("CallCard/" + callCard.id, callCard);
                ViewBag.MsCreate = "Thêm mới phiếu mượn thành công!";
                return View();
            }
            else
            {
                ViewBag.MsCreate = "Đã có lỗi xảy ra. Thêm mới phiếu mượn thất bại!";
                return View();
            }
        }

        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv SỬA vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("CallCard/" + id);
            CallCard data = JsonConvert.DeserializeObject<CallCard>(response.Body);
            //Tách chuỗi sách sang mảng
            if (data.books_id != null)
                data.books_id_temp = data.books_id.Split(',');           
            //
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(CallCard callCard)
        {
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            if (callCard.user_id != null && callCard.books_id != null)
            {
                // gộp mảng sách sang chuỗi
                callCard.books_id = string.Join(",", callCard.books_id_temp);
                callCard.books_id_temp = null;
                //
                client = new FireSharp.FirebaseClient(config);
                client.Set("CallCard/" + callCard.id, callCard);
                return RedirectToAction("ListCallCard", new { @check = 1 });
            }
            else
            {
                ViewBag.MsEdit = "Đã có lỗi xảy ra. Sửa thông tin phiếu mượn thất bại!";
                return View();
            }

        }

        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv XOÁ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult Delete(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("CallCard/" + id);
                CallCard data = JsonConvert.DeserializeObject<CallCard>(response.Body);
                client.Delete("CallCard/" + id);
                return RedirectToAction("ListCallCard", new { @check = data.id });
            }
            catch
            {
                ViewBag.MsDelete = "Đã có lỗi xảy ra. Xoá phiếu mượn thất bại!";
                return RedirectToAction("ListCallCard");
            }

        }

        /// <returns></returns>
        public List<User> getUser()
        {
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
            return list;
        }
        public List<Books> getBooks()
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