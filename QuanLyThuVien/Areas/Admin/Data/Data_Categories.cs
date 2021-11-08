using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using Newtonsoft.Json;

namespace QuanLyThuVien.Areas.Admin.Data
{
    public static class Data_Categories
    {
        private static IFirebaseClient client;
        private static IFirebaseConfig config = new FirebaseConfig
        {
            BasePath = "https://libmanagerdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/",
            AuthSecret = "Sxg7VD8YEx8nLTf7SJSSFK8c4ZWfKzvBokW1uw25"
        };
        public static int UpdateCount = 0;

        public static List<Categories> CategoriesList = new List<Categories>();

        public static void GetAllData()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Categories");
            Dictionary<string, Categories> data = JsonConvert.DeserializeObject<Dictionary<string, Categories>>(response.Body.ToString());

            foreach (var item in data)
            {
                Categories categories = new Categories();
                categories.id = item.Value.id;
                categories.name = item.Value.name;
                categories.shortDescription = item.Value.shortDescription;
                categories.longDescription = item.Value.longDescription;
                categories.image = item.Value.image;
                CategoriesList.Add(categories);
            }
            UpdateCount++;
        }
        public static Categories GetSingleData(string id)
        {
            Categories data = null;
            if (UpdateCount == 0)
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Categories/" + id);
                data = JsonConvert.DeserializeObject<Categories>(response.Body);
            }
            else
            {
                foreach (var item in CategoriesList)
                {
                    if (id == item.id)
                    {
                        data = item;
                        break;
                    }
                }
            }
            return data;
        }
        public static void CreateData(Categories categories)
        {
            CategoriesList.Add(categories);

            client = new FireSharp.FirebaseClient(config);
            PushResponse response = client.Push("Categories/", categories);
            categories.id = response.Result.name;
            client.Set("Categories/" + categories.id, categories);       
        }
        public static void EditData(Categories categories)
        {
            client = new FireSharp.FirebaseClient(config);
            client.Set("Categories/" + categories.id, categories);

            int index = CategoriesList.FindIndex(cate => cate.id.Equals(categories.id));
            CategoriesList.Insert(index, categories);
            CategoriesList.RemoveAt(index + 1);
        }
        public static void DeleteData(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            client.Delete("Categories/" + id);

            int index = CategoriesList.FindIndex(cate => cate.id.Equals(id));
            CategoriesList.RemoveAt(index);
        }
    }
}