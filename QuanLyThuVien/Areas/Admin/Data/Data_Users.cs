﻿using System.Collections.Generic;
using System.Linq;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using QuanLyThuVien.Models;
using Newtonsoft.Json;
using System.Threading;

namespace QuanLyThuVien.Areas.Admin.Data
{
    public static class Data_Users
    {
        /* Class Data Users (Lưu dữ liệu - Làm việc với database)
         * 
         * 1. Cấu hình
         * 2. Biến dữ liệu
         * 3. Lấy tất cả dữ liệu người dùng
         * 4. Lấy dữ liệu 1 người dùng theo id
         * 5. Thêm mới 1 người dùng
         * 6. Sửa thông tin 1 người dùng
         * 7. Xoá 1 người dùng
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
        public static List<User> UserList = new List<User>();
        //3, Lấy tất cả dữ liệu người dùng
        public static List<User> GetAllData()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("User");
            Dictionary<string, User> data = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Body.ToString());
           
            foreach (var item in data)
            {
                User user = new User();
                user.id = item.Value.id;
                user.username = item.Value.username;
                user.email = item.Value.email;
                user.password = item.Value.password;
                user.fullName = item.Value.fullName;
                user.dateOfBirth = item.Value.dateOfBirth;
                user.gender = item.Value.gender;
                user.adress = item.Value.adress;
                user.phone = item.Value.phone;
                user.dateOfRegist = item.Value.dateOfRegist;
                user.avatar = item.Value.avatar;
                user.status = item.Value.status;
                UserList.Add(user);
            }
            UpdateCount = true;
            return UserList;
        }
        //4. Lấy dữ liệu 1 người dùng theo id
        public static User GetSingleData(string id)
        {
            User data = null;
            if (!UpdateCount)
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("User/" + id);
                data = JsonConvert.DeserializeObject<User>(response.Body);
            }
            else
            {
                int index = UserList.FindIndex(urs => urs.id.Equals(id));
                data = UserList.ElementAt(index);
            }
            return data;
        }
        //5. Thêm mới 1 người dùng
        public static bool CreateData(User user)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in UserList)
            {
                if (user.username.Equals(item.username))
                {
                    return false;
                }
            }
            Thread t1 = new Thread(() =>
            {
                UserList.Add(user);
            });
            Thread t2 = new Thread(() =>
            {
                client = new FireSharp.FirebaseClient(config);
                PushResponse response = client.Push("User/", user);
                user.id = response.Result.name;
                client.Set("User/" + user.id, user);
            });
            t1.Start();
            t2.Start();
            return true;
        }
        //6. Sửa thông tin 1 người dùng
        public static bool EditData(User user)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in UserList)
            {
                if (user.id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Set("User/" + user.id, user);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = UserList.FindIndex(urs => urs.id.Equals(user.id));
                        UserList.Insert(index, user);
                        UserList.RemoveAt(index + 1);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //7. Xoá 1 người dùng
        public static bool DeleteData(string id)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in UserList)
            {
                if (id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Delete("User/" + id);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = UserList.FindIndex(urs => urs.id.Equals(id));
                        UserList.RemoveAt(index);
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