using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_SachController : Controller
    {       
        /* Controller Quản lý sách
         * 
         * 1. Danh sách sách
         * 2. Thêm mới sách
         * 3. Sửa thông tin sách
         * 4. Xoá sách
         * 5. Chi tiết sách
         * 6. Lấy danh sách tác giả
         * 7. Lấy danh sách thể loại
         */

        //1. Danh sách sách
        public ActionResult ListBooks(string message)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            if (!Data_Books.UpdateCount)
            {               
                return View(Data_Books.GetAllData());
            }
            else
            {
                return View(Data_Books.BooksList);
            }
        }       
        //2. Thêm mới sách
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            return View();
        }
        [HttpPost]
        public ActionResult Create(Books book)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            if (book.title != null && book.author_temp != null && book.categories_temp != null)
            {
                // gộp mảng tác giả, thể loại sang chuỗi
                book.authors = string.Join(",", book.author_temp);
                book.categories = string.Join(",", book.categories_temp);
                book.author_temp = null;
                book.categories_temp = null;
                book.count_in = book.count;
                //
                Data_Books.CreateData(book);
            }
            return View();
        }
        //3. Sửa thông tin sách
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            Books data = Data_Books.GetSingleData(id);
            //Tách chuỗi tác giả, thể loại sang mảng
            if (data.authors != null)
                data.author_temp = data.authors.Split(',');
            if (data.categories != null)
                data.categories_temp = data.categories.Split(',');
            //
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Books book)
        {
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            if (book.title != null && book.author_temp != null && book.categories_temp != null)
            {
                // gộp mảng tác giả, thể loại sang chuỗi
                book.authors = string.Join(",", book.author_temp);
                book.categories = string.Join(",", book.categories_temp);
                book.author_temp = null;
                book.categories_temp = null;
                //
                Data_Books.EditData(book);
            }
            return RedirectToAction("ListBooks");
        }
        //4. Xoá sách
        public ActionResult Delete(string id)
        {
            Data_Books.DeleteData(id);
            return RedirectToAction("ListBooks");
        }
        //5. Chi tiết sách
        [HttpGet]
        public ActionResult Detail(string id)
        {
            return View(Data_Books.GetSingleData(id));
        }
        //6. Lấy danh sách tác giả
        public List<Author> getAuthor()
        {
            if (!Data_Authors.UpdateCount)
            {
                return (Data_Authors.GetAllData());
            }
            else
            {
                return (Data_Authors.AuthorsList);
            }
        }
        //7. Lấy danh sách thể loại
        public List<Categories> getCategories()
        {
            if (!Data_Categories.UpdateCount)
            {               
                return (Data_Categories.GetAllData());
            }
            else
            {
                return (Data_Categories.CategoriesList);
            }
        }
    }
}