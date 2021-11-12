using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_NguoiDungController : Controller
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
        public ActionResult ListUser(string check)
        {
            if (!Data_Users.UpdateCount)
            {               
                return View(Data_Users.GetAllData());
            }
            else
            {
                return View(Data_Users.UserList);
            }
        }
        //2. Thêm mới người dùng
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (user.username != null && user.password != null)
            {
                Data_Users.CreateData(user);
            }
            return View();       
        }
        //3. Sửa thông tin người dùng
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(Data_Users.GetSingleData(id));
        }
        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (user.username != null && user.password != null)
            {
                Data_Users.EditData(user);
                return RedirectToAction("ListUser");
            }

            return View();
        }
        //4. Xoá người dùng
        public ActionResult Delete(string id)
        {
            Data_Users.DeleteData(id);
            return RedirectToAction("ListUser");
        }
        //5. Chi tiết người dùng
        [HttpGet]
        public ActionResult Detail(string id)
        {
            return View(Data_Users.GetSingleData(id));
        }      
    }
}