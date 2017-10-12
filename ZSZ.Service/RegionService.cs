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
    /// 区域的Service
    /// </summary>
    public class RegionService : IRegionService
    {
        private RegionDTO ToDTO(RegionEntity region)
        {
            RegionDTO dto = new RegionDTO();
            dto.CityId = region.CityId;
            dto.CityName = region.City.Name;
            dto.CreateDateTime = region.CreateDateTime;
            dto.Id = region.Id;
            dto.Name = region.Name;
            return dto;
        }
        /// <summary>
        /// 通过城市id获得所有的区域
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public RegionDTO[] GetAll(long cityId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RegionEntity> bs 
                    = new BaseService<RegionEntity>(ctx);
                return bs.GetAll().Include(r=>r.City)
                    .Where(r => r.CityId == cityId).ToList()
                    .Select(r=>ToDTO(r)).ToArray();
            }
        }
        /// <summary>
        /// 通过id获得区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RegionDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<RegionEntity> bs
                    = new BaseService<RegionEntity>(ctx);
                var region = bs.GetAll().Include(r => r.City)
                    .SingleOrDefault(r => r.Id == id);
                return region == null ? null : ToDTO(region);
            }
        }
    }
}
