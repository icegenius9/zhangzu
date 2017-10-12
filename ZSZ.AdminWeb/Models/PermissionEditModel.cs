using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.Models
{
    /// <summary>
    /// 修改权限的类
    /// </summary>
    public class PermissionEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}