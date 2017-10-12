using MyIBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBll
{
    public class UserBll2 : IUserBll
    {
        public void AddNew(string username, string pwd)
        {
            Console.WriteLine("2222 user AddNew ");
        }

        public bool Check(string username, string pwd)
        {
            Console.WriteLine("2222 user check");
            return false;
        }
    }
}
