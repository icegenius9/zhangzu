
using CodeCarvings.Piczard;
using CodeCarvings.Piczard.Filters.Watermarks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Models;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.DTO;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    public class HouseController : Controller
    {
        public IAdminUserService userSerivce { get; set; }

        public IHouseService houseService { get; set; }
        public ICityService cityService { get; set; }
        public IRegionService regionService { get; set; }
        public ICommunityService communityService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IAttachmentService attService { get; set; }


        /// <summary>
        /// 房源列表以及分页
        /// </summary>
        /// <param name="typeId">房源类型（整租、合租）</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [CheckPermission("House.List")]
        public ActionResult List(long typeId, int pageIndex = 1)
        {
            //因为AuthorizFilter做了是否登录的检查，因此这里不会取不到id
            long userId = (long)AdminHelper.GetUserId(HttpContext);
            //cityId可能为空，为空是总部
            long? cityId = userSerivce.GetById(userId).CityId;
            if (cityId == null)
            {
                //如果“总部不能***”的操作很多，也可以定义成一个AuthorizeFilter
                //最好用FilterAttribute的方式标注，这样对其他的不涉及这个问题的地方效率高
                //立即实现
                return View("Error", (object)"总部不能进行房源管理");
            }
            //分页
            var houses = houseService.GetPagedData(cityId.Value, typeId, 10, (pageIndex - 1) * 10);
            //获得总的条数
            long totalCount = houseService.GetTotalCount(cityId.Value, typeId);
            //当前页
            ViewBag.pageIndex = pageIndex;
            //总的条数
            ViewBag.totalCount = totalCount;
            //房源类型的id
            ViewBag.typeId = typeId;
            return View(houses);
        }

        /// <summary>
        /// //监听RegionId加载区域对应的小区
        /// </summary>
        /// <param name="regionId">区域的id</param>
        /// <returns></returns>
        public ActionResult LoadCommunities(long regionId)
        {
            var communities = communityService.GetByRegionId(regionId);
            return Json(new AjaxResult { Status = "ok", Data = communities });
        }

        [CheckPermission("House.Add")]
        [HttpGet]
        public ActionResult Add()
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }
            var regions = regionService.GetAll(cityId.Value);
            var roomTypes = idNameService.GetAll("户型");
            var statuses = idNameService.GetAll("房屋状态");
            var decorateStatuses = idNameService.GetAll("装修状态");
            var attachments = attService.GetAll();
            var types = idNameService.GetAll("房屋类别");

            HouseAddViewModel model = new HouseAddViewModel();
            model.regions = regions;
            model.roomTypes = roomTypes;
            model.statuses = statuses;
            model.decorateStatuses = decorateStatuses;
            model.attachments = attachments;
            model.types = types;
            return View(model);
        }

        //当客户端请求服务器时,发出的请求有html标签<p></p>这类的，会禁止请求基于安全考虑，从客户端***中检测到有潜在危险的Request.From值
        //XSS漏洞，用户在提交内容输入恶意的html代码，所以只有的确认没有人会恶意输入html代码,才能设置[ValidateInput(false)]
        //[ValidateInput(false)]就是关闭这样一个检测，
        [ValidateInput(false)]
        [CheckPermission("House.Add")]
        [HttpPost]
        public ActionResult Add(HouseAddModel model)
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }

            HouseAddNewDTO dto = new HouseAddNewDTO();
            dto.Address = model.address;
            dto.Area = model.area;
            dto.AttachmentIds = model.attachmentIds;
            dto.CheckInDateTime = model.checkInDateTime;
            dto.CommunityId = model.CommunityId;
            dto.DecorateStatusId = model.DecorateStatusId;
            dto.Description = model.description;
            dto.Direction = model.direction;
            dto.FloorIndex = model.floorIndex;
            dto.LookableDateTime = model.lookableDateTime;
            dto.MonthRent = model.monthRent;
            dto.OwnerName = model.ownerName;
            dto.OwnerPhoneNum = model.ownerPhoneNum;
            dto.RoomTypeId = model.RoomTypeId;
            dto.StatusId = model.StatusId;
            dto.TotalFloorCount = model.totalFloor;
            dto.TypeId = model.TypeId;

            long houseId = houseService.AddNew(dto);
            //生成房源查看的html文件
            CreateStaticPage(houseId); //生成房子的静态页面           


            return Json(new AjaxResult { Status = "ok" });
        }

        /// <summary>
        /// 创建前台房子详情的页面静态化
        /// </summary>
        /// <param name="houseId">房子的id</param>
        private void CreateStaticPage(long houseId)
        {
            var house = houseService.GetById(houseId);
            var pics = houseService.GetPics(houseId);
            var attachments = attService.GetAttachments(houseId);
            //由于静态化的类是前台的类,重新拷贝一个类到后台
            HouseIndexViewModel model = new HouseIndexViewModel();
            model.House = house;
            model.Pics = pics;
            model.Attachments = attachments;
                                                                            //由于静态化的是前台的房屋详情页面,不能跨程序集，在本项目中创建一个和前台房屋详情一样的cshtml
            string html = MVCHelper.RenderViewToString(this.ControllerContext, @"~/Views/House/StaticIndex.cshtml",
                model);
            //保存到前台文件夹中
            System.IO.File.WriteAllText(@"D:\UserData\My Documents\Visual Studio 2015\Projects\ZSZ\ZSZ.FrontWeb\" + houseId + ".html", html);
            
        }

        [CheckPermission("House.Edit")]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }
            var house = houseService.GetById(id);
            HouseEditViewModel model = new HouseEditViewModel();
            model.house = house;

            var regions = regionService.GetAll(cityId.Value);
            var roomTypes = idNameService.GetAll("户型");
            var statuses = idNameService.GetAll("房屋状态");
            var decorateStatuses = idNameService.GetAll("装修状态");
            var attachments = attService.GetAll();
            var types = idNameService.GetAll("房屋类别");

            model.regions = regions;
            model.roomTypes = roomTypes;
            model.statuses = statuses;
            model.decorateStatuses = decorateStatuses;
            model.attachments = attachments;
            model.types = types;
            return View(model);
        }

        [CheckPermission("House.Edit")]
        [HttpPost]
        public ActionResult Edit(HouseEditModel model)
        {
            HouseDTO dto = new HouseDTO();
            dto.Address = model.address;
            dto.Area = model.area;
            dto.AttachmentIds = model.attachmentIds;
            dto.CheckInDateTime = model.checkInDateTime;
            //有没有感觉强硬用一些不适合的DTO，有一些没用的属性时候的迷茫？
            dto.CommunityId = model.CommunityId;
            dto.DecorateStatusId = model.DecorateStatusId;
            dto.Description = model.description;
            dto.Direction = model.direction;
            dto.FloorIndex = model.floorIndex;
            dto.Id = model.Id;
            dto.LookableDateTime = model.lookableDateTime;
            dto.MonthRent = model.monthRent;
            dto.OwnerName = model.ownerName;
            dto.OwnerPhoneNum = model.ownerPhoneNum;
            dto.RoomTypeId = model.RoomTypeId;
            dto.StatusId = model.StatusId;
            dto.TotalFloorCount = model.totalFloor;
            dto.TypeId = model.TypeId;
            houseService.Update(dto);

           // CreateStaticPage(model.Id);//编辑房源的时候重新生成静态页面
            return Json(new AjaxResult { Status = "ok" });
        }
        /// <summary>
        /// 图片上传页面
        /// </summary>
        /// <param name="houseId">房子的id</param>
        /// <returns></returns>
        public ActionResult PicUpload(int houseId)
        {
            return View(houseId);
        }
        /// <summary>
        /// 图片上传的服务器
        /// </summary>
        /// <param name="houseId">房子的id</param>
        /// <param name="file">图片文件(HttpPostedFileBase名字必须是file)因为报文体中form-data中的name就叫file</param>
        /// <returns></returns>
        public ActionResult UploadPic(int houseId, HttpPostedFileBase file)
        {
            /*
            if (houseId < 5)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "id必须大于5" });
            }*/
            //month月，minute
            //把接受到的file文件内容转换为md5值
            string md5 = CommonHelper.CalcMD5(file.InputStream);
            //通过接受到的file文件取得到他的文件后缀
            string ext = Path.GetExtension(file.FileName);
            //把上传的图片文件放在upload/年/月/日 文件夹下已文件内容的md5值做文件名最后.jpg（相对路径）避免某一个文件夹下的文件太多
            string path = "/upload/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + md5 + ext;// /upload/2017/07/07/afadsfa.jpg
            //缩略图保存的相对路径
            string thumbPath = "/upload/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + md5 + "_thumb" + ext;
            //拿到物理路径d://22/upload/2017/07/07/afadsfa.jpg
            string fullPath = HttpContext.Server.MapPath("~" + path);
            //缩略图的物理路径(全路径)
            string thumbFullPath = HttpContext.Server.MapPath("~" + thumbPath);
            //尝试创建可能不存在的文件夹,如果文件夹存在也不会报错
            new FileInfo(fullPath).Directory.Create();
            

            file.InputStream.Position = 0;//指针复位(保存文件长度为0，IO流读到最后一位，在算MD5值得时候)
            //把图片路径保存到物理路径
            //file.SaveAs(fullPath);//SaveAs("d:/1.jpg");
            
            //缩略图
            ImageProcessingJob jobThumb = new ImageProcessingJob();
            jobThumb.Filters.Add(new FixedResizeConstraint(200, 200));//缩略图尺寸200*200
            //缩略图保存(file.InputStream流源,thumbFullPath保存到哪)
            jobThumb.SaveProcessedImageToFileSystem(file.InputStream, thumbFullPath);

            file.InputStream.Position = 0;//指针复位

            //水印
            ImageWatermark imgWatermark =                         //水印的路径                  
                new ImageWatermark(HttpContext.Server.MapPath("~/images/watermark.jpg"));
            imgWatermark.ContentAlignment = System.Drawing.ContentAlignment.BottomRight;//水印位置
            imgWatermark.Alpha = 50;//透明度，需要水印图片是背景透明的png图片
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.Filters.Add(imgWatermark);//添加水印
            //水印图片设置为600*600(如果需要原样尺寸把下面代码注释掉即可)
            jobNormal.Filters.Add(new FixedResizeConstraint(600, 600));
            //保存有水印的图
            jobNormal.SaveProcessedImageToFileSystem(file.InputStream, fullPath);

            //给房屋增加一个房屋图片,Url图片路径,ThumbUrl缩略图路径
            houseService.AddNewHousePic(new HousePicDTO { HouseId = houseId, Url = path, ThumbUrl = thumbPath });

            //CreateStaticPage(houseId);//上传了新图片或者删除图片都要重新生成html页面

            return Json(new AjaxResult
            {
                Status = "ok"
            });
        }

        /// <summary>
        /// 展示房子所有的图片
        /// </summary>
        /// <param name="id">房子的id</param>
        /// <returns></returns>
        public ActionResult PicList(long id)
        {
            var pics = houseService.GetPics(id);
            return View(pics);
        }

        public ActionResult DeletePics(long[] selectedIds)
        {
            foreach (var picId in selectedIds)
            {
                houseService.DeleteHousePic(picId);
            }

            //CreateStaticPage(houseId);//上传了新图片或者删除图片都要重新生成html页面
            //不建议删除图片
            return Json(new AjaxResult { Status = "ok" });
        }
        /// <summary>
        /// 把所有的房源信息生成静态页
        /// </summary>
        /// <returns></returns>
        public ActionResult RebuildAllStaticPage()
        {
            var houses = houseService.GetAll();
            foreach (var house in houses)
            {
                CreateStaticPage(house.Id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
    }
}