using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.DTO;
using ZSZ.IService;
using ZSZ.Service.Entities;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace ZSZ.Service
{
    /// <summary>
    /// 房子的Service
    /// </summary>
    public class HouseService : IHouseService
    {
        /*
        public long AddNew(HouseDTO house)
        {
            HouseEntity houseEntity = new HouseEntity();
            houseEntity.Address = house.Address;
            houseEntity.Area = house.Area;

            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AttachmentEntity> attBS
                    = new BaseService<AttachmentEntity>(ctx);
                var atts = attBS.GetAll().Where(a => house.AttachmentIds.Contains(a.Id));
                foreach (var att in atts)
                {
                    houseEntity.Attachments.Add(att);
                }
                houseEntity.CheckInDateTime = house.CheckInDateTime;
                houseEntity.CommunityId = house.CommunityId;
                houseEntity.CreateDateTime = house.CreateDateTime;
                houseEntity.DecorateStatusId = house.DecorateStatusId;
                houseEntity.Description = house.Description;
                houseEntity.Direction = house.Direction;
                houseEntity.FloorIndex = house.FloorIndex;
                //houseEntity.HousePics 新增后再单独添加
                houseEntity.LookableDateTime = house.LookableDateTime;
                houseEntity.MonthRent = house.MonthRent;
                houseEntity.OwnerName = house.OwnerName;
                houseEntity.OwnerPhoneNum = house.OwnerPhoneNum;
                houseEntity.RoomTypeId = house.RoomTypeId;
                houseEntity.StatusId = house.StatusId;
                houseEntity.TotalFloorCount = house.TotalFloorCount;
                houseEntity.TypeId = house.TypeId;
                ctx.Houses.Add(houseEntity);
                ctx.SaveChanges();
                return houseEntity.Id;
            }
            

        }*/

        public long AddNew(HouseAddNewDTO house)
        {
            HouseEntity houseEntity = new HouseEntity();
            houseEntity.Address = house.Address;
            houseEntity.Area = house.Area;

            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<AttachmentEntity> attBS
                    = new BaseService<AttachmentEntity>(ctx);
                //拿到house.AttachmentIds为主键的房屋配套设施 ,使用Contains取得所有房子的配套设施
                var atts = attBS.GetAll().Where(a => house.AttachmentIds.Contains(a.Id));
                //houseEntity.Attachments = new List<AttachmentEntity>();
                foreach (var att in atts)
                {
                    houseEntity.Attachments.Add(att);
                }
                houseEntity.CheckInDateTime = house.CheckInDateTime;
                houseEntity.CommunityId = house.CommunityId;
                houseEntity.DecorateStatusId = house.DecorateStatusId;
                houseEntity.Description = house.Description;
                houseEntity.Direction = house.Direction;
                houseEntity.FloorIndex = house.FloorIndex;
                //houseEntity.HousePics 新增后再单独添加
                houseEntity.LookableDateTime = house.LookableDateTime;
                houseEntity.MonthRent = house.MonthRent;
                houseEntity.OwnerName = house.OwnerName;
                houseEntity.OwnerPhoneNum = house.OwnerPhoneNum;
                houseEntity.RoomTypeId = house.RoomTypeId;
                houseEntity.StatusId = house.StatusId;
                houseEntity.TotalFloorCount = house.TotalFloorCount;
                houseEntity.TypeId = house.TypeId;
                ctx.Houses.Add(houseEntity);
                ctx.SaveChanges();
                return houseEntity.Id;
            }
        }

        /// <summary>
        /// 新增房子的图片
        /// </summary>
        /// <param name="housePic">图片的dto对象</param>
        /// <returns></returns>
        public long AddNewHousePic(HousePicDTO housePic)
        {
            HousePicEntity entity = new HousePicEntity();
            //房子的id
            entity.HouseId = housePic.HouseId;
            //房子缩略图地址
            entity.ThumbUrl = housePic.ThumbUrl;
            //房子图片地址
            entity.Url = housePic.Url;
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                ctx.HousePics.Add(entity);
                ctx.SaveChanges();
                return entity.Id;
            }
        }

        /// <summary>
        /// 删除房子的图片
        /// </summary>
        /// <param name="housePicId">图片的id</param>
        public void DeleteHousePic(long housePicId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //复习EF状态转换 2种删除方法
                /*
                HousePicEntity entity = new HousePicEntity();
                entity.Id = housePicId;
                ctx.Entry(entity).State = EntityState.Deleted;
                ctx.SaveChanges();*/
                var entity = ctx.HousePics
                    .SingleOrDefault(p => p.IsDeleted == false&&p.Id==housePicId);
                if(entity!=null)
                {
                    ctx.HousePics.Remove(entity);
                    ctx.SaveChanges();
                }
            }
        }

        private HouseDTO ToDTO(HouseEntity entity)
        {
            HouseDTO dto = new HouseDTO();
            dto.Address = entity.Address;
            dto.Area = entity.Area;
            dto.AttachmentIds = entity.Attachments.Select(a => a.Id).ToArray();
            dto.CheckInDateTime = entity.CheckInDateTime;
            dto.CityId = entity.Community.Region.CityId;
            dto.CityName = entity.Community.Region.City.Name;
            dto.CommunityBuiltYear = entity.Community.BuiltYear;
            dto.CommunityId = entity.CommunityId;
            dto.CommunityLocation = entity.Community.Location;
            dto.CommunityName = entity.Community.Name;
            dto.CommunityTraffic = entity.Community.Traffic;
            dto.CreateDateTime = entity.CreateDateTime;
            dto.DecorateStatusId = entity.DecorateStatusId;
            dto.DecorateStatusName = entity.DecorateStatus.Name;
            dto.Description = entity.Description;
            dto.Direction = entity.Direction;
            var firstPic = entity.HousePics.FirstOrDefault();
            if (firstPic != null)
            {
                dto.FirstThumbUrl = firstPic.ThumbUrl;
            }
            dto.FloorIndex = entity.FloorIndex;
            dto.Id = entity.Id;
            dto.LookableDateTime = entity.LookableDateTime;
            dto.MonthRent = entity.MonthRent;
            dto.OwnerName = entity.OwnerName;
            dto.OwnerPhoneNum = entity.OwnerPhoneNum;
            dto.RegionId = entity.Community.RegionId;
            dto.RegionName = entity.Community.Region.Name;
            dto.RoomTypeId = entity.RoomTypeId;
            dto.RoomTypeName = entity.RoomType.Name;
            dto.StatusId = entity.StatusId;
            dto.StatusName = entity.Status.Name;
            dto.TotalFloorCount = entity.TotalFloorCount;
            dto.TypeId = entity.TypeId;
            dto.TypeName = entity.Type.Name;
            return dto;
        }


        /// <summary>
        /// 根据id获得房子的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HouseDTO GetById(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                var house = houseBS.GetAll()
                    .Include(h => h.Attachments).Include(h => h.Community)
                    //Include("Community.Region.City");
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type)
                    .SingleOrDefault(h => h.Id == id);
                //.Where(h => h.Id == id).SingleOrDefault();
                if (house == null)
                {
                    return null;
                }
                return ToDTO(house);
            }
        }

        /// <summary>
        /// 根据cityId城市id，获得startDateTime时间到endDateTime时间的房子的数量
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public long GetCount(long cityId, DateTime startDateTime, DateTime endDateTime)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                return houseBS.GetAll()
                    .LongCount(h => h.Community.Region.CityId == cityId
                    && h.CreateDateTime >= startDateTime && h.CreateDateTime <= endDateTime);
            }
        }

        /// <summary>
        /// 分页获取cityId城市类型为typeId的房子
        /// </summary>
        /// <param name="cityId">城市的id</param>
        /// <param name="typeId">房子的类型</param>
        /// <param name="pageSize">每一页多少条</param>
        /// <param name="currentIndex">当前第几条</param>
        /// <returns></returns>
        public HouseDTO[] GetPagedData(long cityId, long typeId, int pageSize, int currentIndex)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                var houses = houseBS.GetAll()
                    .Include(h => h.Attachments).Include(h => h.Community)
                    //.Include(h => nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region)
                    //    + "." + nameof(RegionEntity.City))
                    //.Include(h => nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    //Indclude("Community.Region.City")
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type)
                    //注意Where的位置，要放到Skip之前
                    .Where(h => h.Community.Region.CityId == cityId && h.TypeId == typeId)
                    .OrderByDescending(h => h.CreateDateTime)
                    .Skip(currentIndex).Take(pageSize);
                    //.Where(h => h.Community.Region.CityId == cityId && h.TypeId == typeId);
                return houses.ToList().Select(h => ToDTO(h)).ToArray();
            }
        }

        /// <summary>
        /// 获得房子下所有的配图
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        public HousePicDTO[] GetPics(long houseId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                //上下2中方法都可以
                /*
                BaseService<HousePicEntity> bs = new BaseService<HousePicEntity>(ctx);
                return bs.GetAll().AsNoTracking().Where(p => p.HouseId == houseId)
                    .Select(p => new HousePicDTO
                    {
                        CreateDateTime = p.CreateDateTime,
                        HouseId = p.HouseId,
                        Id = p.Id,
                        ThumbUrl = p.ThumbUrl,
                        Url = p.Url
                    })
                    .ToArray();*/
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                return bs.GetById(houseId).HousePics.Select(p => new HousePicDTO
                {
                    CreateDateTime = p.CreateDateTime,
                    HouseId = p.HouseId,
                    Id = p.Id,
                    ThumbUrl = p.ThumbUrl,
                    Url = p.Url
                }).ToArray();
            }
        }

        /// <summary>
        /// 获取cityId城市typeId类型的房子数量
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public long GetTotalCount(long cityId, long typeId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                return bs.GetAll()
                    .LongCount(h => h.Community.Region.CityId == cityId && h.TypeId == typeId);
            }
        }

        public void MarkDeleted(long id)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                bs.MarkDeleted(id);
            }
        }
        /// <summary>
        /// 搜索，返回值包含:总条数和HouseDTO[](房子的数据)两个属性
        /// </summary>
        /// <param name="options">搜索条件</param>
        /// <returns></returns>
        public HouseSearchResult Search(HouseSearchOptions options)
        {
            /*
           using (ZSZDbContext ctx = new ZSZDbContext())
           {
               BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
               var items = bs.GetAll().Where(h => h.Address.Contains("楼"));
               long c = items.LongCount();
               items.Take(10).ToList();
           }*/

            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                //先获得所有的房子信息然后通过城市的id和房屋的类型过滤(因为城市id和房屋类型不会为空)
                var items = bs.GetAll().Where(h => h.Community.Region.CityId == options.CityId
                            && h.TypeId == options.TypeId);
                
                if (options.RegionId != null)//区域是否为空
                {
                    items = items.Where(t => t.Community.RegionId == options.RegionId);
                }
                if (options.StartMonthRent != null)//房租起始月租是否为空
                {
                    items = items.Where(t => t.MonthRent >= options.StartMonthRent);
                }
                if (options.EndMonthRent != null)//房租结束月租是否为空
                {
                    items = items.Where(t => t.MonthRent <= options.EndMonthRent);
                }
               
                if (!string.IsNullOrEmpty(options.Keywords))//搜索关键词是否为空
                {
                    //contains被EF翻译为like操作
                    items = items.Where(t => t.Address.Contains(options.Keywords)
                            || t.Description.Contains(options.Keywords)
                            || t.Community.Name.Contains(options.Keywords)
                            || t.Community.Location.Contains(options.Keywords)
                            || t.Community.Traffic.Contains(options.Keywords));
                }
                
                long totalCount = items.LongCount();//总搜索结果条数

                items = items.Include(h => h.Attachments).Include(h => h.Community)
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type).Include(h => h.Attachments);

                //排序
                switch (options.OrderByType)
                {
                    
                    case HouseSearchOrderByType.AreaAsc://房屋面积升序
                        items = items.OrderBy(t => t.Area);
                        break;
                    case HouseSearchOrderByType.AreaDesc://房屋面积降序
                        items = items.OrderByDescending(t => t.Area);
                        break;
                    case HouseSearchOrderByType.CreateDateDesc:
                        items = items.OrderByDescending(t => t.CreateDateTime);
                        break;
                    case HouseSearchOrderByType.MonthRentAsc://月租金额升序
                        items = items.OrderBy(t => t.MonthRent);
                        break;
                    case HouseSearchOrderByType.MonthRentDesc://月租金额降序
                        items = items.OrderByDescending(t => t.MonthRent);
                        break;
                }
                //一定不要items.Where
                //而要items=items.Where();
                //OrderBy要在Skip和Take之前
                //给用户看的页码从1开始，程序中是从0开始,跳过(当前页码-1)*每页多少条，取options.PageSize条数据
                items = items.Skip((options.CurrentIndex - 1) * options.PageSize)
                    .Take(options.PageSize);
                HouseSearchResult searchResult = new HouseSearchResult();
                searchResult.totalCount = totalCount;//总条数
                List<HouseDTO> houses = new List<HouseDTO>();
                //遍历取出来的房屋数据
                foreach (var item in items)
                {
                    //把房屋数据转换为dto
                    houses.Add(ToDTO(item));
                }
                searchResult.result = houses.ToArray();
                return searchResult;
            }
        }

        /// <summary>
        /// 更新房子的信息
        /// </summary>
        /// <param name="house"></param>
        public void Update(HouseDTO house)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                HouseEntity entity = bs.GetById(house.Id);
                entity.Address = house.Address;
                entity.Area = house.Area;
                //2,3,4
                entity.Attachments.Clear();//先删再加(先把旧的配套设施都清掉)
                var atts = ctx.Attachments.Where(a => a.IsDeleted == false &&
                    house.AttachmentIds.Contains(a.Id));
                foreach (AttachmentEntity att in atts)
                {
                    entity.Attachments.Add(att);
                }
                //3,4,5
                entity.CheckInDateTime = house.CheckInDateTime;
                entity.CommunityId = house.CommunityId;
                entity.DecorateStatusId = house.DecorateStatusId;
                entity.Description = house.Description;
                entity.Direction = house.Direction;
                entity.FloorIndex = house.FloorIndex;
                entity.LookableDateTime = house.LookableDateTime;
                entity.MonthRent = house.MonthRent;
                entity.OwnerName = house.OwnerName;
                entity.OwnerPhoneNum = house.OwnerPhoneNum;
                entity.RoomTypeId = house.RoomTypeId;
                entity.StatusId = house.StatusId;
                entity.TotalFloorCount = house.TotalFloorCount;
                entity.TypeId = house.TypeId;
                ctx.SaveChanges();
            }
        }
        /// <summary>
        /// 得到cityId这个城市今天的新增房源的数量
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public int GetTodayNewHouseCount(long cityId)
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> bs = new BaseService<HouseEntity>(ctx);
                //房子创建的时间是在当前时间内的24个小时，就认为是“今天的房源”
                //(DateTime.Now - h.CreateDateTime).TotalHours <= 24)表示当前时间减去房子的创建时间的小时数在24小时之内
                //(DateTime.Now - h.CreateDateTime)日期相减结果是TimeSpan类型
                /* 这种写法ef不支持
                return bs.GetAll().Count(h => h.Community.Region.CityId==cityId
                            && (DateTime.Now - h.CreateDateTime).TotalHours <= 24);*/
                /*
    DateTime date24HourAgo = DateTime.Now.AddHours(-24);//算出来一个24小时之前的时间
    return bs.GetAll().Count(h => h.Community.Region.CityId == cityId
               && h.CreateDateTime<=date24HourAgo);*/
                //SqlFunctions.DateDiff("hh", 较早的时间, 较晚的时间),SqlFunctions只支持sqlserver,建议用上面的写法
                return bs.GetAll().Count(h => h.Community.Region.CityId == cityId
                           && SqlFunctions.DateDiff("hh", h.CreateDateTime, DateTime.Now) <= 24);
            }
        }

        /// <summary>
        /// 获得所有的房源信息
        /// </summary>
        /// <returns></returns>
        public HouseDTO[] GetAll()
        {
            using (ZSZDbContext ctx = new ZSZDbContext())
            {
                BaseService<HouseEntity> houseBS = new BaseService<HouseEntity>(ctx);
                var houses = houseBS.GetAll()
                    .Include(h => h.Attachments).Include(h => h.Community)
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region)
                        + "." + nameof(RegionEntity.City))
                    .Include(nameof(HouseEntity.Community) + "." + nameof(CommunityEntity.Region))
                    .Include(h => h.DecorateStatus)
                    .Include(h => h.HousePics)
                    .Include(h => h.RoomType)
                    .Include(h => h.Status)
                    .Include(h => h.Type);
                return houses.ToList().Select(h => ToDTO(h)).ToArray();
            }
        }
    }
}
