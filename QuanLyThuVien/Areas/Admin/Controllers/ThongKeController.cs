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
using QuanLyThuVien.Areas.Admin.Data;
using System.Threading;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {     
        public ActionResult ThongKe()
        {
            //setup
            Data_Users.GetAllData();
            Data_Books.GetAllData();
            Data_CallCard.GetAllData();
            //Lấy số lượng người dùng
            Thread t1 = new Thread(() =>
            {
                ViewBag.UserCount = Data_Users.UserList.Count;
            });
            //Lấy số lượng sách
            Thread t2 = new Thread(() =>
            {              
                float TongSach = 0;
                float SachCon = 0;
                float tiLe = 0;
                foreach (var book in Data_Books.BooksList)
                {
                    TongSach += book.count;
                    SachCon += book.count_in;

                }
                tiLe = (SachCon / TongSach) * 100;
                ViewBag.TiLeSach = tiLe;
                ViewBag.BookCount = Data_Books.BooksList.Count;
            });
            //Lấy số lượng phiếu mượn và tên người mượn
            Thread t3 = new Thread(() =>
            {
                List<CallCard> callCards = Data_CallCard.CallcardList;
                foreach(var card in callCards)
                {
                    User user = Data_Users.GetSingleData(card.user_id);
                    card.user_id = user.username;
                }
                ViewBag.CallCardCount = Data_CallCard.CallcardList.Count;
                ViewBag.listCallCard = callCards;
            });
            //lấy sách theo id trong phiếu mượn     
            Thread t4 = new Thread(() =>
            {
                ViewBag.listBooksCallCard = new List<Books>();
                foreach (var callCard in Data_CallCard.CallcardList)
                {
                    callCard.books_id_temp = callCard.books_id.Split(',');
                    foreach (var booksID in callCard.books_id_temp)
                    {
                        ViewBag.listBooksCallCard.Add(Data_Books.GetSingleData(booksID));
                    }
                }
            });         
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            return View();
        }
        public JsonResult DataThongKe(string status)
        {          
            int DangMuons = 0;
            int DaTras = 0;
            int QuaHans = 0;
            foreach(var item in Data_CallCard.CallcardList)
            {
                if (item.status == "Đang mượn")
                    DangMuons++;
                if (item.status == "Đã trả")
                    DaTras++;
                if (item.status == "Quá hạn")
                    QuaHans++;
            }
            return Json(new
            {
                Tong = Data_CallCard.CallcardList.Count,
                DangMuon = DangMuons,
                DaTra = DaTras,
                QuaHan = QuaHans,
                Status = status,
            });
        }
    }
}