using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien.Models
{
    public class Favorite
    {
        public string userID { get; set; }
        public string booksID { get; set; }
        public string [] booksID_temp { get; set; }
    }
}