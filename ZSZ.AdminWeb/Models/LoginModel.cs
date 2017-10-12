using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.Models
{
    /// <summary>
    /// 登入用户的model
    /// </summary>
    public class LoginModel
    {
        [Required]
        [StringLength(11,MinimumLength =6)]
        public string PhoneNum { get; set; }
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string VerifyCode { get; set; }
    }
}