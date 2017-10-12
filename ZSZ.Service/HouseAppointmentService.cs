using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ZSZ.Service
{
    /// <summary>
    /// 房屋预约管理的Service
    /// </summary>
    public class HouseAppointmentService : IHouseAppointmentService
    {
        public long AddNew(long? userId, string name, string phoneNum, long houseId, DateTime visitDate)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                HouseAppointmentEntity houseApp = new HouseAppointmentEntity();
                houseApp.HouseId = houseId;
                houseApp.Name = name;
                houseApp.PhoneNum = phoneNum;
                houseApp.Status = "未处理";
                houseApp.UserId = userId;
                houseApp.VisitDate = visitDate;
                ctx.HouseAppointments.Add(houseApp);
                ctx.SaveChanges();
                return houseApp.Id;
            }
        }
        /// <summary>
        /// 预约看房抢单(乐观锁)
        /// </summary>
        /// <param name="adminUserId">用户的id</param>
        /// <param name="houseAppointmentId">预约看房的id</param>
        /// <returns>bool(抢单成功或者失败)</returns>
        public bool Follow(long adminUserId, long houseAppointmentId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseAppointmentEntity> bs =
                    new BaseService<HouseAppointmentEntity>(ctx);
                //先获得id为houseAppointmentId的预约看房
                var app = bs.GetById(houseAppointmentId);
                if (app == null)
                {
                    throw new ArgumentException("不存在的订单id");
                }
                //FollowAdminUserId不为null，说明要么是自己已经抢过，要么是已经早早的
                //被别人抢了
                if (app.FollowAdminUserId != null)
                {
                    return app.FollowAdminUserId == adminUserId;
                    /*
                    if(app.FollowAdminUserId==adminUserId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }*/
                }
                //如果/FollowAdminUserId为null，说明有抢的机会
                app.FollowAdminUserId = adminUserId;
                try
                {
                    ctx.SaveChanges();
                    return true;
                }//如果ctx.SaveChanges()时抛出DbUpdateConcurrencyException说明抢单失败（乐观锁）
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }
        }

        private HouseAppointmentDTO ToDTO(HouseAppointmentEntity houseApp)
        {
            HouseAppointmentDTO dto = new HouseAppointmentDTO();
            //小区的名字通过预约看房的房子属性中的小区属性获得
            dto.CommunityName = houseApp.House.Community.Name;
            dto.CreateDateTime = houseApp.CreateDateTime;
            dto.FollowAdminUserId = houseApp.FollowAdminUserId;
             //如果跟踪用户不为null
            if(houseApp.FollowAdminUser!=null)
            {
                dto.FollowAdminUserName = houseApp.FollowAdminUser.Name;
            }
            dto.FollowDateTime = houseApp.FollowDateTime;
            dto.HouseId = houseApp.HouseId;
            dto.Id = houseApp.Id;
            dto.Name = houseApp.Name;
            dto.PhoneNum = houseApp.PhoneNum;
            //预约看房的房子在哪个区，也是通过导航属性获得
            dto.RegionName = houseApp.House.Community.Region.Name;
            dto.Status = houseApp.Status;
            dto.UserId = houseApp.UserId;
            dto.VisitDate = houseApp.VisitDate;
            dto.HouseAddress = houseApp.House.Address;
            return dto;
        }
        /// <summary>
        /// 根据id获得预约房屋的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HouseAppointmentDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseAppointmentEntity> bs 
                    = new BaseService<HouseAppointmentEntity>(ctx);
                var houseApp = bs.GetAll().Include(a => a.House)
                    //Include("House.Community")
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Community))
                    .Include(a => a.FollowAdminUser)
                    //Include("House.Community.Region")
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    .AsNoTracking().SingleOrDefault(a => a.Id == id);
                if(houseApp==null)
                {
                    return null;
                }
                return ToDTO(houseApp);
            }
        }
        /// <summary>
        /// 分页获取房屋预约信息
        /// </summary>
        /// <param name="cityId">城市的id</param>
        /// <param name="status">房屋预约的状态</param>
        /// <param name="pageSize">取多少数据</param>
        /// <param name="currentIndex">跳过多少条</param>
        /// <returns></returns>
        public HouseAppointmentDTO[] GetPagedData(long cityId, string status, 
            int pageSize, int currentIndex)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseAppointmentEntity> bs
                    = new BaseService<HouseAppointmentEntity>(ctx);
                var apps = bs.GetAll().Include(a => a.House)
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Community))
                    .Include(a => a.FollowAdminUser)
                    .Include(nameof(HouseAppointmentEntity.House) + "." + nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    .AsNoTracking()
                    //过滤城市和状态
                    .Where(a => a.House.Community.Region.CityId == cityId && a.Status == status)
                    .OrderByDescending(a => a.CreateDateTime)//Skip之前一定要调用OrderBy
                    .Skip(currentIndex).Take(pageSize);
                return apps.ToList().Select(a=>ToDTO(a)).ToArray();
            }
        }

        /// <summary>
        /// 获得总数据条数(用LongCount)
        /// </summary>
        /// <param name="cityId">城市的id</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public long GetTotalCount(long cityId, string status)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseAppointmentEntity> bs
                    = new BaseService<HouseAppointmentEntity>(ctx);
                var count = bs.GetAll()
                    //Where(a => a.House.Community.Region.CityId == cityId && a.Status == status).LongCount() //等价于下面的写法
                    .LongCount(a => a.House.Community.Region.CityId == cityId && a.Status == status);
                return count;
            }
        }
    }
}
