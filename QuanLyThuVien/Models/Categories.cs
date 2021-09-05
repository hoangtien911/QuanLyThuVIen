using System;
using System.ComponentModel.DataAnnotations;


namespace QuanLyThuVien.Models
{
    public class Categories
    {
        [Display(Name = "ID")]
        public string id { get; set; }
        [Display(Name = "Tên gọi")]
        [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
        public string name { get; set; }
        [Display(Name = "Giới thiệu")]
        public string shortDescription { get; set; }
        [Display(Name = "Giới thiệu chi tiết")]
        public string longDescription { get; set; }
        [Display(Name = "Hình ảnh")]
        public string image { get; set; }


    }
}