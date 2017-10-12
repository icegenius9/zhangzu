using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 后台管理员表
    /// </summary>
    public class AdminUserEntity:BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNum { get; set; }
        /// <summary>
        /// 盐+密码的MD5
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// 随机的盐
        /// </summary>
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 城市的id
        /// </summary>
        public long? CityId { get; set; }

        public virtual CityEntity City { get; set; }
        
        public int LoginErrorTimes { get; set; }
        public DateTime? LastLoginErrorDateTime { get; set; }
        //角色
        public virtual ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
    }
}
