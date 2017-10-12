using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestIService;

namespace Tests
{
    public class Person
    {
        public IUserService userSvs { get; set; }
        public void Hello()
        {
            userSvs.CheckLogin("", "");
        }
    }
}