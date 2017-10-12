using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb
{
    public class AdminHelper
    {
        //通过session获得用户的id(为什么要写这个方法，因为session中用的是字符串，容易写错，使用方法没这个问题了),
        //之前在内核(asp.net)中用的是HttpContext,在MVC中用HttpContextBase这个类
        public static long? GetUserId(HttpContextBase ctx)
        {
            return (long?)ctx.Session["LoginUserId"];
        }
    }
}