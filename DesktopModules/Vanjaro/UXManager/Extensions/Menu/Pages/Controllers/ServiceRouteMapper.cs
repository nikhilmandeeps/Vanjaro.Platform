﻿using DotNetNuke.Web.Api;

namespace Vanjaro.UXManager.Extensions.Menu.Pages.Controllers
{
    public class ServiceRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("Pages", "default", "{controller}/{action}", new[] { "Vanjaro.UXManager.Extensions.Menu.Pages.Controllers" });
        }
    }
}