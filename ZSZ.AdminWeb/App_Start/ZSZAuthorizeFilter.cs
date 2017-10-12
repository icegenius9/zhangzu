using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.App_Start
{
    /// <summary>
    /// IAuthorizationFilter(controller(方法)执行之前用于权限控制)
    /// </summary>
    public class ZSZAuthorizeFilter : IAuthorizationFilter
    {
        //public IPermissionService permService { get; set; }
        //public IAdminUserService userService { get; set; }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //获得当前要执行的Action上面标注的CheckPermissionAttribute(由于可以标注多个所以返回值为数组)
            CheckPermissionAttribute[] permAtts = (CheckPermissionAttribute[])filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(CheckPermissionAttribute), false);
            //没有标注任何的CheckPermissionAttribute，因此也就不需要检查是否登录
            if (permAtts.Length <= 0)
            {
                return;//登录等这些不要求有用户登录的功能
            }
             //从session中得到当前登入用户的id
            long? userId = (long?)filterContext.HttpContext.Session["LoginUserId"];
            //没有用户登录,就不能访问
            if (userId == null)
            {
                //filterContext.Result = new ContentResult() { Content = "没有登录" };
                //判断客户端发出的请求是不是ajax请求(ajax需要返回json)
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    AjaxResult ajaxResult = new AjaxResult();
                    ajaxResult.Status = "redirect";
                    ajaxResult.Data = "/Main/Login";
                    ajaxResult.ErrorMsg = "没有登录";
                    filterContext.Result = new JsonNetResult { Data = ajaxResult };
                }
                else
                {
                    //跳转到登入页面
                    filterContext.Result = new RedirectResult("~/Main/Login");
                }
                
                return;
            }
            //由于ZSZAuthorizeFilter不是被autofac创建出来的,因此不会自动进行属性注入
            //需要手动获取service对象
            IAdminUserService userService = 
                DependencyResolver.Current.GetService<IAdminUserService>();

            //遍历一下获得的CheckPermissionAttribute数组(检查是否有权限)
            foreach (var permAtt in permAtts)
            {
                //判断登入用户是否具有permAtt.Permission标注的权限(只要碰到任何一个没有的权限就禁止访问)
                //(long)userId等价于userId.Value
                //在IAuthorizationFilter里面，只要修改filterContext.Result,那么真正的Action方法就不会执行了
                if (!userService.HasPermission(userId.Value, permAtt.Permission))
                {

                    //判断客户端发出的请求是不是ajax请求(ajax需要返回json)
                    //根据不同的请求，给予不同的返回格式，确保ajax请求，游览器端也能收到json格式
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        AjaxResult ajaxResult = new AjaxResult();
                        ajaxResult.Status = "error";                        
                        ajaxResult.ErrorMsg = "没有权限"+permAtt.Permission;
                        filterContext.Result = new JsonNetResult { Data = ajaxResult };
                    }
                    else
                    {
                        filterContext.Result =
                      new ContentResult() { Content = "没有" + permAtt.Permission + "这个权限" };

                    }
                        return;
                }
            }
            
        }
    }
}