using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Models
{
    public class Author
    {
        [Display(Name = "ID")]
        public string id { get; set; }
        [Display(Name = "Tên tác giả")]
        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        public string name { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Website")]      
        public string website { get; set; }
        [Display(Name = "Ghi chú")]   
        public string note { get; set; }
        [Display(Name = "Hình ảnh")]
        public string avatar { get; set; }
    }
}