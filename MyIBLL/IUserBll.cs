using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIBLL
{
   public  interface IUserBll
    {
        bool Check(string username, string pwd);
        void AddNew(string username, string pwd);
    }
}
