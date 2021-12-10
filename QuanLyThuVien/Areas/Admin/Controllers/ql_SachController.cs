using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyThuVien.Models;
using QuanLyThuVien.Areas.Admin.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace QuanLyThuVien.Areas.Admin.Controllers
{
    public class ql_SachController : Controller
    {
        /* Controller Quản lý sách
         * 
         * 1. Danh sách sách
         * 2. Thêm mới và sửa sách
         * 3. Lấy thông tin sách cần xoá
         * 4. Lấy thông tin sách cần sửa
         * 5. Xoá sách
         * 6. Lấy danh sách tác giả
         * 7. Lấy danh sách thể loại
         */

        //1. Danh sách sách
        [HttpGet]
        public JsonResult GetListBook()
        {
            if (!Data_Books.UpdateCount)
                Data_Books.GetAllData();
            //lấy tên sách và tên thể loại theo id
            foreach (var book in Data_Books.BooksList)
            {
                if (book.authors != null)
                {
                    book.author_temp = book.authors.Split(',');
                    string[] authorName = new string[book.author_temp.Length];
                    for (int i = 0; i < book.author_temp.Length; i++)
                    {
                        authorName[i] = Data_Authors.GetSingleData(book.author_temp[i]).name;
                    }
                    book.author_temp = authorName;
                }

                if (book.categories != null)
                {
                    book.categories_temp = book.categories.Split(',');
                    string[] cateName = new string[book.categories_temp.Length];
                    for (int i = 0; i < book.categories_temp.Length; i++)
                    {
                        cateName[i] = Data_Categories.GetSingleData(book.categories_temp[i]).name;
                    }
                    book.categories_temp = cateName;
                }
            }
            return Json(Data_Books.BooksList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListBooks(string message)
        {
            if (!Data_Books.UpdateCount)
                Data_Books.GetAllData();
            //lấy tên sách và tên thể loại theo id
            foreach (var book in Data_Books.BooksList)
            {
                if (book.authors != null)
                {
                    book.author_temp = book.authors.Split(',');
                    string[] authorName = new string[book.author_temp.Length];
                    for (int i = 0; i < book.author_temp.Length; i++)
                    {
                        authorName[i] = Data_Authors.GetSingleData(book.author_temp[i]).name;
                    }
                    book.author_temp = authorName;
                }

                if (book.categories != null)
                {
                    book.categories_temp = book.categories.Split(',');
                    string[] cateName = new string[book.categories_temp.Length];
                    for (int i = 0; i < book.categories_temp.Length; i++)
                    {
                        cateName[i] = Data_Categories.GetSingleData(book.categories_temp[i]).name;
                    }
                    book.categories_temp = cateName;
                }
            }
            ViewBag.BooksList = Data_Books.BooksList;
            ViewBag.listAuthor = getAuthor();
            ViewBag.listCategories = getCategories();
            return View();

        }
        //2. Thêm mới và sửa sách       
        [HttpPost]
        public JsonResult Create_Edit(Books book)
        {
            if (book.title != null && book.author_temp != null && book.categories_temp != null)
            {
                // gộp mảng tác giả, thể loại sang chuỗi
                book.authors = string.Join(",", book.author_temp);
                book.categories = string.Join(",", book.categories_temp);
                book.author_temp = null;
                book.categories_temp = null;                             
                //
                if(book._id == null)
                {
                    book.count_in = book.count;
                    if (Data_Books.CreateData(book))
                    {
                        return Json(new { status = "ADD_OK" });
                    }
                    else
                    {
                        return Json(new { status = "ADD_FALSE" });
                    }
                }
                else
                {                    
                    if (Data_Books.EditData(book))
                    {
                        return Json(new { status = "EDIT_OK"});
                    }
                    else
                    {
                        return Json(new { status = "EDIT_FALSE" });
                    }
                }
                       
            }
            else
            {
                string propetyName = "";
                if (book.title == null)
                    propetyName += "Tên sách, ";
                if (book.author_temp == null)
                    propetyName += "Tác giả, ";
                if (book.categories_temp == null)
                    propetyName += "Thể loại, ";
                return Json(new { status = "NO_INPUT_DATA", propery = propetyName });
            }


        }
        //3. Lấy thông tin sách cần xoá
        [HttpGet]
        public JsonResult GetBookDelete(string id)
        {
            Books book = Data_Books.GetSingleData(id);
            if (book != null)
            {

                book.author_temp = book.authors.Split(',');
                string[] authorName = new string[book.author_temp.Length];
                for (int i = 0; i < book.author_temp.Length; i++)
                {
                    authorName[i] = Data_Authors.GetSingleData(book.author_temp[i]).name;
                }
                book.author_temp = authorName;
                        
                book.categories_temp = book.categories.Split(',');
                string[] cateName = new string[book.categories_temp.Length];
                for (int i = 0; i < book.categories_temp.Length; i++)
                {
                    cateName[i] = Data_Categories.GetSingleData(book.categories_temp[i]).name;
                }
                book.categories_temp = cateName;

                return Json(book, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        //4. Lấy thông tin sách cần sửa
        public JsonResult GetBook(string id)
        {
            Books books = Data_Books.GetSingleData(id);
            if (books != null)
            {

                books.author_temp = books.authors.Split(',');
                books.categories_temp = books.categories.Split(',');

                return Json(books, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }      
        //5. Xoá sách
        [HttpPost]
        public JsonResult Delete(string id)
        {
            if (Data_Books.DeleteData(id))
            {
                return Json(new { status = "DELETE_OK" });
            }
            else
            {
                return Json(new { status = "DELETE_FALSE" });
            }
            
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