using Autofac.Integration.Mvc;
using Autofac;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.IService;
using System.Text;
using System.Net.Mail;

namespace ZSZ.AdminWeb.Jobs
{
    /// <summary>
    /// 给老板定时发报表到邮件 Quartz
    /// </summary>
    public class BossReportJob : IJob
    {
        //创建log4net对象
        private static ILog log
            = LogManager.GetLogger(typeof(BossReportJob));

        public void Execute(IJobExecutionContext context)
        {
            log.Debug("准备收集今日新增房源数量");
            //在Execute方法中发生异常,由于在单独线程中,不会被global中的ZExceptionFilter所获得,因此要try,catch单独的抓住异常
            try
            {
                string bossEmails;//老板邮箱
                string smtpServer, smtpUserName, smtpPassword, smtpEmail;
                StringBuilder sbMsg = new StringBuilder();
                //在 Quartz这类单独的线程用AutofacDependencyResolver.Current.ApplicationContainer方法创建autofac
                var container = AutofacDependencyResolver.Current.ApplicationContainer;
                using (container.BeginLifetimeScope())
                {
                    var cityService = container.Resolve<ICityService>();
                    var houseService = container.Resolve<IHouseService>();
                    var settingService = container.Resolve<ISettingService>();
                    bossEmails = settingService.GetValue("老板邮箱");//读取配置中的老板邮箱 
                    //取得配置表中smtp服务器的地址
                    smtpServer = settingService.GetValue("SmtpServer");
                    //取得配置表中发送者的用户名
                    smtpUserName = settingService.GetValue("SmtpUserName");
                    //取得配置表中发送者的(密码)授权码
                    smtpPassword = settingService.GetValue("SmtpPassword");
                    //取得配置表中发送者的邮箱
                    smtpEmail = settingService.GetValue("SmtpEmail");
                    foreach (var city in cityService.GetAll())
                    {
                        //获得每个城市的每天新增的房源数量
                        long count = houseService.GetTodayNewHouseCount(city.Id);
                        sbMsg.Append(city.Name).Append("新增房源的数量是：")
                            .Append(count).AppendLine();
                    }
                }
                log.Debug("收集新增房源数量完成" + sbMsg);
                //要使用System.Net.Mail下的类，不要用System.Web.Mail下的类
                using (MailMessage mailMessage = new MailMessage())
                using (SmtpClient smtpClient = new SmtpClient(smtpServer))
                {
                    //由于可能有多个老板都想收到这个邮件，因此在配置中可以用分号
                    //分隔开各个老板的邮箱地址
                    foreach (var bossEmail in bossEmails.Split(';'))
                    {
                        mailMessage.To.Add(bossEmail);
                    }
                    mailMessage.Body = sbMsg.ToString();
                    mailMessage.From = new MailAddress(smtpEmail);
                    mailMessage.Subject = "今日新增房源数量报表";
                    smtpClient.Credentials
                        = new System.Net.NetworkCredential(smtpUserName, smtpPassword);//如果启用了“客户端授权码”，要用授权码代替密码
                    smtpClient.Send(mailMessage);
                }
                log.Debug("给老板发送新增房源数量报表完成");
            }
            catch (Exception ex)
            {
                log.Error("给老板发报表出错", ex);
            }
        }
    }
}