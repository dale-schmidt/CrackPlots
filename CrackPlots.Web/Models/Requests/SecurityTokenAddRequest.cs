using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class SecurityTokenAddRequest
    {
        public string AspNetUserId { get; set; }
        public string Email { get; set; }
    }
}