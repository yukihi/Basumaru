using System.Web;
using System.Web.Mvc;
//testtesttestt
namespace Basumaru
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
