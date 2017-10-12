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
    /// 权限的Service
    /// </summary>
    public class PermissionService : IPermissionService
    {
        /// <summary>
        /// 添加一个权限
        /// </summary>
        /// <param name="permName">权限项的名称</param>
        /// <param name="description">权限项的描述</param>
        /// <returns></returns>
        public long AddPermission(string permName,string description)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<PermissionEntity> permBS = new BaseService<PermissionEntity>(ctx);
                bool exists = permBS.GetAll().Any(p=>p.Name==permName);
                if(exists)
                {
                    throw new ArgumentException("权限项已经存在");
                }
                PermissionEntity perm = new PermissionEntity();
                perm.Description = description;
                perm.Name = permName;
                ctx.Permissions.Add(perm);
                ctx.SaveChanges();
                return perm.Id;
            }
        }

        /// <summary>
        /// 给这个roleId角色添加一些权限permIds
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="permIds">权限的数组2</param>
        public void AddPermIds(long roleId, long[] permIds)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> roleBS 
                    = new BaseService<RoleEntity>(ctx);
                var role = roleBS.GetById(roleId);
                if(role==null)
                {
                    throw new ArgumentException("roleId不存在"+roleId);
                }
                BaseService<PermissionEntity> permBS
                    = new BaseService<PermissionEntity>(ctx);
                var perms = permBS.GetAll()
                    .Where(p => permIds.Contains(p.Id)).ToArray();
                foreach(var perm in perms)
                {
                    role.Permissions.Add(perm);
                }
                ctx.SaveChanges();
            }
        }

        private PermissionDTO ToDTO(PermissionEntity p)
        {
            PermissionDTO dto = new PermissionDTO();
            dto.CreateDateTime = p.CreateDateTime;
            dto.Description = p.Description;
            dto.Id = p.Id;
            dto.Name = p.Name;
            return dto;
        }

        /// <summary>
        /// 获得所有的权限
        /// </summary>
        /// <returns></returns>
        public PermissionDTO[] GetAll()
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                return bs.GetAll().ToList().Select(p=>ToDTO(p)).ToArray();
            }
        }

        /// <summary>
        /// 根据id查询权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PermissionDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                var pe = bs.GetById(id);
                return pe == null ? null : ToDTO(pe);
            }
        }

        /// <summary>
        /// 根据名字查询权限
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PermissionDTO GetByName(string name)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                var pe = bs.GetAll().SingleOrDefault(p=>p.Name==name);
                return pe == null ? null : ToDTO(pe);
            }
        }

        /// <summary>
        /// 根据某个角色所有的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public PermissionDTO[] GetByRoleId(long roleId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> bs = new BaseService<RoleEntity>(ctx);
                return bs.GetById(roleId).Permissions.ToList().Select(p => ToDTO(p)).ToArray();
            }
        }

        //2,3,4
        //3,4,5
        /// <summary>
        /// 更新某个角色的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permIds"></param>
        public void UpdatePermIds(long roleId, long[] permIds)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RoleEntity> roleBS
                    = new BaseService<RoleEntity>(ctx);
                var role = roleBS.GetById(roleId);
                if (role == null)
                {
                    throw new ArgumentException("roleId不存在" + roleId);
                }
                role.Permissions.Clear();
                BaseService<PermissionEntity> permBS
                    = new BaseService<PermissionEntity>(ctx);
                var perms = permBS.GetAll()
                    .Where(p => permIds.Contains(p.Id)).ToList();
                foreach (var perm in perms)
                {
                    role.Permissions.Add(perm);
                }
                ctx.SaveChanges();
            }
        }

        public void UpdatePermission(long id, string permName, string description)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            { 
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                var perm=bs.GetById(id);
                if (perm == null)
                {
                    throw new ArgumentException("id不存在");
                }
                perm.Name = permName;
                perm.Description = description;
                ctx.SaveChanges();
            }

        }

        public void MarkDeleted(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<PermissionEntity> bs = new BaseService<PermissionEntity>(ctx);
                bs.MarkDeleted(id);
            }
        }
    }
}
