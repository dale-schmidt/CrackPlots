using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class PersonAddRequest
    {
        [Required]
        [StringLength(256)]
        public string Email { get; set; }
        [Required]
        [StringLength(128)]
        public string AspNetUserId { get; set; }
    }
}