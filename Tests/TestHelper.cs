using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIService;

namespace Tests
{
    public class TestHelper
    {
        
        public static void Test()
        {
            // 手动解析DependencyResolver.Current.GetService<IUserService>();手动拿到DependencyResolver实现类对对象
            IUserService svc = DependencyResolver.Current.GetService<IUserService>();
            bool b = svc.CheckLogin("", "");

        }

    }
}