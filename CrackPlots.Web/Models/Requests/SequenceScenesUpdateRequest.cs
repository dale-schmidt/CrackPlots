using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Requests
{
    public class SequenceScenesUpdateRequest
    {
        public List<SceneUpdateRequest> Scenes { get; set; }
    }
}