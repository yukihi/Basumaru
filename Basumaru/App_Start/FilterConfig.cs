﻿using System.Web;
using System.Web.Mvc;
//testtesttest
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
