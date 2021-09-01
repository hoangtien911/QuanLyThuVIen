using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

        [Display(Name = "Năm sinh")]
        [Required(ErrorMessage = "Vui lòng nhập năm sinh")]
        [DataType(DataType.Date)]        
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dateOfBirth { get; set; }
        [Display(Name = "Ngày đăng ký")]
        [Required(ErrorMessage = "Vui lòng nhập ngày đăng ký")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dateOfRegist { get; set; }
    }
}