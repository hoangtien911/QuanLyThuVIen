using System.Collections.Generic;
using System.Linq;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using Newtonsoft.Json;
using System.Threading;

namespace QuanLyThuVien.Areas.Admin.Data
{
    public static class Data_Authors
    {
        /* Class Data Authors (Lưu dữ liệu - Làm việc với database)
         * 
         * 1. Cấu hình
         * 2. Biến dữ liệu
         * 3. Lấy tất cả dữ liệu tác giả
         * 4. Lấy dữ liệu 1 tác giả theo id
         * 5. Thêm mới 1 tác giả
         * 6. Sửa thông tin 1 tác giả
         * 7. Xoá 1 tác giả        
         */

        //1. Cấu hình
        private static IFirebaseClient client;
        private static IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        //2. Biến dữ liệu
        public static bool UpdateCount = false;
        public static List<Author> AuthorsList = new List<Author>();
        //3, Lấy tất cả dữ liệu tác giả
        public static List<Author> GetAllData()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Author");
            Dictionary<string, Author> data = JsonConvert.DeserializeObject<Dictionary<string, Author>>(response.Body.ToString());

            foreach (var item in data)
            {
                Author author = new Author();
                author.id = item.Value.id;
                author.email = item.Value.email;
                author.website = item.Value.website;
                author.name = item.Value.name;
                author.note = item.Value.note;
                author.avatar = item.Value.avatar;
                AuthorsList.Add(author);
            }
            UpdateCount = true;
            return AuthorsList;
        }
        //4. Lấy dữ liệu 1 tác giả theo id
        public static Author GetSingleData(string id)
        {
            Author data = null;
            if (!UpdateCount)
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Author/" + id);
                data = JsonConvert.DeserializeObject<Author>(response.Body);
            }
            else
            {
                int index = AuthorsList.FindIndex(ath => ath.id.Equals(id));
                data = AuthorsList.ElementAt(index);
            }
            return data;
        }
        //5. Thêm mới 1 tác giả
        public static bool CreateData(Author author)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in AuthorsList)
            {
                if (author.name.Equals(item.name))
                {
                    return false;
                }
            }
            Thread t1 = new Thread(() =>
            {
                AuthorsList.Add(author);
            });
            Thread t2 = new Thread(() =>
            {
                client = new FireSharp.FirebaseClient(config);
                PushResponse response = client.Push("Author/", author);
                author.id = response.Result.name;
                client.Set("Author/" + author.id, author);
            });
            t1.Start();
            t2.Start();
            return true;
        }
        //6. Sửa thông tin 1 tác giả
        public static bool EditData(Author author)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in AuthorsList)
            {
                if (author.id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Set("Author/" + author.id, author);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = AuthorsList.FindIndex(ath => ath.id.Equals(author.id));
                        AuthorsList.Insert(index, author);
                        AuthorsList.RemoveAt(index + 1);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //7. Xoá 1 tác giả
        public static bool DeleteData(string id)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in AuthorsList)
            {
                if (id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Delete("Author/" + id);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = AuthorsList.FindIndex(ath => ath.id.Equals(id));
                        AuthorsList.RemoveAt(index);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }

            }
            return false;
        }
    }
}