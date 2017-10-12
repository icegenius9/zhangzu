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
    /// 房屋配套设施
    /// </summary>
    public class AttachmentService : IAttachmentService
    {
        private AttachmentDTO ToDTO(AttachmentEntity att)
        {
            AttachmentDTO dto = new AttachmentDTO();
            dto.CreateDateTime = att.CreateDateTime;
            dto.IconName = att.IconName;
            dto.Id = att.Id;
            dto.Name = att.Name;
            return dto;
        }
        /// <summary>
        /// 获得所有的配套设施
        /// </summary>
        /// <returns></returns>
        public AttachmentDTO[] GetAll()
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AttachmentEntity> bs
                    = new BaseService<AttachmentEntity>(ctx);
                var items = bs.GetAll().AsNoTracking();
                return items.ToList().Select(a=>ToDTO(a)).ToArray();
            }
        }

        /// <summary>
        /// 获得某个房子的所有的配套设施
        /// </summary>
        /// <param name="houseId">房子的id</param>
        /// <returns></returns>
        public AttachmentDTO[] GetAttachments(long houseId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> houseBS
                    = new BaseService<HouseEntity>(ctx);
                //先获id为houseId的房子延迟加载Attachments(房子的配套设施)的属性
                var house = houseBS.GetAll().Include(a => a.Attachments)
                    .AsNoTracking().SingleOrDefault(h=>h.Id==houseId);
                if(house==null)
                {
                    throw new ArgumentException("houseId"+houseId+"不存在");
                }
                //一个房子有多个配套设施
                return house.Attachments.ToList().Select(a => ToDTO(a)).ToArray();
            }
        }
    }
}
