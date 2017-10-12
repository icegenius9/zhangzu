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
    public class RoleController : Controller
    {
        public IRoleService roleService { get; set; }
        public IPermissionService permService { get; set; }
        // GET: Role
        [CheckPermission("Role.List")]
        public ActionResult List()
        {
            var roles=roleService.GetAll();
            return View(roles);
        }
        [CheckPermission("Role.Delete")]
        public ActionResult Delete(int id)
        {
            roleService.MarkDeleted(id);
            return Json(new AjaxResult { Status = "ok" });
        }

        [HttpPost]
        [CheckPermission("Role.Delete")]
        public ActionResult BatchDelete(long [] selectdIds)
        {
            foreach (long id in selectdIds)
            {
                roleService.MarkDeleted(id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }

        [HttpGet]
        [CheckPermission("Role.Add")]
        public ActionResult Add()
        {
            //新增角色的时候也要提供所有的权限项供选择
           var perms= permService.GetAll();
            return View(perms);
        }

        [HttpPost]
        [CheckPermission("Role.Add")]
        public ActionResult Add(RoleAddModel model)
        {
            //ModelState.IsValid检查model验证是否通过
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error",
                    ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }

            //TransactionScope(事务)
            long roleId = roleService.AddNew(model.Name);
            permService.AddPermIds(roleId, model.PermissionIds);
            return Json(new AjaxResult { Status = "ok" });
        }

        [HttpGet]
        [CheckPermission("Role.Edit")]
        public ActionResult Edit(long id)
        {
            var role=roleService.GetById(id);
            var roleperm=permService.GetByRoleId(role.Id);
            var allPerms = permService.GetAll();

            //ViewBag.role = role;
            //ViewBag.roleperm = roleperm;
            RoleEditGetModel model = new RoleEditGetModel();
            model.role = role;
            model.RolePerms = roleperm;
            model.AllPerms = allPerms;
            return View(model);
        }

        [HttpPost]
        [CheckPermission("Role.Edit")]
        public ActionResult Edit(RoleEditModel model)
        {
            //修改角色的名称
            roleService.Update(model.Id, model.Name);
            //修改角色拥有的权限项
            permService.UpdatePermIds(model.Id, model.PermissionIds);

            return Json(new AjaxResult { Status = "ok" });
        }
    }
}