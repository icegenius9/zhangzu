using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 小区
    /// </summary>
    public class CommunityEntity : BaseEntity
    {
        /// <summary>
        /// 小区的名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 小区属于哪个区域
        /// </summary>
        public long RegionId { set; get; }

        public virtual RegionEntity Region { set; get; }

        /// <summary>
        /// 小区的地址
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 小区的交通信息
        /// </summary>
        public string Traffic { get; set; }
        /// <summary>
        /// 小区建造年份
        /// </summary>
        public int? BuiltYear { get; set; }
    }
}
