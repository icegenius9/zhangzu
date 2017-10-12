using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 小区的状态
    /// </summary>
    public class IdNameEntity :BaseEntity
    {   /// <summary>
       /// 类别(装修状态，房屋状态)
       /// </summary>
        public string TypeName { get; set; }
        public string Name { get; set; }
    }
}
