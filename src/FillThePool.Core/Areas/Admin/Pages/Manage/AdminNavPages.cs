using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FillThePool.Core.Areas.Admin.Pages.Manage
{
    public static class AdminNavPages
    {
        public static string Index => "Index";

		public static string Users => "Users";

		public static string Instructors => "Instructors";

        public static string Pools => "Pools";

        public static string Schedules => "Schedules";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

		public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

		public static string InstructorsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Instructors);

        public static string PoolsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Pools);

        public static string SchedulesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Schedules);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}