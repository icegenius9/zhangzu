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

namespace Tests
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //启动全局filter(JsonNetActionFilter)
            GlobalFilters.Filters.Add(new JsonNetActionFilter());

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
            Assembly asmService = Assembly.Load("TestService");
            //把MyBll程序集里的类都注册一遍,给属性自动输入实现类 ,SingleInstance()(单例)创建同一个对象
            //.PropertiesAutowired()加上这句，我这个接口可以依赖于别的Service
            builder.RegisterAssemblyTypes(asmService).AsImplementedInterfaces().PropertiesAutowired();


            //创建一个container出来
            var container = builder.Build();
            //非常重要，把他作为系统的容器，自动从他这获得对象
            //注册系统级别的DependencyResolver，这样当MVC框架创建controller等对象的时候都是管Autofac要对象
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //程序启动的时候执行Quartz
            //IScheduler sched = new StdSchedulerFactory().GetScheduler();
            //JobDetailImpl jdBossReport = new JobDetailImpl("jdTest", typeof(TestJob));
            ////IMutableTrigger triggerBossReport = CronScheduleBuilder.DailyAtHourAndMinute(23, 45).Build();//每天23:45执行一次
            //CalendarIntervalScheduleBuilder scbuilder = CalendarIntervalScheduleBuilder.Create();
            //scbuilder.WithInterval(5, IntervalUnit.Second);//每5秒钟执行一次
            //IMutableTrigger triggerBossReport = scbuilder.Build();
            //triggerBossReport.Key = new TriggerKey("triggerTest");
            //sched.ScheduleJob(jdBossReport, triggerBossReport);
            //sched.Start();
        }
    }
}
