using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.FrontWeb.Models
{
    /// <summary>
    /// 搜索页面的ViewModel
    /// </summary>
    public class HouseSearchViewModel
    {
        /// <summary>
        /// 所有的区域
        /// </summary>
        public RegionDTO[] regions { get; set; }
        /// <summary>
        /// 搜索出来的房子结果
        /// </summary>
        public HouseDTO[] houses { get; set; }
    }
}