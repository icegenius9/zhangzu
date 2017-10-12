using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 锁
{
    public class Girl
    {
        
        public long Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 男朋友的名字
        /// </summary>
        public string BF { get; set; }
        /// <summary>
        /// 类型为timestamp的时间戳定义为byte[]类型(乐观锁用)
        /// </summary>
        public byte[] rowver { get; set; }
    }
}
