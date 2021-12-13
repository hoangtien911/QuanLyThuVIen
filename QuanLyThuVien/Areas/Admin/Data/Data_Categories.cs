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
    public static class Data_Categories
    {
        /* Class Data Categories (Lưu dữ liệu - Làm việc với database)
         * 
         * 1. Cấu hình
         * 2. Biến dữ liệu
         * 3. Lấy tất cả dữ liệu thể loại
         * 4. Lấy dữ liệu 1 thể loại theo id
         * 5. Thêm mới 1 thể loại
         * 6. Sửa thông tin 1 thể loại
         * 7. Xoá 1 thể loại      
         * 8. Kiểm tra thể loại đã tồn tại hay chưa
         * 9. Kiểm tra dữ liệu
         */

        //1. Cấu hình
        private static IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        private static IFirebaseClient client = new FireSharp.FirebaseClient(config);
        //2. Biến dữ liệu
        public static bool UpdateCount = false;
        public static List<Categories> CategoriesList = new List<Categories>();
        //3. Lấy tất cả dữ liệu thể loại
        public static List<Categories> GetAllData()
        {           
            FirebaseResponse response = client.Get("Categories");
            Dictionary<string, Categories> data = JsonConvert.DeserializeObject<Dictionary<string, Categories>>(response.Body.ToString());


            for (int i = data.Count - 1; i >= 0; i--)
            {
                var item = data.ElementAt(i);
                Categories categories = new Categories();
                categories.id = item.Value.id;
                categories.name = item.Value.name;
                categories.shortDescription = item.Value.shortDescription;
                categories.longDescription = item.Value.longDescription;
                categories.image = item.Value.image;
                CheckData(categories);
                CategoriesList.Add(categories);
            }
           
            UpdateCount = true;
            return CategoriesList;
        }
        //4. Lấy dữ liệu 1 thể loại theo id
        public static Categories GetSingleData(string id)
        {
            Categories data = new Categories();
            if (!UpdateCount)
            {
                GetAllData();
                FirebaseResponse response = client.Get("Categories/" + id);
                data = JsonConvert.DeserializeObject<Categories>(response.Body);
            }
            else
            {
                int index = CategoriesList.FindIndex(cate => cate.id.Equals(id));
                data = CategoriesList.ElementAt(index);
            }
           
            return data;
        }
        //5. Thêm mới 1 thể loại
        public static bool CreateData(Categories categories)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }
            if (!CheckCategories(categories, ""))
                return false;
            CheckData(categories);
            PushResponse response = client.Push("Categories/", categories);
            categories.id = response.Result.name;
            Thread t1 = new Thread(() =>
            {
                CategoriesList.Insert(0,categories);
            });
            Thread t2 = new Thread(() =>
            {                              
                client.Set("Categories/" + categories.id, categories);
            });
            t1.Start();
            t2.Start();
            return true;
        }
        //6. Sửa thông tin 1 thể loại
        public static bool EditData(Categories categories)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }
            foreach (var item in CategoriesList)
            {
                if (categories.id.Equals(item.id))
                {
                    if (!CheckCategories(categories, categories.id))
                        return false;
                    CheckData(categories);
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Set("Categories/" + categories.id, categories);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = CategoriesList.FindIndex(cate => cate.id.Equals(categories.id));
                        CategoriesList.Insert(index, categories);
                        CategoriesList.RemoveAt(index + 1);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //7. Xoá 1 thể loại
        public static bool DeleteData(string id)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in CategoriesList)
            {
                if (id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Delete("Categories/" + id);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = CategoriesList.FindIndex(cate => cate.id.Equals(id));
                        CategoriesList.RemoveAt(index);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //8. Kiểm tra thể loại đã tồn tại hay chưa
        private static bool CheckCategories(Categories cate, string id)
        {
            foreach (var item in CategoriesList)
            {
                if (cate.name.Equals(item.name))
                {
                    if (item.id == id)
                    {
                        continue;
                    }
                    else
                        return false;
                }
            }
            return true;
        }
        //9. Kiểm tra dữ liệu
        private static void CheckData(Categories cate)
        {
            if (cate.shortDescription == null)
                cate.shortDescription = "Không có thông tin.";
            if (cate.longDescription == null)
                cate.longDescription = "Không có thông tin.";           
        }
    }
}