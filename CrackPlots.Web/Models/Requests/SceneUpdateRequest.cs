using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class SceneUpdateRequest : SceneAddRequest
    {
        public int? Id { get; set; }
    }
}