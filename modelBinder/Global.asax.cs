using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace modelBinder
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //然后在 Global 中：表示我这个ModelBinders对string类型的数据由我来处理，其他的类型不管
            ModelBinders.Binders.Add(typeof(string),new TrimToDBCModelBinder());
        }
    }
}
