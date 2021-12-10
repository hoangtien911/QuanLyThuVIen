using System.Collections.Generic;
using System.Linq;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using Firebase.Auth;
using Firebase.Storage;
using System.Threading.Tasks;
using System;
using System.Web;

namespace QuanLyThuVien.Areas.Admin.Data
{
    public static class Data_Books
    {
        /* Class Data Books (Lưu dữ liệu - Làm việc với database)
         * 
         * 1. Cấu hình
         * 2. Biến dữ liệu
         * 3. Lấy tất cả dữ liệu sách
         * 4. Lấy dữ liệu 1 sách theo id
         * 5. Thêm mới 1 sách
         * 6. Sửa thông tin 1 sách
         * 7. Xoá 1 sách
         * 8. Kiểm tra trạng thái
         * 9. Kiểm tra sách đã tồn tại hay chưa
         */

        //1. Config
        private static IFirebaseClient client;
        private static IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //2. Biến dữ liệu
        public static bool UpdateCount = false;
        public static List<Books> BooksList = new List<Books>();
        //3. Lấy tất cả dữ liệu sách
        public static List<Books> GetAllData()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Books");
            Dictionary<string, Books> data = JsonConvert.DeserializeObject<Dictionary<string, Books>>(response.Body.ToString());

            for(int i = data.Count-1; i >= 0; i--)
            {
                var item = data.ElementAt(i);
                Books book = new Books();
                book._id = item.Value._id;
                book.title = item.Value.title;
                book.authors = item.Value.authors;
                book.categories = item.Value.categories;
                book.shortDescription = item.Value.shortDescription;
                book.longDescription = item.Value.longDescription;
                book.count = item.Value.count;
                book.count_in = item.Value.count_in;
                book.publishedDate = item.Value.publishedDate;
                book.status = item.Value.status;
                book.thumbnailUrl = item.Value.thumbnailUrl;
                TestStatus(book);
                BooksList.Add(book);
            }
            UpdateCount = true;
            return BooksList;
        }
        //4. Lấy dữ liệu 1 sách theo id
        public static Books GetSingleData(string id)
        {
            Books data = new Books();
            if (!UpdateCount)
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Books/" + id);
                data = JsonConvert.DeserializeObject<Books>(response.Body);
                
            }
            else
            {
                int index = BooksList.FindIndex(book => book._id.Equals(id));
                data = BooksList.ElementAt(index);                
            }
            TestStatus(data);
            return data;
        }
        //5. Thêm mới 1 sách
        public static bool CreateData(Books book)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }
            //Kiểm tra
            TestStatus(book);
            if(!CheckBook(book, ""))
            {
                return false;
            }
            //
            client = new FireSharp.FirebaseClient(config);
            PushResponse response = client.Push("Books/", book);
            book._id = response.Result.name;
            Thread t1 = new Thread(() =>
            {
                BooksList.Insert(0,book);
            });
            Thread t2 = new Thread(() =>
            {             
                client.Set("Books/" + book._id, book);
            });            
            t1.Start();
            t2.Start();           
            return true;
        }
        //6. Sửa thông tin 1 sách
        public static bool EditData(Books books)
        {           
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in BooksList)
            {
                if (books._id.Equals(item._id))
                {
                    //kiểm tra tồn tại
                    TestStatus(books);
                    if (!CheckBook(books, item._id)){
                        return false;
                    }                   
                    //
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Set("Books/" + books._id, books);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = BooksList.FindIndex(bk => bk._id.Equals(books._id));
                        BooksList.Insert(index, books);
                        BooksList.RemoveAt(index + 1);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //7. Xoá 1 sách
        public static bool DeleteData(string id)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }
            foreach (var item in BooksList)
            {
                if (id.Equals(item._id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Delete("Books/" + id);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = BooksList.FindIndex(bk => bk._id.Equals(id));
                        BooksList.RemoveAt(index);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //8. Kiểm tra trạng thái
        private static void TestStatus(Books b)
        {
            if (b.count_in == 0)
            {
                b.status = "Hết sách";
            }
            if (b.count_in > 0 && b.count_in <= b.count)
            {
                b.status = "Còn sách";
            }
        } 
        //9. Kiểm tra sách đã tồn tại hay chưa
        private static bool CheckBook(Books b, string id)
        {
            foreach (var item in BooksList)
            {
                if (b.title.Equals(item.title))
                {
                    if(item._id == id){
                        continue;
                    }
                    else
                        return false;
                }
            }
            return true;
        }      
    }
}