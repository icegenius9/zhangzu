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
    /// 小区Service
    /// </summary>
    public class CommunityService : ICommunityService
    {
        /*
        private CommunityDTO ToDTO(CommunityEntity en)
        {
            CommunityDTO dto = new CommunityDTO();
            dto.bui
        }*/

        /// <summary>
        /// 根据区域id获得所有的小区
        /// </summary>
        /// <param name="regionId">区域id</param>
        /// <returns></returns>
        public CommunityDTO[] GetByRegionId(long regionId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<CommunityEntity> bs 
                    = new BaseService<CommunityEntity>(ctx);
                var cities = bs.GetAll().AsNoTracking()
                    .Where(c => c.RegionId == regionId);
                //下面这段也可以to dto
                return cities.Select(c=>new CommunityDTO { BuiltYear=c.BuiltYear,
                CreateDateTime = c.CreateDateTime,Id=c.Id,Location=c.Location,
                    Name =c.Name,RegionId=c.RegionId,Traffic=c.Traffic}).ToArray();
            }
        }
    }
}
