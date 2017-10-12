using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.DTO
{
    [Serializable]
    public class HousePicDTO : BaseDTO
    {
        public long HouseId { get; set; }
        /// <summary>
        /// 原图(可能带水印)
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public String ThumbUrl { get; set; }
    }

}
