using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;

namespace ZSZ.CommonMVC
{
    //邮件发送
   public class SMSSender
    {
        //参数用户名
        public string UserName { get; set; }
        //参数
        public String AppKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateId">短信模板的id</param>
        /// <param name="code">短信的验证码</param>
        /// <param name="phoneNum">发到哪个手机号</param>
        /// <returns></returns>
        public SMSResult SendSMS(string templateId, string code, string phoneNum)
        {
            //调用http请求的类
            WebClient wc = new WebClient();
            //Uri.EscapeDataString是用来进行urlencode
            string url = "http://sms.rupeng.cn/SendSms.ashx?userName=" +
                Uri.EscapeDataString(UserName) + "&appKey=" + Uri.EscapeDataString(AppKey) +
                "&templateId=" + Uri.EscapeDataString(templateId)
                + "&code=" + Uri.EscapeDataString(code) +
                "&phoneNum=" + Uri.EscapeDataString(phoneNum);
            //设置返回的编码类型，不然可能会乱码
            wc.Encoding = Encoding.UTF8;
            //发送url这样一个http请求(get请求)返回值为响应报文体
            string resp = wc.DownloadString(url);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //系列化返回的结果
            SMSResult result = jss.Deserialize<SMSResult>(resp);
            return result;
        }
    }
}
