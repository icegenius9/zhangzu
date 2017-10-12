using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    /// <summary>
    /// 预约看房管理
    /// </summary>
    public class HouseAppointmentController : Controller
    {
        public IHouseAppointmentService appService { get; set; }
        public IAdminUserService userService { get; set; }

        /// <summary>
        /// 预约看房列表
        /// </summary>
        /// <returns></returns>
        //[CheckPermission("HouseApp.List")]
        public ActionResult List()
        {
            //先查出用户的id
            long? userId = AdminHelper.GetUserId(HttpContext);
            //通过用户的id查出用户城市的id
            long? cityId = userService.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部的人不能进行房源抢单");
            }

            //todo:做分页
            var apps = appService.GetPagedData(cityId.Value, "未处理", 10, 1);
            return View(apps);
        }
        /// <summary>
        /// 开始抢单
        /// </summary>
        /// <param name="appId">预约看房id</param>
        /// <returns></returns>
        //[CheckPermission("HouseApp.Follow")]
        public ActionResult Follow(long appId)
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            bool isOK = appService.Follow(userId.Value, appId);
            if (isOK)
            {
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult { Status = "fail" });
            }
        }
    }
}