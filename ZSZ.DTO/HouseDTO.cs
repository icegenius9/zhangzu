using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.DTO
{
    [Serializable]
    public class HouseDTO : BaseDTO
    {
        public long CityId { get; set; }
        public String CityName { get; set; }
        public long RegionId { get; set; }
        public String RegionName { get; set; }
        public long CommunityId { get; set; }
        /// <summary>
        /// 小区的名字
        /// </summary>
        public String CommunityName { get; set; }
        public String CommunityLocation { get; set; }
        public String CommunityTraffic { get; set; }
        public int? CommunityBuiltYear { get; set; }

        public long RoomTypeId { get; set; }
        /// <summary>
        /// 房屋类型
        /// </summary>
        public String RoomTypeName { get; set; }
        public String Address { get; set; }
        /// <summary>
        /// 房屋的月租金
        /// </summary>
        public int MonthRent { get; set; }
        public long StatusId { get; set; }
        public String StatusName { get; set; }
        /// <summary>
        /// 房屋的面积
        /// </summary>
        public decimal Area { get; set; }
        public long DecorateStatusId { get; set; }
        public String DecorateStatusName { get; set; }
        public int TotalFloorCount { get; set; }
        public int FloorIndex { get; set; }
        public long TypeId { get; set; }
        public String TypeName { get; set; }
        public String Direction { get; set; }
        public DateTime LookableDateTime { get; set; }
        public DateTime CheckInDateTime { get; set; }

        public String OwnerName { get; set; }
        public String OwnerPhoneNum { get; set; }
        public String Description { get; set; }
        public long[] AttachmentIds { get; set; }
        /// <summary>
        /// 第一张缩略图的地址(作为预览图)
        /// </summary>
        public String FirstThumbUrl { get; set; }
    }

}
