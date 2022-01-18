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
         * 2. Thêm mới và sửa phiếu mượn
         * 3. Lấy thông tin 1 phiếu mượn
         * 4. Lấy thông tin 1 phiếu mượn Xoá
         * 5. Xoá phiếu mượn      
         */

        //1. Danh sách phiếu mượn
        public ActionResult ListCallCard()
        {
            if (!Data_Users.UpdateCount)
                ViewBag.ListUser = Data_Users.GetAllData();
            else
                ViewBag.ListUser = Data_Users.UserList;
            if (!Data_Books.UpdateCount)
                ViewBag.ListBook = Data_Books.GetAllData();
            else
                ViewBag.ListBook = Data_Books.BooksList;
            return View();
        }
        public JsonResult GetListCallCard()
        {
            if (!Data_CallCard.UpdateCount)
                Data_CallCard.GetAllData();
            //lấy tên người dùng và tên sách theo id
            foreach (var callcard in Data_CallCard.CallcardList)
            {
                if (callcard.user_id != null)
                {
                    callcard.username = Data_Users.GetSingleData(callcard.user_id).username;
                }
                if (callcard.books_id != null)
                {
                    callcard.books_id_temp = callcard.books_id.Split(',');                   
                    for (int i = 0; i < callcard.books_id_temp.Length; i++)
                    {
                        string idBook = callcard.books_id_temp[i];
                        string nameBook = Data_Books.GetSingleData(idBook).title;
                        callcard.books_id_temp[i] = nameBook;
                    }                  
                }
            }
            return Json(Data_CallCard.CallcardList, JsonRequestBehavior.AllowGet);
        }
        //2. Thêm mới và sửa phiếu mượn      
        public JsonResult Create_Edit(CallCard callCard)
        {
            if (callCard.user_id != null && callCard.books_id_temp != null)
            {
                // gộp mảng sách sang chuỗi
                callCard.books_id = string.Join(",", callCard.books_id_temp);               
                callCard.books_id_temp = null;
                callCard.username = null;
                //Create
                if (callCard.id == null)
                {
                    if (Data_CallCard.CreateData(callCard))
                    {
                        return Json(new { status = "ADD_OK" });
                    }                   
                    return Json(new { status = "ADD_FALSE" });
                }
                //Edit
                else
                {
                    if (Data_CallCard.EditData(callCard))
                    {
                        return Json(new { status = "EDIT_OK" });
                    }
                    else
                    {
                        return Json(new { status = "EDIT_FALSE" });
                    }
                }

            }
            else
            {
                string propetyName = "";
                if (callCard.user_id == null)
                    propetyName += "Người mượn, ";
                if (callCard.books_id_temp == null)
                    propetyName += "Sách mượn, ";               
                return Json(new { status = "NO_INPUT_DATA", propery = propetyName });
            }


        }
        //3. Lấy thông tin 1 phiếu mượn
        public JsonResult GetCallCard(string id)
        {
            CallCard callCard = Data_CallCard.GetSingleData(id);
            if (callCard != null)
            {

                callCard.books_id_temp = callCard.books_id.Split(',');               
                return Json(callCard, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }
        //4. Lấy thông tin 1 phiếu mượn Xoá
        public JsonResult GetCallCard_D(string id)
        {
            CallCard callCard = Data_CallCard.GetSingleData(id);
            if (callCard != null)
            {
                callCard.username = Data_Users.GetSingleData(callCard.user_id).username;
                callCard.books_id_temp = callCard.books_id.Split(',');
                for (int i = 0; i < callCard.books_id_temp.Length; i++)
                {
                    string idBook = callCard.books_id_temp[i];
                    string nameBook = Data_Books.GetSingleData(idBook).title;
                    callCard.books_id_temp[i] = nameBook;
                }
                return Json(callCard, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }
        //5. Xoá phiếu mượn
        public ActionResult Delete(string id)
        {
            if (Data_CallCard.DeleteData(id))
                return Json(new { status = "DELETE_OK" });
            else
                return Json(new { status = "DELETE_FALSE" });
        }
    }
}