using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class AspNetUserRoleAddRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}