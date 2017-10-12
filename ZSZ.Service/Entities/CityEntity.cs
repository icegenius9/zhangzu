using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Service.Entities
{
    /// <summary>
    /// 城市表
    /// </summary>
    public class CityEntity :BaseEntity
    {
        public string Name { get; set; }
    }
}
