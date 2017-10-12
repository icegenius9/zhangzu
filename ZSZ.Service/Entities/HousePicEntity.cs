using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 房子的图片
    /// </summary>
    public class HousePicEntity :BaseEntity
    {
        /// <summary>
        /// 房子的id
        /// </summary>
        public long HouseId { get; set; }
        public virtual HouseEntity House { get; set; }
        /// <summary>
        /// 大图片的地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 缩略图的地址
        /// </summary>
        public string ThumbUrl { get; set; }
    }
}
