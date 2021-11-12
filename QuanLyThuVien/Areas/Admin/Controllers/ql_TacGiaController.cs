using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_TacGiaController : Controller
    {
        /* Controller Quản lý tác giả
         * 
         * 1. Danh sách tác giả
         * 2. Thêm mới tác giả
         * 3. Sửa thông tin tác giả
         * 4. Xoá tác giả
         * 5. Chi tiết tác giả
         */

        //1. Danh sách tác giả
        public ActionResult ListAuthor(string message)
        {
            if (!Data_Authors.UpdateCount)
            {               
                return View(Data_Authors.GetAllData());
            }
            else
            {
                return View(Data_Authors.AuthorsList);
            }           
        }
        //2. Thêm mới tác giả
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Author author)
        {
            if (author.name != null)
            {
                Data_Authors.CreateData(author);
            }
            return View();
        }
        //3. Sửa thông tin tác giả
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(Data_Authors.GetSingleData(id));
        }
        [HttpPost]
        public ActionResult Edit(Author author)
        {
            if (author.name != null)
            {
                Data_Authors.EditData(author);
                return RedirectToAction("ListAuthor");
            }
            return View();
        }
        //4. Xoá tác giả
        public ActionResult Delete(string id)
        {           
            Data_Authors.DeleteData(id);
            return RedirectToAction("ListAuthor");
        }
        //5. Chi tiết tác giả
        [HttpGet]
        public ActionResult Detail(string id)
        {
            return View(Data_Authors.GetSingleData(id));
        }
    }
}