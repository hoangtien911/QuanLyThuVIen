using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_NguoiDungController : BaseController
    {
        /* Controller Quản lý người dùng
         * 
         * 1. Danh sách người dùng
         * 2. Thêm mới người dùng
         * 3. Sửa thông tin người dùng
         * 4. Xoá người dùng
         * 5. Chi tiết người dùng
         */

        //1. Danh sách người dùng
        public ActionResult ListUser()
        {
            return View();
        }
        public JsonResult GetListUser()
        {
            if (!Data_Users.UpdateCount)
                Data_Users.GetAllData();
            return Json(Data_Users.UserList, JsonRequestBehavior.AllowGet);
        }
        //2. Thêm mới và sửa người dùng
        public ActionResult Create_Edit(User user)
        {
            if (user.username != null && user.password != null)
            {
                //Create
                if (user.id == null)
                {
                    if (Data_Users.CreateData(user))
                        return Json(new { status = "ADD_OK" });
                    else
                        return Json(new { status = "ADD_FALSE" });
                }
                //Edit
                else
                {
                    if (Data_Users.EditData(user))
                        return Json(new { status = "EDIT_OK" });
                    else
                        return Json(new { status = "EDIT_FALSE" });
                }                
            }
            else
            {
                string propetyName = "";
                if (user.username == null)
                    propetyName += "Username, ";
                if (user.email == null)
                    propetyName += "Email";
                if (user.password == null)
                    propetyName += "Password";
                return Json(new { status = "NO_INPUT_DATA", propery = propetyName });
            }
                
        }
        //3. Lấy 1 người dùng       
        public ActionResult GetUser(string id)
        {
            User user = Data_Users.GetSingleData(id);
            if(user != null)
                return Json(user, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }
        //4. Xoá người dùng
        public ActionResult Delete(string id)
        {
            if (Data_Users.DeleteData(id))
                return Json(new { status = "DELETE_OK" });
            else
                return Json(new { status = "DELETE_FALSE" });
        }     
    }
}