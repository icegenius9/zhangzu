using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZSZ.CommonMVC;
using ZSZ.DTO;
using ZSZ.FrontWeb.Models;
using ZSZ.IService;

namespace ZSZ.FrontWeb.Controllers
{
    public class HouseController : Controller
    {
        public IHouseService houseService { get; set; }
        public IAttachmentService attService { get; set; }
        public ICityService cityService { get; set; }
        public IRegionService regionService { get; set; }

        public IHouseAppointmentService appService { get; set; }

        /// <summary>
        /// 测试MUI上拉加载
        /// </summary>
        /// <returns></returns>
        public ActionResult Test()
        {
            return View();
        }
        /*
        public ActionResult AA()
        {
            HouseSearchOptions opt = new HouseSearchOptions();
            opt.CityId = 1;
            opt.TypeId = 11;
            opt.StartMonthRent = 300;
            opt.OrderByType = HouseSearchOrderByType.AreaDesc;
            opt.Keywords = "楼";
            opt.PageSize = 10;
            opt.CurrentIndex = 1;
            var result =  houseService.Search(opt);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("总结过条数："+result.totalCount);
            foreach(var h in result.result)
            {
                sb.AppendLine(h.CommunityName+","+h.Area+","+h.MonthRent);
            }
            return Content(sb.ToString());
        }*/

        /// <summary>
        /// 分析月租金"200-300"、"300-*"这样的价格区间
        /// </summary>
        /// <param name="value">200-300</param>
        /// <param name="startMonthRent">解析出来的起始租金</param>
        /// <param name="endMonthRent">解析出来的结束租金</param>
        private void ParseMonthRent(string value,
            out int? startMonthRent, out int? endMonthRent)
        {
            //如果没有传递MonthRent参数，说明“不限制房租”
            if (string.IsNullOrEmpty(value))
            {
                startMonthRent = null;
                endMonthRent = null;
                return;
            }
            //先将200-300或者300-*用-进行分割
            string[] values = value.Split('-');
            string strStart = values[0];
            string strEnd = values[1];
            if (strStart == "*")
            {
                startMonthRent = null;//不设限
            }
            else
            {
                startMonthRent = Convert.ToInt32(strStart);
            }
            if (strEnd == "*")
            {
                endMonthRent = null;//不设限
            }
            else
            {
                endMonthRent = Convert.ToInt32(strEnd);
            }
        }
        /// <summary>
        /// 加载更多的数据配合search2实现下拉加载
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyWords">关键字</param>
        /// <param name="monthRent">价格区间</param>
        /// <param name="orderByType">排序规则</param>
        /// <param name="regionId">区域id</param>
        /// <param name="pageIndex">加载第几页的数据</param>
        /// <returns></returns>
        public ActionResult LoadMore(long typeId, string keyWords, string monthRent,
            string orderByType, long? regionId, int pageIndex)
        {
            //获得当前用户城市Id
            long cityId = FrontUtils.GetCityId(HttpContext);
            //创建搜索条件的类
            HouseSearchOptions searchOpt = new HouseSearchOptions();
            searchOpt.CityId = cityId;
            //加载第几页的数据
            searchOpt.CurrentIndex = pageIndex;

            //解析月租部分
            int? startMonthRent;
            int? endMonthRent;
            //ref/out
            ParseMonthRent(monthRent, out startMonthRent, out endMonthRent);
            searchOpt.EndMonthRent = endMonthRent;
            searchOpt.StartMonthRent = startMonthRent;

            searchOpt.Keywords = keyWords;
            //排序规则
            switch (orderByType)
            {
                case "MonthRentAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentAsc;
                    break;
                case "MonthRentDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentDesc;
                    break;
                case "AreaAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaAsc;
                    break;
                case "AreaDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaDesc;
                    break;
            }
            //每页的条数
            searchOpt.PageSize = 10;
            //区域的id
            searchOpt.RegionId = regionId;
            //房屋的类型(合租，整租)
            searchOpt.TypeId = typeId;

            //开始搜索
            var searchResult = houseService.Search(searchOpt);
            var houses = searchResult.result;
            return Json(new AjaxResult { Status = "ok", Data = houses });
        }
        /// <summary>
        /// 下拉加载数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyWords">关键字</param>
        /// <param name="monthRent">价格区间</param>
        /// <param name="orderByType">排序规则</param>
        /// <param name="regionId">区域id</param>
        /// <returns></returns>
        public ActionResult Search2(long typeId, string keyWords, string monthRent,
            string orderByType, long? regionId)
        {
            //获得当前用户城市Id
            long cityId = FrontUtils.GetCityId(HttpContext);
            //直接把搜索的所有房源传过去，通过下拉加载（页面通过ajax渲染）
            var regions = regionService.GetAll(cityId);
            return View(regions);
        }
        /// <summary>
        /// 展示搜索的页面
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyWords">关键字</param>
        /// <param name="monthRent">价格区间</param>
        /// <param name="orderByType">排序规则</param>
        /// <param name="regionId">区域id</param>
        /// <returns></returns>
        public ActionResult Search(long typeId, string keyWords, string monthRent,
            string orderByType, long? regionId)
        {
            //获得当前用户城市Id
            long cityId = FrontUtils.GetCityId(HttpContext);

            //获取城市下所有区域
            var regions = regionService.GetAll(cityId);
            HouseSearchViewModel model = new HouseSearchViewModel();
            model.regions = regions;

            //创建搜索条件的类
            HouseSearchOptions searchOpt = new HouseSearchOptions();
            searchOpt.CityId = cityId;
            //当前页的页码
            searchOpt.CurrentIndex = 1;

            //解析月租部分
            int? startMonthRent;
            int? endMonthRent;
            //ref/out
            ParseMonthRent(monthRent, out startMonthRent, out endMonthRent);
            searchOpt.EndMonthRent = endMonthRent;
            searchOpt.StartMonthRent = startMonthRent;
            //搜索关键字
            searchOpt.Keywords = keyWords;
            //排序规则
            switch (orderByType)
            {
                case "MonthRentAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentAsc;
                    break;
                case "MonthRentDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentDesc;
                    break;
                case "AreaAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaAsc;
                    break;
                case "AreaDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaDesc;
                    break;
            }
            //每页的条数
            searchOpt.PageSize = 10;
            //区域的id
            searchOpt.RegionId = regionId;
            //房屋的类型(合租，整租)
            searchOpt.TypeId = typeId;

            //开始搜索
            var searchResult = houseService.Search(searchOpt);
            model.houses = searchResult.result;
            //当前用户城市Id

            return View(model);
        }
        /// <summary>
        /// 根据房子的id返回房子对应的详细信息
        /// </summary>
        /// <param name="id">房子的id</param>
        /// <returns></returns>
        public ActionResult Index(long id)
        {

            //通过房子的id取得房子的信息
            var house = houseService.GetById(id);
            if (house == null)
            {
                return View("Error", (object)"不存在的房源id");
            }
            //通过房子的id取得房子的图片
            var pics = houseService.GetPics(id);
            // 通过房子的id取得房子的配套设施
            var attachments = attService.GetAttachments(id);
            //把房子所有的信息赋值给viewmodel
            HouseIndexViewModel model = new HouseIndexViewModel();
            model.House = house;
            model.Pics = pics;
            model.Attachments = attachments;

            ////缓存的key(key不能重复, 使用不同的房子不同的房子id)
            //string cacheKey = "HouseIndex_" + id;
            ////先尝试去缓存中找
            //HouseIndexViewModel model = (HouseIndexViewModel)HttpContext.Cache[cacheKey];
            //if (model == null)//缓存中没有找到,则去数据库取数据
            //{
            //    var house = houseService.GetById(id);
            //    if (house == null)
            //    {
            //        return View("Error", (object)"不存在的房源id");
            //    }
            //    var pics = houseService.GetPics(id);
            //    var attachments = attService.GetAttachments(id);

            //    model = new HouseIndexViewModel();
            //    model.House = house;
            //    model.Pics = pics;
            //    model.Attachments = attachments;
            //    //存入缓存(缓存的key,值为model,null,缓存的过期时间)
            //    HttpContext.Cache.Insert(cacheKey, model, null,
            //        DateTime.Now.AddMinutes(1), TimeSpan.Zero);
            //}


            //string cacheKey = "HouseIndex_" + id;
            ////先尝试去缓存中找,Memcach的类都必须是可序列化(包括关联的类)
            //HouseIndexViewModel model =
            //    MemcacheMgr.Instance.GetValue<HouseIndexViewModel>(cacheKey);
            //if (model == null)//缓存中没有找到
            //{
            //    var house = houseService.GetById(id);
            //    if (house == null)
            //    {
            //        return View("Error", (object)"不存在的房源id");
            //    }
            //    var pics = houseService.GetPics(id);
            //    var attachments = attService.GetAttachments(id);

            //    model = new HouseIndexViewModel();
            //    model.House = house;
            //    model.Pics = pics;
            //    model.Attachments = attachments;
            //    //存入缓存
            //    MemcacheMgr.Instance.SetValue(cacheKey, model
            //        , TimeSpan.FromMinutes(1));
        //}
            return View(model);
        }
        /// <summary>
        /// 看房预约订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult MakeAppointment(HouseMakeAppointmentModel model)
        {
            //验证表单的合法性
            if (!ModelState.IsValid)
            {
                string msg = MVCHelper.GetValidMsg(ModelState);
                return Json(new AjaxResult { Status = "erorr", ErrorMsg = msg });
            }
            //得到当前登录用户，没有的话为null
            long? userId = FrontUtils.GetUserId(HttpContext);
            appService.AddNew(userId, model.Name,
                model.PhoneNum, model.HouseId, model.VisitDate);
            return Json(new AjaxResult
            {
                Status = "ok"
            });
        }
    }
}