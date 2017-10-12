using MyIBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBll
{
    public class DogBll : IDogBll
    {
        public void bark()
        {
            Console.WriteLine("汪汪汪");
        }
    }
}
