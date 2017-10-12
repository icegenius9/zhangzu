using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIService;

namespace Tests.Controllers
{
    public class DefaultController : Controller
    {
        //因为Global中注入了PropertiesAutowired()属性注入，所以会给controller的属性自动赋值
        public IUserService IuserService{get;set;}
        // GET: Default
        public ActionResult Index()
        {
            //bool b=IuserService.CheckLogin("abc", "123");
            ////手动注入
            //TestHelper.Test();       
            //return Content(b.ToString());

            //手动解析只有Autofac帮我们创建的对象才有可能给我们自动进行属性的赋值PropertiesAutowired()
            Person p1 =DependencyResolver.Current.GetService<Person>();
            
            p1.Hello();
            return Content("ok");
        }

        [HttpGet]
        public ActionResult TestJson()
        {
            return View();
        }

        //json.net的学习，以及使用json.net和IActionFilter结合
        [HttpPost]
        public ActionResult TestJson(FormCollection fc)
        {
            Dog dog = new Dog() { BirthDate=DateTime.Now,Id=5,Name="旺财"};

            //使用json.net和IActionFilter结合，返回结果为使用json,net创建类(JsonNetResult)
            return Json(dog);

            //使用json.net返回json
            //return new JsonNetResult { Data = dog };


        }
        //实现分页在View调用调用ZSZ.CommonMVC 类库的MyPager类完成分页
        public ActionResult Pager1()
        {
            return View();
        }
    }
}