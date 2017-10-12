using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace memcachedtest
{
    //标注为可序列化
    [Serializable]
    class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
