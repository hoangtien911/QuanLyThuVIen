using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;


namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_PhieuMuonController : Controller
    {
        /* Controller Quản lý phiếu mượn
         * 
         * 1. Danh sách phiếu mượn
         * 2. Thêm mới phiếu mượn
         * 3. Sửa thông tin phiếu mượn
         * 4. Xoá phiếu mượn
         * 5. Chi tiết phiếu mượn
         * 6. Lấy danh sách người dùng
         * 7. Lấy danh sách sách
         * 8. Hàm kiểm tra trạng thái sách theo ngày mượn - trả - hẹn trả
         */

        //1. Danh sách phiếu mượn
        public ActionResult ListCallCard(string message)
        {
           
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            if (!Data_CallCard.UpdateCount)
            {
                return View(Data_CallCard.GetAllData());
            }
            else
            {
                return View(Data_CallCard.CallcardList);
            }
        }
       
        //2. Thêm mới phiếu mượn
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
            string message = "";
            if (callCard.user_id != null && callCard.books_id_temp != null)
            {             
                //Trừ sách trong kho
                foreach(var item in callCard.books_id_temp)
                {
                    Books books_temp = Data_Books.GetSingleData(item);
                    books_temp.count_in--;
                    if (books_temp.count_in < 0)
                    {
                        message += books_temp.title + ", ";
                    }
                    else
                    {
                        Data_Books.EditData(books_temp);
                    }
                }
                //nếu message rỗng (số lượng sách không âm)
                if (message.Equals(""))
                {
                    //Gộp mảng id sách sang chuỗi
                    callCard.books_id = string.Join(",", callCard.books_id_temp);
                    callCard.books_id_temp = null;
                    //Kiểm tra trạng thái
                    callCard.status = checkStatus(callCard);
                    //Thêm mới
                    Data_CallCard.CreateData(callCard);
                }
            }
            return View();
        }

        //3. Sửa thông tin phiếu mượn
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            CallCard data = Data_CallCard.GetSingleData(id);
            //Tách chuỗi id sách sang mảng
            if (data.books_id != null)
                data.books_id_temp = data.books_id.Split(',');
            //Cộng sách chuẩn bị sửa
            foreach (var item in data.books_id_temp)
            {
                Books books_temp = Data_Books.GetSingleData(item);
                books_temp.count_in++;
                Data_Books.EditData(books_temp);
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(CallCard callCard)
        {
            ViewBag.listUser = getUser();
            ViewBag.listBooks = getBooks();
            string message = "";
            if (callCard.user_id != null && callCard.books_id_temp != null)
            {
                //Trừ sách trong kho
                foreach (var item in callCard.books_id_temp)
                {
                    Books books_temp = Data_Books.GetSingleData(item);
                    books_temp.count_in--;
                    if (books_temp.count_in < 0)
                    {
                        message += books_temp.title + ", ";
                    }
                    else
                    {
                        Data_Books.EditData(books_temp);
                    }
                }
                //nếu message rỗng (số lượng sách không âm)
                if (message.Equals(""))
                {
                    //Gộp mảng id sách sang chuỗi
                    callCard.books_id = string.Join(",", callCard.books_id_temp);
                    callCard.books_id_temp = null;
                    //Kiểm tra trạng thái
                    callCard.status = checkStatus(callCard);
                    //Sửa sách
                    Data_CallCard.EditData(callCard);
                }
            }
            return RedirectToAction("ListCallCard");
        }
        //4. Xoá phiếu mượn
        public ActionResult Delete(string id)
        {
            Data_CallCard.DeleteData(id);
                return RedirectToAction("ListCallCard");
           
        }
        //5. Chi tiết phiếu mượn
        [HttpGet]
        public ActionResult Detail(string id)
        {
            CallCard data = Data_CallCard.GetSingleData(id);
            ViewBag.UserDetail = Data_Users.GetSingleData(data.user_id);
            //Tách chuỗi sách sang mảng
            if (data.books_id != null)
                data.books_id_temp = data.books_id.Split(',');
            //
            int i = 0;
            int length_listBooks = data.books_id_temp.Length - 1;
            string namebooks = "";
            foreach (var bookid in data.books_id_temp)
            {
                Books books_temp = Data_Books.GetSingleData(bookid);
                if (i < length_listBooks)
                    namebooks += books_temp.title + ", ";
                else
                    namebooks += books_temp.title + ".";
                i++;
            }
            ViewBag.nameBooks = namebooks;
            return View(data);
        }
        //6. Lấy danh sách người dùng
        public List<User> getUser()
        {
            if (!Data_Users.UpdateCount)
            {
                return (Data_Users.GetAllData());
            }
            else
            {
                return (Data_Users.UserList);
            }
        }
        //7. Lấy danh sách sách
        public List<Books> getBooks()
        {
            if (!Data_Books.UpdateCount)
            {
                return (Data_Books.GetAllData());
            }
            else
            {
                return (Data_Books.BooksList);
            }
        }
        //8. Hàm kiểm tra trạng thái sách theo ngày mượn - trả - hẹn trả
        public string checkStatus (CallCard callCard)
        {
            if (callCard.date_return != null)
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
            return callCard.status;
        }
    }
}