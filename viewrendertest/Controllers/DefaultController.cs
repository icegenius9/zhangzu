using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace viewrendertest.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            //运行/Views/Default/Test.cshtml,把model传给他
            string html = RenderViewToString(this.ControllerContext, "~/Views/Default/Test.cshtml",
                "hello");
            //把html写入到文本中
            System.IO.File.WriteAllText("d:/1.txt", html);
            return Content("ok");
        }
        /// <summary>
        /// 页面静态化的方法取得html的字符串
        /// 运行viewPath的cshtml,把model传进去,返回的是渲染好的html字符串
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewPath">视图页面的地址</param>
        /// <param name="model">视图渲染用到的类</param>
        /// <returns></returns>
        static string RenderViewToString(ControllerContext context,
                string viewPath,
                object model = null)
        {
            ViewEngineResult viewEngineResult =
            ViewEngines.Engines.FindView(context, viewPath, null);//找视图
            if (viewEngineResult == null)//视图找不到
                throw new FileNotFoundException("View" + viewPath + "cannot be found.");
            var view = viewEngineResult.View;//拿到视图
            context.Controller.ViewData.Model = model;//给视图的model赋值
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                context.Controller.ViewData,
                                context.Controller.TempData,
                                sw);//渲染视图
                view.Render(ctx, sw);
                return sw.ToString();
            }
        }

    }
}