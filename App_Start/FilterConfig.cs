using System.Web;
using System.Web.Mvc;

namespace INATEL_T141_DM106_Final_Homework
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
