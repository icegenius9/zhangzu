using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.FrontWeb.Controllers
{
    public class UserController : Controller
    {
        public ISettingService settingService { get; set; }
        public IUserService userService { get; set; }

        /// <summary>
        /// 找回密码的视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        /// <summary>
        /// 点击下一步
        /// </summary>
        /// <param name="phoneNum">手机号</param>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword(string phoneNum,
            string verifyCode)
        {
            //从TempData获得图形验证码
            string serverVerifyCode = (string)TempData["verifyCode"];
            if (serverVerifyCode != verifyCode)
            {
                return Json(new AjaxResult
                {
                    Status = "error",
                    ErrorMsg = "验证码错误"
                });
            }
            var user = userService.GetByPhoneNum(phoneNum);
            if (user == null)
            {
                return Json(new AjaxResult
                {
                    Status = "error",
                    ErrorMsg = "没有这个手机号"
                });
            }
            string appKey = settingService.GetValue("如鹏短信平台AppKey");
            string userName = settingService.GetValue("如鹏短信平台UserName");
            string tempId = settingService.GetValue("如鹏短信平台找回密码短信模板Id");
            //生成一个4位的短信验证码
            string smsCode = new Random().Next(1000, 9999).ToString();
            //发送短信
            SMSSender smsSender = new SMSSender();
            smsSender.AppKey = appKey;
            smsSender.UserName = userName;
            var sendResult = smsSender.SendSMS(tempId, smsCode, phoneNum);
            //如果发送成功
            if (sendResult.code == 0)
            {
                //把发送短信的时候存入TempData
                TempData["ForgotPasswordPhoneNum"] = phoneNum;
                //发送给手机的验证码也存入TempData
                TempData["SmsCode"] = smsCode;
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult
                {
                    Status = "error",
                    ErrorMsg = sendResult.msg
                });
            }
        }
        /// <summary>
        /// 输入手机短信验证码页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword2()
        {
            return View();
        }
        /// <summary>
        /// 点击验证
        /// </summary>
        /// <param name="smsCode">手机短信验证码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword2(string smsCode)
        {
            string serverSmsCode = (string)TempData["SmsCode"];
            //如果输入的手机验证码和TempData中存的不一样
            if (smsCode != serverSmsCode)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "短信验证码错误" });
            }
            else //如果输入的手机验证码和TempData中存的一样
            {
                //告诉第3步“短信验证码验证通过”，防止恶意用户跳过ForgotPassword2直接重置密码
                TempData["ForgotPassword2_OK"] = true;
                return Json(new AjaxResult
                {
                    Status = "ok"
                });
            }
        }
        /// <summary>
        /// 修改密码的页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword3()
        {
            return View();
        }
        /// <summary>
        /// 点击修改密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword3(string password)
        {
            //防止恶意用户跳过ForgotPassword2直接重置密码
            bool? is2_OK = (bool?)TempData["ForgotPassword2_OK"];
            if (is2_OK != true)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "您没有通过短信验证码的验证" });
            }

            //需要重置密码的手机号
            string phoneNum = (string)TempData["ForgotPasswordPhoneNum"];
            var user = userService.GetByPhoneNum(phoneNum);
            userService.UpdatePwd(user.Id, password);
            return Json(new AjaxResult { Status = "ok" });
        }

        [HttpGet]
        public ActionResult ForgotPassword4()
        {
            return View();
        }
    }
}