using CaptchaGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.Models;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    public class MainController : Controller
    {
        public IAdminUserService userService { get; set; }

        public ActionResult Index()
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            if (userId == null)
            {
                return Redirect("~/Main/Login");
            }
            var user = userService.GetById((long)userId);
            return View(user);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Abandon();//销毁Session
            return Redirect("~/Main/Login");
        }

        // GET: Main
        [HttpGet]
        public ActionResult Login()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            //使用ModelState检测验证码是否为空(防止前端被人代码注释，多次请求)
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult
                {
                    Status = "error",
                    ErrorMsg = MVCHelper.GetValidMsg(ModelState)
                });
            }

            //输入的验证码和TempData中的验证码是否匹配(不使用session)
            if (model.VerifyCode != TempData["verifyCode"].ToString())
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误" });
            }
            //用户名和密码的检测
            bool result=userService.CheckLogin(model.PhoneNum, model.Password);
            if (result)
            {
                //登录成功吧当前登入用户的id存在session里面(给后面检查"当前Session登录的这个用户有没有***的权限")
               Session["LoginUserId"]=userService.GetByPhoneNum(model.PhoneNum).Id;
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult { Status = "error",ErrorMsg="用户名或者密码错误" });
            }
        }
        /// <summary>
        /// 生成登入验证码的action(已File的形式返回)
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateVerityCode()
        {
            //创建4位验证码
            string verityCode = CommonHelper.CreateVerifyCode(4);
            //把验证码放入session中
            //Session["verifyCode"] = verityCode;

            //把验证码放入TempData中(取一次就销毁,下次取就为空，安全)(不使用session)
            TempData["verifyCode"] = verityCode;

            //通过CaptchaGen图形验证码创建流文件
            MemoryStream ms = ImageFactory.GenerateImage(verityCode, 60, 100, 20, 6);
            //返回一个文件类型(流文件,文件格式)
            return File(ms, "image/jpeg");
            
            //不用using asp.netMVC会自动释放
            //using (MemoryStream ms = ImageFactory.GenerateImage(cerityCode, 60, 100, 20, 6))            
            //{
            //    //返回一个文件类型(流文件,文件格式)
            //    return File(ms, "image/jpeg");
            //}
        }
    }
}