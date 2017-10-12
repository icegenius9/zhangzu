using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 锁
{
    class GirlConfig : EntityTypeConfiguration<Girl>
    {
        public GirlConfig()
        {
            //表名
            ToTable("T_Girls");
            //把数据库中rowver列的属性设定为IsRowVersion类型(为timestamp时间戳)
            Property(g => g.rowver).IsRowVersion();
        }
    }
}
