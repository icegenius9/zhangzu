using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tests
{
    public class JsonNetActionFilter : IActionFilter
    {
        //在Action执行完成(返回)之后执行，要在Global加filter的启动
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //把filterContext.Result从JsonResult换成JsonNetResult(用json.net写的)
            //filterContext.Result指的就是Action执行返回的ActionResult           
            if (filterContext.Result is JsonResult 
                && !(filterContext.Result is JsonNetResult))
            {
                JsonResult jsonResult = (JsonResult)filterContext.Result;
                JsonNetResult jsonNetResult = new JsonNetResult();
                //属性换一下
                jsonNetResult.ContentEncoding = jsonResult.ContentEncoding;
                jsonNetResult.ContentType = jsonResult.ContentType;
                jsonNetResult.Data = jsonResult.Data;
                jsonNetResult.JsonRequestBehavior = jsonResult.JsonRequestBehavior;
                jsonNetResult.MaxJsonLength = jsonResult.MaxJsonLength;
                jsonNetResult.RecursionLimit = jsonResult.RecursionLimit;

                filterContext.Result = jsonNetResult;

            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
        }
    }
}