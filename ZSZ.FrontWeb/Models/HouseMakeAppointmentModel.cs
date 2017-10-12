using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZSZ.FrontWeb.Models
{
    /// <summary>
    /// 预约看房的的model
    /// </summary>
    public class HouseMakeAppointmentModel
    {
        /// <summary>
        /// 预约房子的id
        /// </summary>
        public long HouseId { get; set; }

        /// <summary>
        /// 预约人的姓名
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 预约人的手机号
        /// </summary>
        [Required]
        [Phone]
        public string PhoneNum { get; set; }
        /// <summary>
        /// 预约的日期
        /// </summary>
        [Required]
        public DateTime VisitDate { get; set; }
    }
}