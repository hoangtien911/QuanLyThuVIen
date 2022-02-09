using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        /// <summary> Đăng nhập </summary>        
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(User user)
        {
            //check data
            if (!Data_Users.UpdateCount)
                Data_Users.GetAllData();
            //logic
            UserLogin userLogin = new UserLogin();
            bool check = false;
            foreach (var useritem in Data_Users.UserList)
            {
                if (user.username == useritem.username)
                {
                    if(user.password != useritem.password)
                    {
                        ViewBag.thongbao = "Mật khẩu không đúng";
                        return View();
                    }
                    else
                    {
                        userLogin.userid = useritem.id;
                        userLogin.username = useritem.username;
                        userLogin.avatar = useritem.avatar;
                        check = true;
                        break;
                    }
                }
            }
            if (!check)
            {
                ViewBag.thongbao = "Tên tài khoản không tồn tại.";
                return View();
            }         
            else
            {
                switch (Data_Users.GetSingleData(userLogin.userid).status)
                {
                    case "Admin":
                        {
                            Session["UserSession"] = userLogin.userid;
                            return RedirectToAction("ThongKe", "ThongKe", new { area = "Admin" });
                        }
                    case "Thường":
                        {
                            Session["UserSession"] = userLogin.userid;
                            Response.Cookies["UserCookies"]["username"] = userLogin.username;
                            Response.Cookies["UserCookies"]["userid"] = userLogin.userid;
                            if (user.remember)
                            {
                                Response.Cookies["UserCookies"].Expires = DateTime.Now.AddDays(15);
                            }
                            return RedirectToAction("HomePage", "TrangChu");
                        }
                    case "Khoá":
                        {
                            ViewBag.thongbao = "Tài khoản của bạn đã bị khoá. Xin vui lòng liên hệ trực tiếp với quản trị viên.";
                            return View();
                        }
                }             
            }
            return View();
        }

        /// <summary> Đăng xuất </summary>   
        [HttpGet]
        public ActionResult DangXuat()
        {
            Session["UserSession"] = null;
            Response.Cookies["UserCookies"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["AdminCookies"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("HomePage", "TrangChu");
        }
        /// <summary> Chi tiết thông tin cá nhân </summary>   
        public ActionResult ChiTietUser(string id)
        {
            if(Session["UserSession"] != null && Session["UserSession"].ToString() == id)
            {
                //lấy các dữ liệu user từ userid trong session 
                User getUser = Data_Users.GetSingleData(id);

                //Lấy dữ liệu sách yêu thích
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse getFavoriteResponse = client.Get("Favorite/" + getUser.id);
                Favorite getFavorite = JsonConvert.DeserializeObject<Favorite>(getFavoriteResponse.Body.ToString());
                if (getFavorite != null && getFavorite.booksID != null)
                {
                    getFavorite.booksID_temp = getFavorite.booksID.Split(',');
                    //Lấy danh sách sách từ danh sách id sách yêu thích                  
                    ViewBag.listBooksFavorite = new List<Books>();
                    foreach (var booksID in getFavorite.booksID_temp)
                    {
                        ViewBag.listBooksFavorite.Add(Data_Books.GetSingleData(booksID));
                    }
                }
                //Lấy dữ liệu phiếu mượn
                if (Data_CallCard.UpdateCount == false)
                    Data_CallCard.GetAllData();
                //Tìm kiếm các phiếu mượn có id trùng với session id
                List<CallCard> listUserCallCard = new List<CallCard>();
                foreach (var callcard in Data_CallCard.CallcardList)
                {
                    if (callcard.user_id == getUser.id)
                        listUserCallCard.Add(callcard);
                }
                //kiểm tra dữ liệu
                if (listUserCallCard.Count == 0)
                {
                    ViewBag.listCallCard = null;
                }
                else
                {
                    ViewBag.listCallCard = listUserCallCard;
                    //lấy danh sách sách dựa trên id sách trong phiếu mượn                
                    ViewBag.listBooksCallCard = new List<Books>();
                    foreach (var item in listUserCallCard)
                    {
                        item.books_id_temp = item.books_id.Split(',');
                        foreach (var booksID in item.books_id_temp)
                        {
                            ViewBag.listBooksCallCard.Add(Data_Books.GetSingleData(booksID));
                        }
                    }
                }
                return View(getUser);
            }
            else
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            
        }

        public JsonResult SuaThongTin(string id, string fullName, string dateOfBirth, string phone, string email, string gender, string adress, string avatar)
        {
            client = new FireSharp.FirebaseClient(config);
            // get user with user id
            FirebaseResponse getUserResponse = client.Get("User/" + id);
            User user = JsonConvert.DeserializeObject<User>(getUserResponse.Body.ToString());
            user.fullName = fullName;
            user.dateOfBirth = dateOfBirth;
            user.phone = phone;
            user.email = email;
            user.gender = gender;
            user.adress = adress;
            user.avatar = avatar;
            client.Set("User/" + user.id, user);
            return Json(new { status = true });

        }
        public JsonResult DoiPassword(string id, string password, string newpassword, string checknewpass)
        {
            client = new FireSharp.FirebaseClient(config);
            // get user with user id
            FirebaseResponse getUserResponse = client.Get("User/" + id);
            User user = JsonConvert.DeserializeObject<User>(getUserResponse.Body.ToString());

            if (password.Equals(user.password))
            {
                if (newpassword.Equals(checknewpass))
                {
                    user.password = newpassword;
                    client.Set("User/" + user.id, user);
                    return Json(new { status = "Đổi mật khẩu thành công." });
                }
                else
                {
                    return Json(new { status = "Nhập lại mật khẩu không đúng." });
                }
            }
            else
            {
                return Json(new { status = "Mật khẩu không đúng." });
            }
        }
    }
}