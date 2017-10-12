using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZSZ.CommonMVC;
using ZSZ.FrontWeb.App_Start;
using ZSZ.IService;

namespace ZSZ.FrontWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //启动全局filter(JsonNetActionFilter)
            GlobalFilters.Filters.Add(new JsonNetActionFilter());

            //启动Log4net
            log4net.Config.XmlConfigurator.Configure();


            //创建ZSZExceptionFilter处理异常
            GlobalFilters.Filters.Add(new ZSZExceptionFilter());

            //然后在 Global 中：表示我这个ModelBinders对string类型的数据由我来处理，其他的类型不管
            ModelBinders.Binders.Add(typeof(string), new TrimToDBCModelBinder());
            ModelBinders.Binders.Add(typeof(int),new TrimToDBCModelBinder());
            ModelBinders.Binders.Add(typeof(long), new TrimToDBCModelBinder());
            ModelBinders.Binders.Add(typeof(double), new TrimToDBCModelBinder());


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
            Assembly[] asmService =new Assembly[] { Assembly.Load("ZSZ.Service") };
            //把ZSZ.Service程序集里的类都注册一遍,给属性自动输入实现类 ,SingleInstance()(单例)创建同一个对象
            //.PropertiesAutowired()加上这句，我这个接口可以依赖于别的Service(属性自动的植入)
            //Where(type=>!type.IsAbstract&&typeof(IServiceSupport).IsAssignableFrom(type))这个表示加载ZSZ.Service程序集里的类必须不是抽象类，而且要继承IServiceSupport(标识接口)接口
            //typeof(IServiceSupport).IsAssignableFrom(type) 这个是反射里的，表示typeof(IServiceSupport)类型的变量是否可以指向IsAssignableFrom(type)类型的对象
            //上面的换一种说法：type是否实现了typeof(IServiceSupport)接口，或者typeof(IServiceSupport)是否继承自type
            //typeof(IServiceSupport).IsAssignableFrom(type)只注册实现了IServiceSupport的类，避免其他无关的类注册到AutoFac中
            builder.RegisterAssemblyTypes(asmService)
                .Where(type=>!type.IsAbstract&&typeof(IServiceSupport).IsAssignableFrom(type))
                .AsImplementedInterfaces().PropertiesAutowired();
            //创建一个container出来
            var container = builder.Build();
            //非常重要，把他作为系统的容器，自动从他这获得对象
            //注册系统级别的DependencyResolver，这样当MVC框架创建controller等对象的时候都是管Autofac要对象
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));




            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
