using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyThuVien.Models
{
    public class CallCard
    {
        [Display(Name = "ID")]
        public string id { get; set; }
        [Display(Name = "Người mượn")]
        [Required(ErrorMessage = "Vui lòng nhập người mượn")]
        public string user_id { get; set; }
        [Display(Name = "Sách mượn")]
        [Required(ErrorMessage = "Vui lòng nhập sách mượn")]
        public string books_id { get; set; }
        [Display(Name = "Ngày mượn")]
        public string date_issued { get; set; }
        [Display(Name = "Ngày hẹn trả")]
        public string date_return { get; set; }
        [Display(Name = "Ngày trả")]        
        public string date_returned { get; set; }
        [Display(Name = "Trạng thái")]
        public string status { get; set; }
        public string [] books_id_temp { get; set; }
        public string username { get; set; }
    }
}