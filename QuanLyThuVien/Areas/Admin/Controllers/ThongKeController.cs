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

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        // GET: Admin/ThongKe
        private IFirebaseClient client;
        private IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };     
        public ActionResult ThongKe()
        {
            client = new FireSharp.FirebaseClient(config);
            //
            FirebaseResponse response = client.Get("User");
            Dictionary<string, User> UserData = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Body.ToString());
            ViewBag.UserCount = UserData.Count;
            //
            response = client.Get("Books");
            Dictionary<string, Books> BookData = JsonConvert.DeserializeObject<Dictionary<string, Books>>(response.Body.ToString());
            float TongSach = 0;
            float SachCon = 0;
            float tiLe = 0;
            foreach (var book in BookData)
            {
                TongSach += book.Value.count;
                SachCon += book.Value.count_in;

            }
            tiLe = (SachCon / TongSach) * 100;
            ViewBag.TiLeSach = (int) tiLe;
            ViewBag.BookCount = TongSach;
            //
            response = client.Get("CallCard");
            Dictionary<string, CallCard> CallCardData = JsonConvert.DeserializeObject<Dictionary<string, CallCard>>(response.Body.ToString());
            ViewBag.CallCardCount = CallCardData.Count;

            List<CallCard> listCallCard = new List<CallCard>();
            foreach (var item in CallCardData)
            {
                if (item.Value.status.Equals("Quá hạn"))
                {
                    CallCard callCard = new CallCard();
                    callCard.id = item.Value.id;
                    callCard.books_id = item.Value.books_id;
                    callCard.user_id = item.Value.user_id;
                    FirebaseResponse getUsername = client.Get("User/" + item.Value.user_id + "/username");
                    callCard.user_id = JsonConvert.DeserializeObject<string>(getUsername.Body);
                    callCard.date_issued = item.Value.date_issued;
                    callCard.date_return = item.Value.date_return;
                    callCard.date_returned = item.Value.date_returned;
                    callCard.status = item.Value.status;
                    listCallCard.Add(callCard);
                }
            }
            ViewBag.listCallCard = listCallCard;

            ViewBag.listCallCard = listCallCard;
            //get books with callcard books_id                    
            ViewBag.listBooksCallCard = new List<Books>();
            foreach (var item in listCallCard)
            {
                item.books_id_temp = item.books_id.Split(',');
                foreach (var booksID in item.books_id_temp)
                {
                    FirebaseResponse getBook = client.Get("Books/" + booksID);
                    ViewBag.listBooksCallCard.Add(JsonConvert.DeserializeObject<Books>(getBook.Body.ToString()));
                }
            }
            //                         
            return View();
        }
        public JsonResult DataThongKe(string status)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("CallCard");
            Dictionary<string, CallCard> CallCardData = JsonConvert.DeserializeObject<Dictionary<string, CallCard>>(response.Body.ToString());
            int DangMuons = 0;
            int DaTras = 0;
            int QuaHans = 0;
            foreach(var item in CallCardData)
            {
                if (item.Value.status == "Đang mượn")
                    DangMuons++;
                if (item.Value.status == "Đã trả")
                    DaTras++;
                if (item.Value.status == "Quá hạn")
                    QuaHans++;
            }
            return Json(new
            {
                Tong = CallCardData.Count,
                DangMuon = DangMuons,
                DaTra = DaTras,
                QuaHan = QuaHans,
                Status = status,
            });

        }
    }
}