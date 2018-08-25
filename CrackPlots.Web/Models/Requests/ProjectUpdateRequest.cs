using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class ProjectUpdateRequest : ProjectAddRequest
    {
        public int Id { get; set; }
    }
}