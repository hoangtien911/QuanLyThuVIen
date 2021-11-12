using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_TheLoaiController : Controller
    {       
        /* Controller Quản thể loại
        * 
        * 1. Danh sách thể loại
        * 2. Thêm mới thể loại
        * 3. Sửa thông tin thể loại
        * 4. Xoá thể loại
        * 5. Chi tiết thể loại         
        */
        //1. Danh sách thể loại
        public ActionResult ListCategories(string message)
        {          
            if (!Data_Categories.UpdateCount)
            {
                return View(Data_Categories.GetAllData());
            }
            else
            {
                return View(Data_Categories.CategoriesList);
            }
        }
        //2. Thêm mới thể loại
        [HttpGet]
        public ActionResult Create()
        {          
            return View();
        }
        [HttpPost]
        public ActionResult Create(Categories categories)
        {
            if (categories.name != null)
            {               
                Data_Categories.CreateData(categories);
            }
            return View();
        }
        //3. Sửa thông tin thể loại
        [HttpGet]
        public ActionResult Edit(string id)
        {                      
            return View(Data_Categories.GetSingleData(id));
        }
        [HttpPost]
        public ActionResult Edit(Categories categories)
        {
            if (categories.name != null)
            {
                Data_Categories.EditData(categories);
                return RedirectToAction("ListCategories");
            }          
            return View();          
        }
        //4. Xoá thông tin thể loại
        public ActionResult Delete(string id)
        {
            Data_Categories.DeleteData(id);
            return RedirectToAction("ListCategories");
        }
        //5. Chi tiết thể loại
        [HttpGet]
        public ActionResult Detail(string id)
        {
            return View(Data_Categories.GetSingleData(id));
        }
    }
}