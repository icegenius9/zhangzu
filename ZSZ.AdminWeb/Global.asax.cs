using Autofac;
using Autofac.Integration.Mvc;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Jobs;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //第一个用户，第一次进入才会执行
        protected void Application_Start()
        {
            //启动全局filter(JsonNetActionFilter)
            GlobalFilters.Filters.Add(new JsonNetActionFilter());

            //启动Log4net
            log4net.Config.XmlConfigurator.Configure();

            //创建ZSZExceptionFilter处理异常
            GlobalFilters.Filters.Add(new ZSZExceptionFilter());

            //创建IAuthorizationFilter(权限控制)
            GlobalFilters.Filters.Add(new ZSZAuthorizeFilter());

            //先创建一个ContainerBuilder容器构建者  
            var builder = new ContainerBuilder();
            //RegisterControllers在这个Autofac.Integration.Mvc程序集中
            //(需要把哪个程序集放入autofac中)把当前程序集中的Controller都注册到Autofac中 一种写法：获得MvcApplication类所在的程序集
            //PropertiesAutowired()属性注入
            builder.RegisterControllers(typeof(MvcApplication).Assembly)
                .PropertiesAutowired();
            //把MvcApplication程序集中所有的类都注册一遍
            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly)
                .PropertiesAutowired().PropertiesAutowired();
            //拿到所有实现类所在程序集的名字
            Assembly[] asmService = new Assembly[] { Assembly.Load("ZSZ.Service") };
            //把ZSZ.Service程序集里的类都注册一遍,给属性自动输入实现类 ,SingleInstance()(单例)创建同一个对象
            //.PropertiesAutowired()加上这句，我这个接口可以依赖于别的Service(属性自动的植入)
            //Where(type=>!type.IsAbstract&&typeof(IServiceSupport).IsAssignableFrom(type))这个表示加载ZSZ.Service程序集里的类必须不是抽象类，而且要继承IServiceSupport(标识接口)接口
            //typeof(IServiceSupport).IsAssignableFrom(type) 这个是反射里的，表示typeof(IServiceSupport)类型的变量是否可以指向IsAssignableFrom(type)类型的对象
            //上面的换一种说法：type是否实现了typeof(IServiceSupport)接口，或者typeof(IServiceSupport)是否继承自type
            //typeof(IServiceSupport).IsAssignableFrom(type)只注册实现了IServiceSupport的类，避免其他无关的类注册到AutoFac中
            builder.RegisterAssemblyTypes(asmService)
                .Where(type => !type.IsAbstract && typeof(IServiceSupport).IsAssignableFrom(type))
                .AsImplementedInterfaces().PropertiesAutowired();
            //创建一个container出来
            var container = builder.Build();
            //非常重要，把他作为系统的容器，自动从他这获得对象
            //注册系统级别的DependencyResolver，这样当MVC框架创建controller等对象的时候都是管Autofac要对象
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //启动给老板发送报表的job
            //startQuartz();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        /// <summary>
        /// 给老板定时发送报表的job
        /// </summary>
        private void startQuartz()
        {
            IScheduler sched = new StdSchedulerFactory().GetScheduler();

            //给老板的报表开始,每一个job弄下面这块,如果建多个job这个"jdBossReport"名词不同就可以
            JobDetailImpl jdBossReport
                = new JobDetailImpl("jdBossReport", typeof(BossReportJob));
            IMutableTrigger triggerBossReport
                = CronScheduleBuilder.DailyAtHourAndMinute(2, 24).Build();//每天23:45执行一次
            triggerBossReport.Key = new TriggerKey("triggerBossReport");
            sched.ScheduleJob(jdBossReport, triggerBossReport);
            //给老板的报表结束


            sched.Start();

        }
    }
}
