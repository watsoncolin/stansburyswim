using System.Web.Mvc;
using FillThePool.Web.Filters;

namespace FillThePool.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}