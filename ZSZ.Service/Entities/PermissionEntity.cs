using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class PermissionEntity :BaseEntity
    {
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        public virtual ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
    }
}
