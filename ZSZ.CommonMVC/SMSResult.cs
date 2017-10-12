using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.CommonMVC
{
    //短信接口返回的信息
   public class SMSResult
    {
        //返回的编码
        public int code { get; set; }
        //返回的消息
        public string msg { get; set; }
    }
}
