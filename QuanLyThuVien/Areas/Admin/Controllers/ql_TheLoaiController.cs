using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_TheLoaiController : Controller
    {
        // GET: Admin/ql_TheLoai
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv DANH SÁCH THỂ LOẠI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult ListCategories(string check)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//         
            if (check != null)
            {
                if (check.Equals("1"))
                {
                    ViewBag.MsEdit = "Sửa thông tin thể loại thành công!";
                    check = null;
                }
                else
                {
                    ViewBag.MsDelete = check;
                    check = null;
                }
            }
            if (Data_Categories.UpdateCount == 0)
            {
                Data_Categories.GetAllData();                
                return View(Data_Categories.CategoriesList);
            }
            else
            {
                return View(Data_Categories.CategoriesList);
            }
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv THÊM MỚI vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Create()
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            return View();
        }
        [HttpPost]
        public ActionResult Create(Categories categories)
        {
            if (categories.name != null)
            {
                Data_Categories.CreateData(categories);
                return View();
            }
            else
            {
                ViewBag.MsCreate = "Đã có lỗi xảy ra. Thêm mới thể loại thất bại!";
                return View();
            }
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv SỬA vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Edit(string id)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//         
            return View(Data_Categories.GetSingleData(id));
        }
        [HttpPost]
        public ActionResult Edit(Categories categories)
        {
            if (categories.name != null)
            {
                Data_Categories.EditData(categories);
                return RedirectToAction("ListCategories", new { @check = 1 });
            }
            else
            {
                ViewBag.MsEdit = "Đã có lỗi xảy ra. Sửa thông tin thể loại thất bại!";
                return View();
            }
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv XOÁ vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        public ActionResult Delete(string id)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//
            try
            {            
                Categories data = Data_Categories.GetSingleData(id);
                Data_Categories.DeleteData(id);
                return RedirectToAction("ListCategories", new { @check = data.name });
            }
            catch
            {
                ViewBag.MsDelete = "Đã có lỗi xảy ra. Xoá thể loại thất bại!";
                return RedirectToAction("ListCategories");
            }
        }
        //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv CHI TIẾT vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv//
        [HttpGet]
        public ActionResult Detail(string id)
        {
            //------------- START Check Session ---------------//
            if (Session["AdminSession"] == null)
                return RedirectToAction("PageNotFound", "Error", new { area = "", status = "Bạn không có quyền truy cập vào trang này!" });
            //------------- END Check Session ---------------//            
            return View(Data_Categories.GetSingleData(id));
        }
    }
}