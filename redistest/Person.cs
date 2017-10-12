using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redistest
{
    //Redis是使用json序列化,不需要标记为可序列化
    class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
