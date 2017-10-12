using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    /// <summary>
    /// 角色的service
    /// </summary>
    public class RoleService : IRoleService
    {
        /// <summary>
        /// 增加一个角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public long AddNew(string roleName)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> roleBS 
                    = new BaseService<RoleEntity>(ctx);
                bool exists = roleBS.GetAll().Any(r => r.Name == roleName);
                //正常情况不应该执行这个异常，因为UI层应该把这些情况处理好
                //这里只是“把好最后一关”
                if (exists)
                {
                    throw new ArgumentException("角色名字已经存在"+roleName);
                }
                RoleEntity role = new RoleEntity();
                role.Name = roleName;
                ctx.Roles.Add(role);
                ctx.SaveChanges();
                return role.Id;
            }
        }

        /// <summary>
        /// 给用户增加角色
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="roleIds"></param>
        public void AddRoleIds(long adminUserId, long[] roleIds)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> userBS 
                    = new BaseService<AdminUserEntity>(ctx);
                var user = userBS.GetById(adminUserId);
                if(user==null)
                {
                    throw new ArgumentException("用户不存在" + adminUserId);
                }
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                var roles = roleBS.GetAll().Where(r=>roleIds.Contains(r.Id)).ToArray();
                foreach(var role in roles)
                {
                    user.Roles.Add(role);
                }
                ctx.SaveChanges();
            }
        }

        private RoleDTO ToDTO(RoleEntity en)
        {
            RoleDTO dto = new RoleDTO();
            dto.CreateDateTime = en.CreateDateTime;
            dto.Id = en.Id;
            dto.Name = en.Name;
            return dto;
        }

        /// <summary>
        /// 获得所有的角色
        /// </summary>
        /// <returns></returns>
        public RoleDTO[] GetAll()
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                return bs.GetAll().ToList().Select(p => ToDTO(p)).ToArray();
            }
        }

        /// <summary>
        /// 根据用户id获得所有角色
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public RoleDTO[] GetByAdminUserId(long adminUserId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                var user = bs.GetById(adminUserId);
                if(user==null)
                {
                    throw new ArgumentException("不存在的管理员"+adminUserId);
                }
                return user.Roles.ToList().Select(r=>ToDTO(r)).ToArray();
            }
        }

        /// <summary>
        /// 根据id获得角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoleDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                var role = bs.GetById(id);
                return role == null ? null : ToDTO(role);
            }
        }

        /// <summary>
        /// 根据名字获得角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RoleDTO GetByName(string name)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                var role = bs.GetAll().SingleOrDefault(r => r.Name == name);
                return role == null ? null : ToDTO(role);
            }
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="roleId"></param>
        public void MarkDeleted(long roleId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                bs.MarkDeleted(roleId);
            }
        }

        /// <summary>
        /// 更新roleId角色的名字为roleName
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleName"></param>
        public void Update(long roleId, string roleName)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                bool exists = roleBS.GetAll().Any(r=>r.Name==roleName&&r.Id!=roleId);
                //正常情况不应该执行这个异常，因为UI层应该把这些情况处理好
                //这里只是“把好最后一关”
                if (exists)
                {
                    throw new ArgumentException("");
                }
                RoleEntity role = new RoleEntity();
                role.Id = roleId;
                ctx.Entry(role).State = System.Data.Entity.EntityState.Unchanged;
                role.Name = roleName;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 更新用户所有的角色
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="roleIds"></param>
        public void UpdateRoleIds(long adminUserId, long[] roleIds)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> userBS
                    = new BaseService<AdminUserEntity>(ctx);
                var user = userBS.GetById(adminUserId);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在" + adminUserId);
                }
                user.Roles.Clear();
                BaseService<RoleEntity> roleBS = new BaseService<RoleEntity>(ctx);
                var roles = roleBS.GetAll().Where(r => roleIds.Contains(r.Id)).ToArray();
                foreach (var role in roles)
                {
                    user.Roles.Add(role);
                }
                ctx.SaveChanges();
            }
        }
    }
}
