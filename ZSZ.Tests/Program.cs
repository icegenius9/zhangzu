using Autofac;
using MyBll;
using MyIBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZSZ.CommonMVC;
using ZSZ.Service;

namespace ZSZ.Tests
{
    class Program
    {
        //手机发送短信验证码，调用平台接口
        static void Main(string[] args)
        {
            //using (ZSZDbContext ct=new ZSZDbContext())
            //{
            //    ct.Database.Delete();
            //    ct.Database.Create();
            //}
            //Console.WriteLine("OK");
                string userName = "icegenius";
            string appKey = "8b4de3b31534437b416aaa";
            string templateId = "326";
            string code = "6666";
            string phoneNum = "18918918189";
            /*
                        WebClient wc = new WebClient();
                        string url = "http://sms.rupeng.cn/SendSms.ashx?userName=" +
                            Uri.EscapeDataString(userName) + "&appKey=" + Uri.EscapeDataString(appKey) +
                            "&templateId=" + templateId + "&code=" + Uri.EscapeDataString(code) +
                            "&phoneNum=" + phoneNum;
                        wc.Encoding = Encoding.UTF8;
                        string resp = wc.DownloadString(url);
                        //发出url这样一个http请求（Get请求）返回值为响应报文体
                        Console.WriteLine(resp);
                        */
            SMSSender sender = new SMSSender();
            sender.AppKey = appKey;
            sender.UserName = userName;
            var result = sender.SendSMS(templateId, code, phoneNum);
            Console.WriteLine("返回码：" + result.code + ",消息：" + result.msg);
            Console.WriteLine("ok");
            Console.ReadKey();

        }
            //IOC容器,atofac演示
            static void Main1(string[] args)
        {
            //最土的调用
            //UserBll userbll = new UserBll();
            //基于接口编程
            //IUserBll userbll = new UserBll();
            //userbll.AddNew("aaa", "123");
            
            //Autofac
            //先创建一个ContainerBuilder容器构建者
            ContainerBuilder builder = new ContainerBuilder();

            //演化1：注册IUserBll是IUserBll接口的实现类
            //builder.RegisterType<UserBll>().As<IUserBll>();
            //演化2：把UserBll注册为所有实现类，比如UserBll类实现了好几个接口，那么Resolve任何一个接口，都会返回我的对象
            //builder.RegisterType<UserBll>().AsImplementedInterfaces();
            //builder.RegisterType<DogBll>().AsImplementedInterfaces();
            //演化3：首先拿到所有实现类所在程序集的名字
            Assembly asm = Assembly.Load("MyBll");
            //把MyBll程序集里的类都注册一遍
            //builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces();
            //把MyBll程序集里的类都注册一遍,给属性自动输入实现类 ,SingleInstance()(单例)创建同一个对象
            builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces().PropertiesAutowired().SingleInstance();

            IContainer container = builder.Build();
            //Resolve获得实现类UserBll,创建IUserBll实现类的对象
            //IUserBll bll = container.Resolve<IUserBll>();
            //Console.WriteLine(bll.GetType());
            //bll.AddNew("aa", "123");
            //如果IUserBll有多个实现类，把实现类放到一个集合中
            //IEnumerable<IUserBll> blls= container.Resolve<IEnumerable<IUserBll>>();
            //foreach (IUserBll bll in blls)
            //{
            //    Console.WriteLine(bll.GetType());
            //    bll.AddNew("aaa","333");
            //}


            //IDogBll dogbll=container.Resolve<IDogBll>();
            //dogbll.bark();

            ISchool school = container.Resolve<ISchool>();
            school.FangXue();

            ISchool school1 = container.Resolve<ISchool>();
            //加了SingleInstance() 判断school和school1是不是同一个引用(对象)
            Console.WriteLine(object.ReferenceEquals(school,school1));

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
