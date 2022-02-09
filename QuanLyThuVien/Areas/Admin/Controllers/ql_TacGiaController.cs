using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_TacGiaController : BaseController
    {
        /* Controller Quản lý tác giả
         * 
         * 1. Danh sách tác giả
         * 2. Thêm mới và sửa tác giả
         * 3. Lấy 1 tác giả
         * 4. Xoá tác giả
         */

        //1. Danh sách tác giả
        public ActionResult ListAuthor()
        {
            return View();
        }
        public JsonResult GetListAuthor()
        {
            if (!Data_Authors.UpdateCount)
                Data_Authors.GetAllData();
            return Json(Data_Authors.AuthorsList, JsonRequestBehavior.AllowGet);
        }
        //2. Thêm mới và sửa tác giả      
        public JsonResult Create_Edit(Author author)
        {
            if (author.name != null)
            {
                //Create
                if (author.id == null)
                {
                    if (Data_Authors.CreateData(author))
                        return Json(new { status = "ADD_OK" });
                    else
                        return Json(new { status = "ADD_FALSE" });
                }
                //Edit
                else
                {
                    if (Data_Authors.EditData(author))
                        return Json(new { status = "EDIT_OK" });
                    else
                        return Json(new { status = "EDIT_FALSE" });
                }
            }
            else
                return Json(new { status = "NO_INPUT_DATA" });
        }      
        //3. Lấy 1 tác giả
        public JsonResult GetAuthor(string id)
        {
            Author author = Data_Authors.GetSingleData(id);
            if (author != null)
                return Json(author, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }
        //4. Xoá tác giả
        public ActionResult Delete(string id)
        {
            if (Data_Authors.DeleteData(id))
                return Json(new { status = "DELETE_OK" });
            else
                return Json(new { status = "DELETE_FALSE" });
        }      
    }
}