using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Common;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    /// <summary>
    /// 前台用户的userService
    /// </summary>
    public class UserService : IUserService
    {
        public long AddNew(string phoneNum, string password)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //检查手机号不能重复

                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                bool exists = bs.GetAll().Any(u=>u.PhoneNum==phoneNum);
                if(exists)
                {
                    throw new ArgumentException("手机号已经存在");
                }
                UserEntity user = new UserEntity();
                user.PhoneNum = phoneNum;
                string salt = CommonHelper.CreateVerifyCode(5);
                string pwdHash = CommonHelper.CalcMD5(salt + password);
                user.PasswordHash = pwdHash;
                user.PasswordSalt = salt;
                ctx.Users.Add(user);
                ctx.SaveChanges();
                return user.Id;
            }
        }

        /// <summary>
        /// 检查登入
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckLogin(string phoneNum, string password)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetAll().SingleOrDefault(u=>u.PhoneNum==phoneNum);
                if(user==null)
                {
                    return false;
                }
                else
                {
                    string dbPwdHash = user.PasswordHash;
                    string salt = user.PasswordSalt;
                    string userPwdHash = CommonHelper.CalcMD5(salt + password);
                    return dbPwdHash == userPwdHash;
                }
            }
        }

        private UserDTO ToDTO(UserEntity user)
        {
            UserDTO dto = new UserDTO();
            dto.CityId = user.CityId;
            dto.CreateDateTime = user.CreateDateTime;
            dto.Id = user.Id;
            dto.LastLoginErrorDateTime = user.LastLoginErrorDateTime;
            dto.LoginErrorTimes = user.LoginErrorTimes;
            dto.PhoneNum = user.PhoneNum;
            return dto;
        }

        public UserDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(id);
                return user == null ? null : ToDTO(user);
            }
        }

        public UserDTO GetByPhoneNum(string phoneNum)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetAll().SingleOrDefault(u => u.PhoneNum == phoneNum);
                return user == null ? null : ToDTO(user);
            }
        }

        /// <summary>
        /// 设置用户的城市
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cityId"></param>
        public void SetUserCityId(long userId, long cityId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(userId);
                if(user==null)
                {
                    throw new ArgumentException("用户id不存在"+userId);
                }
                user.CityId = cityId;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 修改前台用户的密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        public void UpdatePwd(long userId, string newPassword)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //检查手机号不能重复
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(userId);
                if (user==null)
                {
                    throw new ArgumentException("用户不存在 "+ userId);
                }
                string salt = user.PasswordSalt;// CommonHelper.CreateVerifyCode(5);
                string pwdHash = CommonHelper.CalcMD5(salt + newPassword);
                user.PasswordHash = pwdHash;
                user.PasswordSalt = salt;
                ctx.SaveChanges();
            }
        }
        /// <summary>
        /// 记录一次登录失败
        /// </summary>
        /// <param name="id">用户id</param>
        public void IncrLoginError(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //检查手机号不能重复
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(id);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在 " + id);
                }
                //设置登录错误次数
                user.LoginErrorTimes++;
                //设置登录错误时间
                user.LastLoginErrorDateTime = DateTime.Now;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 重置登录失败信息
        /// </summary>
        /// <param name="id">用户id</param>
        public void ResetLoginError(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //检查手机号不能重复
                BaseService<UserEntity> bs = new BaseService<UserEntity>(ctx);
                var user = bs.GetById(id);
                if (user == null)
                {
                    throw new ArgumentException("用户不存在 " + id);
                }
                //设置登录错误次数
                user.LoginErrorTimes = 0;
                //设置登录错误时间
                user.LastLoginErrorDateTime = null;
                ctx.SaveChanges();
            }
        }
        /// <summary>
        /// 判断用户是否已经被锁定
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public bool IsLocked(long id)
        {
            var user = GetById(id);
            //错误登录次数>=5，最后一次登录错误时间在30分钟之内
            return (user.LoginErrorTimes >= 5
                && user.LastLoginErrorDateTime > DateTime.Now.AddMinutes(-30));
        }
    }
}
