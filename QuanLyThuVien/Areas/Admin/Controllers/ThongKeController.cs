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
using System.Globalization;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ThongKeController : BaseController
    {     
        public ActionResult ThongKe()
        {
            //Check data
            if (!Data_Books.UpdateCount)
                Data_Books.GetAllData();
            if (!Data_Users.UpdateCount)
                Data_Users.GetAllData();
            //Logic
            Thread t1 = new Thread(() =>
            {
                int BookCount = Data_Books.BooksList.Count;
                int Account = 0;
                foreach(var usertemp in Data_Users.UserList)
                {
                    if (usertemp.status != "Admin")
                        Account++;
                }
                ViewBag.Account = Account;
                ViewBag.BookCount = BookCount;
            });
            Thread t2 = new Thread(() =>
            {
                int TotalBook = 0;
                int nBook = 0;
                foreach (var book in Data_Books.BooksList)
                {
                    TotalBook += book.count;
                    nBook += book.count_in;
                }
                ViewBag.TotalBook = TotalBook;
                ViewBag.nBook = nBook;
            });
            t1.Start();
            t2.Start();                                          
            return View();
        }
        public JsonResult DataBarChart()
        {
            int Month = DateTime.Now.Month;
            //Check data
            if (!Data_CallCard.UpdateCount)
                Data_CallCard.GetAllData();
            //Logic

            ///Tìm số phiếu mượn theo từng tháng để hiển thị lên biểu đồ
            int[] CountCallCard = new int[12];
            Thread t1 = new Thread(() =>
            {
                int y = DateTime.Now.Year;
                foreach (var card in Data_CallCard.CallcardList)
                {
                    if (card.status == "Đã trả" || card.status == "Đang mượn")
                    {
                        DateTime timeTemp = DateTime.ParseExact(card.date_issued, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        if(timeTemp.Year == y)
                        {
                            int m = timeTemp.Month - 1;
                            CountCallCard[m] += 1;
                        }                 
                    }
                }
            });        
            ///Tổng phiếu mượn
            int AllCallCard = Data_CallCard.CallcardList.Count;
            ///Tính tỉ lệ phiếu mượn tháng này so với tháng trước
            float rate = 0;
            Thread t2 = new Thread(() =>
            {
                float CallCardLate = 0;
                float CallCardNow = 0;        
                ////Tháng 1 năm này và tháng 12 năm trước
                if (Month == 1)
                {
                    int y = DateTime.Now.Year - 1;
                    foreach (var card in Data_CallCard.CallcardList)
                    {
                        if (card.status == "Đã trả" || card.status == "Đang mượn")
                        {
                            DateTime timeTemp = DateTime.ParseExact(card.date_issued, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            if (timeTemp.Year == y && (timeTemp.Month) == 12)
                            {
                                CallCardLate += 1;
                            }
                            if (timeTemp.Year == (y + 1) && (timeTemp.Month) == 1)
                            {
                                CallCardNow += 1;
                            }
                        }
                    }
                }
                ////Tháng này và tháng trước
                if (Month > 1)
                {
                    foreach (var card in Data_CallCard.CallcardList)
                    {
                        if (card.status == "Đã trả" || card.status == "Đang mượn")
                        {
                            DateTime timeTemp = DateTime.ParseExact(card.date_issued, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            if ((timeTemp.Month) == Month -1)
                            {
                                CallCardLate += 1;
                            }
                            if ((timeTemp.Month) == Month)
                            {
                                CallCardNow += 1;
                            }
                        }
                    }
                }
                ////Tính tỷ lệ
                if (CallCardLate == 0 && CallCardNow == 0)
                    rate = 0;
                else if (CallCardLate == 0 && CallCardNow > 0)
                    rate = 100;
                else if (CallCardNow > 0 && CallCardNow == 0)
                    rate = -100;
                else
                {
                    rate = (CallCardNow / CallCardLate) * 100 -100;
                }
            });
            t1.Start();
            t2.Start();
            return Json(new { Month = Month, CountCallCard = CountCallCard, AllCallCard = AllCallCard, Rate = rate }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DataBarLine()
        {
            //checkData
            if (!Data_CallCard.UpdateCount)
                Data_CallCard.GetAllData();
            //logic
            float Count = Data_CallCard.CallcardList.Count;
            float[] CountCard = new float[6];
            foreach(var card in Data_CallCard.CallcardList)
            {
                if (card.status == "Đã trả")
                    CountCard[0]++;
                else if (card.status == "Đang mượn")
                    CountCard[1]++;
                else if (card.status == "Quá hạn")
                    CountCard[2]++;
                else if (card.status == "Chưa được duyệt")
                    CountCard[3]++;
                else if (card.status == "Lỗi thông tin")
                    CountCard[4]++;
                else if (card.status == "Chưa cập nhật")
                    CountCard[5]++;
            }
            double[] RateCard = new double[6];
            for(int i = 0; i < 6; i++)
            {
                RateCard[i] = CountCard[i] / Count * 100;
                RateCard[i] = Math.Round(RateCard[i]);
            }
            return Json(new { RateCard = RateCard }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DataPieChart()
        {
            //check data
            if (!Data_Users.UpdateCount)
                Data_Users.GetAllData();
            //logic
            int[] UserData = new int[3];
            int Count = 0;
            foreach(var user in Data_Users.UserList)
            {
                if (user.status == "Thường")
                    UserData[0]++;
                else if (user.status == "Vi phạm")
                    UserData[1]++;
                else if (user.status == "Khoá")
                    UserData[2]++;
                if (user.status != "Admin")
                    Count++;
            }
            return Json(new { UserData = UserData, Count = Count }, JsonRequestBehavior.AllowGet);
        }
    }
}