using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {    
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PageNotFound(string status)
        {
            ViewBag.status = status;
            return View();
        }
    }
}