using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;
using System.Data.Entity;
using ZSZ.Common;

namespace ZSZ.Service
{
    /// <summary>
    /// 管理员相关的service
    /// </summary>
    public class AdminUserService : IAdminUserService
    {
        /// <summary>
        /// 新增后台管理员
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="phoneNum">手机号</param>
        /// <param name="password">用户输入的密码</param>
        /// <param name="email">邮箱</param>
        /// <param name="cityId">城市id</param>
        /// <returns></returns>
        public long AddAdminUser(string name, string phoneNum, 
            string password, string email, long? cityId)
        {
            AdminUserEntity user = new AdminUserEntity();
            user.CityId = cityId;
            user.Email = email;
            user.Name = name;
            user.PhoneNum = phoneNum;
            string salt = CommonHelper.CreateVerifyCode(5);//盐
            user.PasswordSalt = salt;
            //Md5(盐+用户密码)
            string pwdHash = CommonHelper.CalcMD5(salt+password);
            user.PasswordHash = pwdHash;
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs 
                    = new BaseService<AdminUserEntity>(ctx);
                bool exists = bs.GetAll().Any(u => u.PhoneNum == phoneNum);
                if(exists)
                {
                    throw new ArgumentException("手机号已经存在"+phoneNum);
                }
                ctx.AdminUsers.Add(user);
                ctx.SaveChanges();
                return user.Id;
            }
        }

        /// <summary>
        /// 检查登录
        /// </summary>
        /// <param name="phoneNum">手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public bool CheckLogin(string phoneNum, string password)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs = new BaseService<AdminUserEntity>(ctx);
                //除了错不可怕，最怕的是有错但是表面“风平浪静” SingleOrDefault(有且只有一条出现2条会报错)
                var user = bs.GetAll().SingleOrDefault(u=>u.PhoneNum==phoneNum);
                if (user == null)
                {
                    return false;
                }
                string dbHash = user.PasswordHash;
                string userHash = CommonHelper.CalcMD5(user.PasswordSalt+password);
                //比较数据库中的PasswordHash是否和MD5(salt+用户输入密码)一直
                return userHash == dbHash;
            }
        }

        private AdminUserDTO ToDTO(AdminUserEntity user)
        {
            AdminUserDTO dto = new AdminUserDTO();
            dto.CityId = user.CityId;
            if(user.City!=null)
            {
                dto.CityName = user.City.Name;//需要Include提升性能
                //如鹏总部（北京）、如鹏网上海分公司、如鹏广州分公司、如鹏北京分公司
            }
            else
            {
                dto.CityName = "总部";
            }
            
            dto.CreateDateTime = user.CreateDateTime;
            dto.Email = user.Email;
            dto.Id = user.Id;
            dto.LastLoginErrorDateTime = user.LastLoginErrorDateTime;
            dto.LoginErrorTimes = user.LoginErrorTimes;
            dto.Name = user.Name;
            dto.PhoneNum = user.PhoneNum;
            return dto;
        }

        /// <summary>
        /// 取得所有的管理员信息
        /// </summary>
        /// <returns></returns>
        public AdminUserDTO[] GetAll()
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //using System.Data.Entity;才能在IQueryable中用Include(避免延迟加载)、AsNoTracking(不改变状态)
                BaseService<AdminUserEntity> bs 
                    = new BaseService<AdminUserEntity>(ctx);
                return bs.GetAll().Include(u=>u.City)
                    .AsNoTracking().ToList().Select(u => ToDTO(u)).ToArray();
            }
        }

        /// <summary>
        /// 获得某个城市的管理员
        /// </summary>
        /// <param name="cityId">如果为null则获取总部的管理员；否则是获取某个地区的</param>
        /// <returns></returns>
        public AdminUserDTO[] GetAll(long? cityId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs
                    = new BaseService<AdminUserEntity>(ctx);
                //CityId is null;CityId=3
                var all = bs.GetAll().Include(u => u.City)
                    .AsNoTracking().Where(u => u.CityId == cityId);
                return all.ToList().Select(u => ToDTO(u)).ToArray();
            }
        }

        /// <summary>
        /// 根据id获得管理员
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public AdminUserDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs
                    = new BaseService<AdminUserEntity>(ctx);
                //这里不能用bs.GetById(id);因为无法Include、AsNoTracking()等  因为只有IQueryable才能用Include、AsNoTracking()等
                var user = bs.GetAll().Include(u => u.City)
                    .AsNoTracking().SingleOrDefault(u=>u.Id==id);
                    //.AsNoTracking().Where(u=>u.Id==id).SingleOrDefault(); //这句跟上面一句效果一样
                //var user = bs.GetById(id); 用include就不能用GetById
                if (user==null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }
        
        /// <summary>
        /// 根据手机号查找某个管理员
        /// </summary>
        /// <param name="phoneNum">手机号</param>
        /// <returns></returns>
        public AdminUserDTO GetByPhoneNum(string phoneNum)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs
                    = new BaseService<AdminUserEntity>(ctx);
                var users = bs.GetAll().Include(u => u.City)
                    .AsNoTracking().Where(u => u.PhoneNum == phoneNum);
                int count = users.Count();
                if(count <= 0)
                {
                    return null;
                }
                else if(count==1)
                {
                    //Single()取一条数据，把IQueryable<T> 转换成T类型然后在转成DTO
                    return ToDTO(users.Single());
                }
                else
                {
                    throw new ApplicationException("找到多个手机号为"+phoneNum+"的管理员");
                }
            }
        }
        /// <summary>
        /// 判断adminUserId用户是否有permissionName一个权限
        /// </summary>
        /// <param name="adminUserId">用户id</param>
        /// <param name="permissionName">权限名称</param>
        /// <returns></returns>
        //HasPermission(5,"User.Add")
        public bool HasPermission(long adminUserId, string permissionName)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs
                    = new BaseService<AdminUserEntity>(ctx);
                var user = bs.GetAll().Include(u => u.Roles)
                    .AsNoTracking().SingleOrDefault(u=>u.Id==adminUserId);
                //var user = bs.GetById(adminUserId);
                if (user==null)
                {
                    throw new ArgumentException("找不到id="+adminUserId+"的用户");
                }
                //每个Role都有一个Permissions属性
                //Roles.SelectMany(r => r.Permissions)就是遍历Roles的每一个Role
                //然后把每个Role的Permissions放到一个集合中 IEnumerable<PermissionEntity>
                //最后判断这个用户有没有permissionName这个权限
                return user.Roles.SelectMany(r => r.Permissions)
                    .Any(p=>p.Name==permissionName);
            }
        }

        /// <summary>
        /// 后台用户软删除
        /// </summary>
        /// <param name="adminUserId"></param>
        public void MarkDeleted(long adminUserId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs 
                    = new BaseService<AdminUserEntity>(ctx);
                bs.MarkDeleted(adminUserId);
            }
        }

        /// <summary>
        /// 记录用户错误登录的次数
        /// </summary>
        /// <param name="id"></param>
        public void RecordLoginError(long id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 重置用户错误登录次数
        /// </summary>
        /// <param name="id"></param>
        public void ResetLoginError(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新后台用户的信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="phoneNum"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="cityId"></param>
        public void UpdateAdminUser(long id, string name, string phoneNum, 
            string password, string email, long? cityId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AdminUserEntity> bs
                    = new BaseService<AdminUserEntity>(ctx);
                var user = bs.GetById(id);
                if(user==null)
                {
                    throw new ArgumentException("找不到id="+id+"的管理员");
                }
                user.Name = name;
                user.PhoneNum = phoneNum;
                user.Email = email;
                user.PasswordHash = 
                    CommonHelper.CalcMD5(user.PasswordSalt+password);
                user.CityId = cityId;                
                ctx.SaveChanges();
            }
        }
    }
}
