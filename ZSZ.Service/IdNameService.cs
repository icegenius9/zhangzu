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
    /// 数据字典的Service
    /// </summary>
    public class IdNameService : IIdNameService
    {
        public long AddNew(string typeName, string name)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                IdNameEntity idName =
                    new IdNameEntity { Name = name, TypeName = typeName };

                //todo:检查重复性
                ctx.IdNames.Add(idName);
                ctx.SaveChanges();
                return idName.Id;
            }
        }

        private IdNameDTO ToDTO(IdNameEntity entity)
        {
            IdNameDTO dto = new IdNameDTO();
            dto.CreateDateTime = entity.CreateDateTime;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.TypeName = entity.TypeName;
            return dto;
        }

        public IdNameDTO[] GetAll(string typeName)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<IdNameEntity> bs 
                    = new BaseService<IdNameEntity>(ctx);
                return bs.GetAll().Where(e => e.TypeName == typeName)
                    .ToList().Select(e=>ToDTO(e)).ToArray();
            }
        }

        /// <summary>
        /// 根据id获得某给数据字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IdNameDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<IdNameEntity> bs
                    = new BaseService<IdNameEntity>(ctx);
                return ToDTO(bs.GetById(id));
            }
        }
    }
}
