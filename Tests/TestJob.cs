using Autofac;
using Autofac.Integration.Mvc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIService;

namespace Tests
{
    public class TestJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try {
                //IUserService svc = 
                //    DependencyResolver.Current.GetService<IUserService>();
                //拿到容器

                // 如 果 在 Quartz 等 单 独 的 线 程 中 ， 无 法 通 过
                //DependencyResolver.Current.GetService<ICityService>()获取
                 IUserService svc;
                var container = AutofacDependencyResolver.Current.ApplicationContainer;
                using (container.BeginLifetimeScope())
                {
                     svc = container.Resolve<IUserService>();
                }            
                bool b = svc.CheckLogin("", "");
            }
            catch (Exception ex)
            {

            }

        }
    }
}