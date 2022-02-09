using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_TheLoaiController : BaseController
    {       
        /* Controller Quản thể loại
        * 
        * 1. Danh sách thể loại
        * 2. Thêm mới thể loại
        * 3. Lấy 1 thể loại
        * 4. Xoá thể loại       
        */
        //1. Danh sách thể loại
        public ActionResult ListCategories()
        {
            return View();
        }
        public JsonResult GetListCate()
        {
            if (!Data_Categories.UpdateCount)
                Data_Categories.GetAllData();
            return Json(Data_Categories.CategoriesList, JsonRequestBehavior.AllowGet);
        }
        //2. Thêm mới và sửa thể loại
        public JsonResult Create_Edit(Categories categories)
        {
            if (categories.name != null)
            {
                //Create
                if (categories.id == null)
                {
                    if (Data_Categories.CreateData(categories))
                        return Json(new { status = "ADD_OK" });
                    else
                        return Json(new { status = "ADD_FALSE" });
                }
                //Edit
                else
                {
                    if (Data_Categories.EditData(categories))
                        return Json(new { status = "EDIT_OK" });
                    else
                        return Json(new { status = "EDIT_FALSE" });
                }
            }
            else
                return Json(new { status = "NO_INPUT_DATA" });
        }      
        //3. Lấy một thể loại
        public JsonResult GetCategories(string id)
        {
            Categories categories = Data_Categories.GetSingleData(id);
            if (categories != null)
                return Json(categories, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }
        //4. Xoá thông tin thể loại
        public ActionResult Delete(string id)
        {
            if (Data_Categories.DeleteData(id))
                return Json(new { status = "DELETE_OK" });
            else
                return Json(new { status = "DELETE_FALSE" });
        }       
    }
}