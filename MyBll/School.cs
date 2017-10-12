using MyIBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBll
{
    public class School : ISchool
    {
        //需要给属性也注入
        public IDogBll dogbll { get; set; }
        public void FangXue()
        {
            //需要给属性也注入builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces().PropertiesAutowired();
            dogbll.bark();
            Console.WriteLine("放学啦");
        }
    }
}
