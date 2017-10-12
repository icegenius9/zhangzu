using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.AdminWeb.Models
{
    /// <summary>
    /// 修改角色的viewmodel
    /// </summary>
    public class RoleEditGetModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public RoleDTO role{get;set;}
        /// <summary>
        /// 角色拥有的权限
        /// </summary>
        public PermissionDTO[] RolePerms{get;set;}
        /// <summary>
        /// 所有的权限
        /// </summary>
        public PermissionDTO[] AllPerms { get; set; }
    }
}