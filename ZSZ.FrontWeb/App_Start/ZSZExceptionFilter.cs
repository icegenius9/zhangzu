using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZSZ.FrontWeb.App_Start
{
    //用log4net配合IExceptionFilter处理异常信息，需要配置log4net，在Global配置GlobalFilters.Filters.Add(new ZSZExceptionFilter());
    public class ZSZExceptionFilter : IExceptionFilter
    {
        private static ILog log = LogManager.GetLogger(typeof(ZSZExceptionFilter));
        public void OnException(ExceptionContext filterContext)
        {
            // filterContext.Exception就是异常对象 
            log.ErrorFormat("出现未处理异常:{0}", filterContext.Exception);
        }
    }
}