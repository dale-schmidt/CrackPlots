using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Domain
{
    public class Plot
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string PlotName { get; set; }
        public string Description { get; set; }
    }
}