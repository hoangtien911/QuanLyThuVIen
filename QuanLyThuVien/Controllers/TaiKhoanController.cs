using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;

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
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            Dictionary<string, User> data = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Body.ToString());
            User loginUser = new User();
            bool check = false;
            foreach (var item in data)
            {
                if (item.Value.username.Equals(user.username))
                {
                    loginUser.id = item.Value.id;
                    loginUser.username = item.Value.username;
                    loginUser.email = item.Value.email;
                    loginUser.password = item.Value.password;
                    loginUser.fullName = item.Value.fullName;
                    loginUser.dateOfBirth = item.Value.dateOfBirth;
                    loginUser.gender = item.Value.gender;
                    loginUser.adress = item.Value.adress;
                    loginUser.phone = item.Value.phone;
                    loginUser.dateOfRegist = item.Value.dateOfRegist;
                    loginUser.avatar = item.Value.avatar;
                    loginUser.status = item.Value.status;
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                ViewBag.thongbao = "Tên tài khoản không tồn tại.";
                return View();
            }
            if (!loginUser.password.Equals(user.password))
            {
                ViewBag.thongbao = "Mật khẩu không đúng";
                return View();
            }
            else if (loginUser.password.Equals(user.password))
            {
                if (loginUser.status.Equals("Admin"))
                {
                    Session["AdminSession"] = loginUser.id;                     
                    Response.Cookies["AdminCookies"]["id"] = loginUser.id;
                    Response.Cookies["AdminCookies"]["name"] = loginUser.fullName;
                    Response.Cookies["AdminCookies"]["avatar"] = loginUser.avatar;
                    return RedirectToAction("ThongKe", "ThongKe", new { area = "Admin" });
                }
                if (loginUser.status.Equals("User"))
                {
                    Session["UserSession"] = loginUser.id;
                    Response.Cookies["UserCookies"]["username"] = loginUser.username;
                    Response.Cookies["UserCookies"]["userid"] = loginUser.id;
                    if (user.remember)
                    {                      
                        Response.Cookies["UserCookies"]["password"] = loginUser.password;
                        Response.Cookies["UserCookies"].Expires = DateTime.Now.AddDays(15);
                    }
                    return RedirectToAction("HomePage", "TrangChu");
                }

            }
            return View();
        }

        /// <summary> Đăng xuất </summary>   
        [HttpGet]
        public ActionResult DangXuat()
        {
            Session["UserSession"] = null;
            Session["AdminSession"] = null;
            Response.Cookies["UserCookies"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["AdminCookies"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("HomePage", "TrangChu");
        }
        /// <summary> Chi tiết thông tin cá nhân </summary>   
        public ActionResult ChiTietUser(string id)
        {
            //check session user
            if (Session["UserSession"] == null)
                return RedirectToAction("DangNhap", "TaiKhoan");
            //Config and call response
            client = new FireSharp.FirebaseClient(config);

            // get user with user id
            FirebaseResponse getUserResponse = client.Get("User/" + id);
            User getUser = JsonConvert.DeserializeObject<User>(getUserResponse.Body.ToString());

            FirebaseResponse getCallCardResponse = client.Get("CallCard/");
            FirebaseResponse getFavoriteResponse = client.Get("Favorite/" + getUser.id);

            //get favorite with user id
            Favorite getFavorite = JsonConvert.DeserializeObject<Favorite>(getFavoriteResponse.Body.ToString());
            if (getFavorite != null && getFavorite.booksID != null)
            {
                getFavorite.booksID_temp = getFavorite.booksID.Split(',');
                //get books with favorite id
                ViewBag.listBooksFavorite = new List<Books>();
                foreach (var booksID in getFavorite.booksID_temp)
                {
                    FirebaseResponse getBook = client.Get("Books/" + booksID);
                    ViewBag.listBooksFavorite.Add(JsonConvert.DeserializeObject<Books>(getBook.Body.ToString()));
                }
            }
            // get callcard == user.id
            Dictionary<string, CallCard> data = JsonConvert.DeserializeObject<Dictionary<string, CallCard>>(getCallCardResponse.Body.ToString());
            if (data != null)
            {
                List<CallCard> listCallCard = new List<CallCard>();
                foreach (var item in data)
                {
                    if (item.Value.user_id.Equals(getUser.id))
                    {
                        CallCard callCard = new CallCard();
                        callCard.id = item.Value.id;
                        callCard.books_id = item.Value.books_id;
                        callCard.user_id = item.Value.user_id;
                        callCard.date_issued = item.Value.date_issued;
                        callCard.date_return = item.Value.date_return;
                        callCard.date_returned = item.Value.date_returned;
                        callCard.status = item.Value.status;
                        listCallCard.Add(callCard);
                    }
                }
                if(listCallCard.Count == 0)
                {
                    ViewBag.listCallCard = null;
                }
                else
                {
                    ViewBag.listCallCard = listCallCard;
                    //get books with callcard books_id                    
                    ViewBag.listBooksCallCard = new List<Books>();
                    foreach (var item in listCallCard)
                    {
                        item.books_id_temp = item.books_id.Split(',');
                        foreach (var booksID in item.books_id_temp)
                        {
                            FirebaseResponse getBook = client.Get("Books/" + booksID);
                            ViewBag.listBooksCallCard.Add(JsonConvert.DeserializeObject<Books>(getBook.Body.ToString()));
                        }
                    }
                }
                
            }
            return View(getUser);
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