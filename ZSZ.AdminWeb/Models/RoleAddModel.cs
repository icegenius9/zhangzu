using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.Models
{
    public class RoleAddModel
    {
        /// <summary>
        /// 新增角色的名字
        /// </summary>
        /// 验证
        [Required]
        [StringLength(50)]      
        public string Name { get; set; }
        /// <summary>
        /// 新增角色的权限id
        /// </summary>
        public long[] PermissionIds { get; set; }
    }
}