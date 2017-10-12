using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZSZ.FrontWeb.Models
{
    /// <summary>
    /// 前台用户注册的model
    /// </summary>
    public class UserRegModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [Phone]
        public string PhoneNum { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string SmsCode { get; set; }
        /// <summary>
        /// 第一次输入的密码
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 第二次输入的密码
        /// </summary>
        [Required]
        [Compare(nameof(Password))]
        public string Password2 { get; set; }
    }
}