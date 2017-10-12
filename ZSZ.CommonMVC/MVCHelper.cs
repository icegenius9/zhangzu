using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ZSZ.CommonMVC
{
    public class MVCHelper
    {
        //校验表单信息ModelState
        // 有 两 个ModelStateDictionary 类，别弄混乱了，先引用system.web.MVC 在用 using System.Web.Mvc; 下的
        public static string GetValidMsg(ModelStateDictionary modelState)
                                                                         
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ms in modelState.Values)
            {
                foreach (var modelError in ms.Errors)
                {
                    sb.AppendLine(modelError.ErrorMessage);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 把NameValueCollection(键值对)转换成querystring(拼成字符串)
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string ToQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            //遍历NameValueCollection的每一个键
            foreach (var key in nvc.AllKeys)
            {
                //获取键对应的值
                string value = nvc[key];
                //EscapeDataString就是对特殊字符进行uri编码
                sb.Append(key).Append("=")
                    .Append(Uri.EscapeDataString(value)).Append("&");
            }
            return sb.ToString().Trim('&');//去掉最后一个多余的&
        }

        /// <summary>
        /// 从QueryString中移除掉一部分
        /// </summary>
        /// <param name="nvc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RemoveQueryString(NameValueCollection nvc, string name)
        {
            //拷贝一份NameValueCollection
            NameValueCollection newNVC = new NameValueCollection(nvc);
            //拷贝出来NameValueCollection去掉包含name的键值对
            newNVC.Remove(name);
            //将移除掉的NameValueCollection转换成QueryString返回
            return ToQueryString(newNVC);
        }
        /// <summary>
        /// 更新QueryString,如果包含就更新，没有就增加
        /// </summary>
        /// <param name="nvc">键值对的集合</param>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string UpdateQueryString(NameValueCollection nvc,
            string name, string value)
        {
            //拷贝一份NameValueCollection
            NameValueCollection newNVC = new NameValueCollection(nvc);
            //如果拷贝出来的NameValueCollection中包含name的键,则将这个键的值更改为value
            if (newNVC.AllKeys.Contains(name))
            {
                newNVC[name] = value;
            }
            else
            {
                //如果拷贝出来的NameValueCollection中不包含name的键，则增加一个键为name，值为value的键值对
                newNVC.Add(name, value);
            }
            //将NameValueCollection转换成QueryString返回
            return ToQueryString(newNVC);
        }

        /// <summary>
        /// 页面静态化需要,把cshtml渲染生成取得html的字符串
        /// 运行viewPath的cshtml,把model传进去,返回的是渲染好的html字符串
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewPath">视图页面的地址</param>
        /// <param name="model">视图渲染用到的类</param>
        /// <returns></returns>
        public static string RenderViewToString(ControllerContext context,
                string viewPath,
                object model = null)
        {
            ViewEngineResult viewEngineResult =
            ViewEngines.Engines.FindView(context, viewPath, null);
            if (viewEngineResult == null)
                throw new FileNotFoundException("View" + viewPath + "cannot be found.");
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                context.Controller.ViewData,
                                context.Controller.TempData,
                                sw);
                view.Render(ctx, sw);
                return sw.ToString();
            }
        }

    }
}
