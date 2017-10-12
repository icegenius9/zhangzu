using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 房屋预约(预约看房)
    /// </summary>
    public class HouseAppointmentEntity :BaseEntity
    {
        /// <summary>
        /// 预约房屋的用户id(可能没有登录就预约)
        /// </summary>
        public long? UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 预约用户的电话
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 预约的时间
        /// </summary>
        public DateTime VisitDate { get; set; }
        /// <summary>
        /// 预约房子的id
        /// </summary>
        public long HouseId { get; set; }
        public virtual HouseEntity House { get; set; }
        /// <summary>
        /// 预约房子的状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 抢单管理员的id
        /// </summary>
        public long? FollowAdminUserId { get; set; }
        public virtual AdminUserEntity FollowAdminUser { get; set; }
        /// <summary>
        /// 抢单时间
        /// </summary>
        public DateTime? FollowDateTime { get; set; }
        /// <summary>
        /// 用于乐观锁,数据库中是timestamp类型
        /// </summary>
        public byte[] RowVersion { get; set; }
    }
}
