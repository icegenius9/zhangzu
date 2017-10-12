using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;
using System.Data.Entity;

namespace ZSZ.Service
{
    /// <summary>
    /// 配置Service
    /// </summary>
    public class SettingService : ISettingService
    {
        /// <summary>
        /// 获得所有的配置
        /// </summary>
        /// <returns></returns>
        public SettingDTO[] GetAll()
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<SettingEntity> bs = new BaseService<SettingEntity>(ctx);
                /*
                return bs.GetAll().Select(s => new SettingDTO
                {
                    Id = s.Id,
                    CreateDateTime = s.CreateDateTime,
                    Name = s.Name,
                    Value = s.Value
                }).ToArray();*/
                List<SettingDTO> list = new List<SettingDTO>();
                foreach(var setting in bs.GetAll())
                {
                    SettingDTO dto = new SettingDTO();
                    dto.CreateDateTime = setting.CreateDateTime;
                    dto.Id = setting.Id;
                    dto.Name = setting.Name;
                    dto.Value = setting.Value;
                    list.Add(dto);
                }
                return list.ToArray();
            }
        }
        /// <summary>
        /// 获得bool的value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool? GetBoolValue(string name)
        {
            string value = GetValue(name);
            if(value==null)
            {
                return null;
            }
            else
            {
                return Convert.ToBoolean(value);
            }
        }

        /// <summary>
        /// 获得int的value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetIntValue(string name)
        {
            string value = GetValue(name);
            if (value == null)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// 根据配置项的名字获得value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetValue(string name)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<SettingEntity> bs = new BaseService<SettingEntity>(ctx);
                var setting = bs.GetAll().AsNoTracking()
                    .SingleOrDefault(s => s.Name == name);
                if (setting == null)//没有
                {
                    return null;
                }
                else//有就返回配置项的value
                {
                    return setting.Value;
                }
            }
        }

        public void SetBoolValue(string name, bool value)
        {
            SetValue(name, value.ToString());
        }

        public void SetIntValue(string name, int value)
        {
            SetValue(name, value.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, string value)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<SettingEntity> bs = new BaseService<SettingEntity>(ctx);
                //先获得名字为name他的配置项
                var setting = bs.GetAll().SingleOrDefault(s => s.Name == name);
                if(setting==null)//没有，则新增
                {
                    ctx.Settings.Add(new SettingEntity { Name=name,Value=value});
                }
                else//有的话更新
                {
                    setting.Value = value;
                }
                ctx.SaveChanges();
            }
        }
    }
}
