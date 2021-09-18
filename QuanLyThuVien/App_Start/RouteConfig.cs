﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuanLyThuVien
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "TrangChu", action = "HomePage", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Areas",
                url: "{area}/{controller}/{action}/{id}",
                defaults: new { area = "Admin", controller = "TrangChu", action = "HomePage", id = UrlParameter.Optional }
            );
        }
    }
}
