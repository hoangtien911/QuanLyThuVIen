using System.Web.Mvc;

namespace QuanLyThuVien.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin",
                "Admin/{controller}/{action}/{id}",
                new { controller = "ThongKe", action = "ThongKe", id = UrlParameter.Optional }
            );
        }
    }
}