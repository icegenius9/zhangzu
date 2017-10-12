using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.FrontWeb.Models
{
    /// <summary>
    /// 房源信息的viewmodel
    /// </summary>
    //一个类被标记为可序列化,这个类关联的其他类也要被标记为可序列化
    [Serializable]
    public class HouseIndexViewModel
    {
        /// <summary>
        /// 房子的信息
        /// </summary>
        public HouseDTO House { get; set; }
        /// <summary>
        /// 房子的图片
        /// </summary>
        public HousePicDTO[] Pics { get; set; }
        /// <summary>
        /// 房子的配套设施
        /// </summary>
        public AttachmentDTO[] Attachments { get; set; }
    }
}