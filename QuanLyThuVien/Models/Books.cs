using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Models
{
    public class Books
    {
        [Display(Name = "ID")]
        public string _id { get; set; }
        [Display(Name = "Tên sách")]
        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        public string title { get; set; }
        [Display(Name = "Tác giả")]   
        public string authors { get; set; }
        [Display(Name = "Thể loại")]      
        public string categories { get; set; }
        [Display(Name = "Giới thiệu")]    
        public string shortDescription { get; set; }
        [Display(Name = "Giới thiệu chi tiết")]      
        public string longDescription { get; set; }
        [Display(Name = "Số lượng")]       
        public int count { get; set; }
        [Display(Name = "Ngày phát hành")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime publishedDate { get; set; }
        [Display(Name = "Trạng thái")]       
        public string status { get; set; }
        [Display(Name = "Ảnh bìa")]       
        public string thumbnailUrl { get; set; }

        public string [] author_temp { get; set; }
        public string[] categories_temp { get; set; }
    }
}