using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Models
{
    public class User
    {
        [Display(Name = "ID")]
        public string id { get; set; }
        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string username { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        public string email { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string password { get; set; }       
        [Display(Name = "Giới tính")]
        public string gender { get; set; }
        [Display(Name = "Họ tên")]
        public string fullName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string adress { get; set; }
        [Display(Name = "Số điện thoại")]
        public string phone { get; set; }     
        [Display(Name = "Ảnh đại diện")]
        public string avatar { get; set; }
        [Display(Name = "Trạng thái")]
        public string status { get; set; }      
        public string dateOfBirth { get; set; }      
        public string dateOfRegist { get; set; }
        public bool remember { get; set; }
    }
}