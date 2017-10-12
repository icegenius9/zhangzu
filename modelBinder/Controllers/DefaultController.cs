using modelBinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace modelBinder.Controllers
{
    public class DefaultController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //ModelBinder能处理Model方式
        [HttpPost]
        public ActionResult Index(User model)
        {
            //model.UserName = ToDBC(model.UserName.Trim());
            //model.Password = ToDBC(model.Password.Trim());

            if (model.UserName == "admin" && model.Password == "123")
            {
                return Content("成功");
            }
            else
            {
                return Content("错误");
            }
        }

        /*ModelBinder能处理普通参数
        [HttpPost]
        public ActionResult Index(string userName,string password)
        {
            //model.UserName = ToDBC(model.UserName.Trim());
            // model.Password = ToDBC(model.Password.Trim());

            if (userName == "admin" && password == "123")
            {
                return Content("成功");
            }
            else
            {
                return Content("错误");
            }
        }*/

        //ModelBinder不能处理FormCollection方式
        //[HttpPost]
        //public ActionResult Index(FormCollection fc)
        //{
        //    //model.UserName = ToDBC(model.UserName.Trim());
        //    // model.Password = ToDBC(model.Password.Trim());
        //    string userName = fc["UserName"];
        //    string password = fc["Password"];
        //    if (userName == "admin" && password == "123")
        //    {
        //        return Content("成功");
        //    }
        //    else
        //    {
        //        return Content("错误");
        //    }
        //}
    }
}