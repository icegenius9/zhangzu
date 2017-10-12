using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Models;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    public class PermissionController : Controller
    {
        public IPermissionService PermSvc { get; set; }
        // GET: Permission
        [CheckPermission("Permission.List")]
        public ActionResult List()
        {
            var pers=PermSvc.GetAll();
            return View(pers);
        }

        /// <summary>
        /// get软删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [CheckPermission("Permission.Delete")]
        public ActionResult GetDelete(long id)
        {
            PermSvc.MarkDeleted(id);
            //软删除权限之后刷新
            //return RedirectToAction("List");
            //C#6.0语法
            return RedirectToAction(nameof(List));
        }
        /// <summary>
        /// ajax软删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete2(long id)
        {
            PermSvc.MarkDeleted(id);
            return Json(new AjaxResult { Status = "ok" });
        }

        [HttpGet]
        [CheckPermission("Permission.Add")]
        public ActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>        
        //public ActionResult Add(string name,string description)
        [CheckPermission("Permission.Add")]
        [HttpPost]
        public ActionResult Add(PermissionAddNewModel model)
        {
            PermSvc.AddPermission(model.Name, model.Description);
            //iframe中重定向与预期不符
            // return RedirectToAction(nameof(List));

            //todo:权限项名字不能重复
            return Json(new AjaxResult { Status = "ok" });
        }

        [HttpGet]
        [CheckPermission("Permission.Edit")]
        public ActionResult Edit(int id)
        {
            var perm = PermSvc.GetById(id);
            return View(perm);
        }

        [HttpPost]
        [CheckPermission("Permission.Edit")]
        public ActionResult Edit(PermissionEditModel model)
        {
            PermSvc.UpdatePermission(model.id, model.Name, model.Description);
            //todo:检查name不能重复
            return Json(new AjaxResult { Status = "ok" });
        }
    }
}