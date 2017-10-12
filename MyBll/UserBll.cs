using MyIBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBll
{
    public class UserBll : IUserBll
    {
        public void AddNew(string username, string pwd)
        {
            Console.WriteLine("新增用户 username="+username+"   pwd="+pwd);
        }

        public bool Check(string username, string pwd)
        {
            Console.WriteLine("检查登录，username="+username);
            return true;
        }
    }
}
