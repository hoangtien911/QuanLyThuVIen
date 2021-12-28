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
    public class Data_CallCard
    {
        /* Class Data CallCard (Lưu dữ liệu - Làm việc với database)
         * 
         * 1. Cấu hình
         * 2. Biến dữ liệu
         * 3. Lấy tất cả dữ liệu phiếu mượn
         * 4. Lấy dữ liệu 1 phiếu mượn theo id
         * 5. Thêm mới 1 phiếu mượn
         * 6. Sửa thông tin 1 phiếu mượn
         * 7. Xoá 1 phiếu mượn
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
        public static List<CallCard> CallcardList = new List<CallCard>();
        //3, Lấy tất cả dữ liệu phiếu mượn
        public static List<CallCard> GetAllData()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("CallCard");
            Dictionary<string, CallCard> data = JsonConvert.DeserializeObject<Dictionary<string, CallCard>>(response.Body.ToString());

            //for ()
            foreach (var item in data)
            {
                CallCard callCard = new CallCard();
                callCard.id = item.Value.id;
                callCard.books_id = item.Value.books_id;
                callCard.user_id = item.Value.user_id;
                callCard.date_issued = item.Value.date_issued;
                callCard.date_return = item.Value.date_return;
                callCard.date_returned = item.Value.date_returned;
                callCard.status = item.Value.status;
                CallcardList.Add(callCard);
            }
            UpdateCount = true;
            return CallcardList;
        }
        //4. Lấy dữ liệu 1 phiếu mượn theo id
        public static CallCard GetSingleData(string id)
        {
            CallCard data = null;
            if (!UpdateCount)
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("CallCard/" + id);
                data = JsonConvert.DeserializeObject<CallCard>(response.Body);
            }
            else
            {
                int index = CallcardList.FindIndex(ccard => ccard.id.Equals(id));
                data = CallcardList.ElementAt(index);
            }
            return data;
        }
        //5. Thêm mới 1 phiếu mượn
        public static void CreateData(CallCard card)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }          
            Thread t1 = new Thread(() =>
            {
                CallcardList.Add(card);
            });
            Thread t2 = new Thread(() =>
            {
                client = new FireSharp.FirebaseClient(config);
                PushResponse response = client.Push("CallCard/", card);
                card.id = response.Result.name;
                client.Set("CallCard/" + card.id, card);
            });
            t1.Start();
            t2.Start();        
        }
        //6. Sửa thông tin 1 phiếu mượn
        public static bool EditData(CallCard callCard)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in CallcardList)
            {
                if (callCard.id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Set("CallCard/" + callCard.id, callCard);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = CallcardList.FindIndex(ccard => ccard.id.Equals(callCard.id));
                        CallcardList.Insert(index, callCard);
                        CallcardList.RemoveAt(index + 1);
                    });
                    t1.Start();
                    t2.Start();
                    return true;
                }
            }
            return false;
        }
        //7. Xoá 1 phiếu mượn
        public static bool DeleteData(string id)
        {
            if (!UpdateCount)
            {
                GetAllData();
            }

            foreach (var item in CallcardList)
            {
                if (id.Equals(item.id))
                {
                    Thread t1 = new Thread(() =>
                    {
                        client = new FireSharp.FirebaseClient(config);
                        client.Delete("CallCard/" + id);
                    });
                    Thread t2 = new Thread(() =>
                    {
                        int index = CallcardList.FindIndex(ccard => ccard.id.Equals(id));
                        CallcardList.RemoveAt(index);
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