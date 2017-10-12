using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestIService;

namespace TestService
{
    public class UserService : IUserService
    {
        //.PropertiesAutowired()加上这句(属性注入)，我这个接口可以依赖于别的Service
        public INewsService newService { get; set; }
        public bool CheckLogin(string username, string password)
        {
            newService.AddNews(username, password);
            return true;
        }

        public bool CheckUserNameExists(string username)
        {
            return false;
        }
    }
}
