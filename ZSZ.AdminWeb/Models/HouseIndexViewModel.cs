using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.AdminWeb.Models
{
    /// <summary>
    /// 为了(房子详情)页面静态化复制了前台一个一模一样的类
    /// </summary>
    public class HouseIndexViewModel
    {
        public HouseDTO House { get; set; }
        public HousePicDTO[] Pics { get; set; }
        public AttachmentDTO[] Attachments { get; set; }
    }
}