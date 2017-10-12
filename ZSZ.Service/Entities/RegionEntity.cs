using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 区域
    /// </summary>
    public class RegionEntity :BaseEntity
    {
        /// <summary>
        /// 区域的名字
        /// </summary>
        public string Name { get; set; }
        public long CityId { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public virtual CityEntity City { get; set; }
    }
}
