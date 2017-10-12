using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.App_Start
{
    //AttributeTargets.Method代表权限这个Attribute可以应用到方法上，
    //AllowMultiple = true表示而且可以添加多个
    //使用CheckPermissionAttribute的时候可以不加Attribute
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CheckPermissionAttribute : Attribute
    {
        public string Permission { get; set; }
        public CheckPermissionAttribute(string permission)
        {
            this.Permission = permission;
        }
    }
}