using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 房子
    /// </summary>
    public class HouseEntity:BaseEntity
    {
        /// <summary>
        /// 小区的id
        /// </summary>
        public long CommunityId { get; set; }
        /// <summary>
        /// 小区
        /// </summary>
        public virtual CommunityEntity Community { get; set; }
        /// <summary>
        /// 房屋的类别(二室一厅)
        /// </summary>
        public long RoomTypeId { get; set; }
        
        public virtual IdNameEntity RoomType { get; set; }
        /// <summary>
        /// 几号楼
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 月租金
        /// </summary>
        public int MonthRent { get; set; }
        /// <summary>
        /// 房子的状态(有人住)
        /// </summary>
        public long StatusId { get; set; }
        
        public virtual IdNameEntity Status { get; set; }
        /// <summary>
        /// 房子的面积
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// 装修的状态id
        /// </summary>
        public long DecorateStatusId { get; set; }
       
        public virtual IdNameEntity DecorateStatus { get; set; }
        /// <summary>
        /// 一共几层楼
        /// </summary>
        public int TotalFloorCount { get; set; }
        /// <summary>
        /// 第几层
        /// </summary>
        public int FloorIndex { get; set; }
        /// <summary>
        /// 房屋的类别id(整租)
        /// </summary>
        public long TypeId { get; set; }
       
        public virtual IdNameEntity Type { get; set; }
        /// <summary>
        /// 朝向(朝南)
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// 什么时候可以看房
        /// </summary>
        public DateTime LookableDateTime { get; set; }
        public DateTime CheckInDateTime { get; set; }
        /// <summary>
        /// 房东名字
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 房东手机号
        /// </summary>
        public string OwnerPhoneNum { get; set; }
        /// <summary>
        /// 房子的描述
        /// </summary>
        public string Description { get; set; }

        public virtual ICollection<AttachmentEntity> Attachments { get; set; } = new List<AttachmentEntity>();
        public virtual ICollection<HousePicEntity> HousePics { get; set; } = new List<HousePicEntity>();
    }
}
