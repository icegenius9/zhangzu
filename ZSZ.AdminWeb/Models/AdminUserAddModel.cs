using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.Models
{
    public class AdminUserAddModel
    {
        [Required]
        [Phone]
        public string PhoneNum { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]//表示Password2要跟Password相同(验证密码)
        public string Password2 { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public long? CityId { get; set; }
        public long[] RoleIds { get; set; }
    }
}
