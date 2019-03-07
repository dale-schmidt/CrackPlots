using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class SecurityToken
    {
        public string AspNetUserId { get; set; }
        public string Email { get; set; }
        public Guid TokenGuid { get; set; }
        public DateTime DateCreated { get; set; }
    }
}