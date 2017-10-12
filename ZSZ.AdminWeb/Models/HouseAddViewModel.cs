using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.AdminWeb.Models
{
    public class HouseAddViewModel
    {
        /// <summary>
        /// 地区
        /// </summary>
        public RegionDTO[] regions { get; set; }
        /// <summary>
        /// 房屋类型
        /// </summary>
        public IdNameDTO[] roomTypes { get; set; }
        public IdNameDTO[] statuses { get; set; }
        /// <summary>
        /// 装修状态
        /// </summary>
        public IdNameDTO[] decorateStatuses { get; set; }
        public IdNameDTO[] types { get; set; }
        /// <summary>
        /// 可选配套设施
        /// </summary>
        public AttachmentDTO[] attachments { get; set; }
    }
}